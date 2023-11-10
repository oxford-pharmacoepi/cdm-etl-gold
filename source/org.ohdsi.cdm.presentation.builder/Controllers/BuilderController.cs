using org.ohdsi.cdm.framework.common.Base;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Lookups;
using org.ohdsi.cdm.framework.common.Omop;
using org.ohdsi.cdm.framework.desktop.Base;
using org.ohdsi.cdm.framework.desktop.DbLayer;
using org.ohdsi.cdm.framework.desktop.Enums;
using org.ohdsi.cdm.framework.desktop.Helpers;
using org.ohdsi.cdm.framework.desktop.Savers;
using org.ohdsi.cdm.presentation.builder.Base;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.IO;



namespace org.ohdsi.cdm.presentation.builder.Controllers
{
    public class BuilderController
    {
        #region Variables

        private readonly ChunkController _chunkController;

        private List<Person> Persons = new List<Person>();
        private List<ObservationPeriod> ObservationPeriods = new List<ObservationPeriod>();

        private int pagesize = 50000;

        #endregion

        #region Properties
        private const string INX_FOR_DATA_CLEAN_CREATED = "DataCleaningIndexesCreated";
        private const string PROC_CREATED = "ProcedureCreated";
        private const string IDX_FOR_MAPPING_CREATED = "MappingIndexesCreated";
        private const string DATA_CLEAN_DONE = "DataCleaningIsDone";
        private const string DAYSUPPLY_TABLES_CREATED = "DaySupplyTablesCreated";

        private string[] sourceTables = {  "patient"
                                            //,
                                            //"consultation",
                                            //"clinical",
                                            //"additional",
                                            //"referral",
                                            //"immunisation",
                                            //"test",
                                            //"therapy"
                                        };

        private const string CDM_PK_CREATED = "CdmPKCreated";
        private const string CDM_IDX_CREATED = "CdmIndexesCreated";
        private const string CDM_FK_CREATED = "CdmFKCreated";

        public BuilderState CurrentState { get; set; }
        //public int CompleteChunksCount => Settings.Current.Building.CompletedChunkIds.Count;
        public int CompleteChunksCount = 0;

        #endregion

        #region Constructor

        public BuilderController()
        {
            _chunkController = new ChunkController();

            if (Settings.Current.Building.ChunksCount != 0)
            {
                SetCompleteChunksCount();
            }
        }

        #endregion

        #region Methods 

        public void SetCompleteChunksCount() {

            var dbCleaning = new DbCleaning(Settings.Current.Building.SourceConnectionString, Settings.Current.Building.SourceSchema);

            CompleteChunksCount = dbCleaning.GetCompletedChunksCount();

        }

        private void PerformAction(Action act)
        {

            if (CurrentState == BuilderState.Error)
                return;

            try
            {
                act();
            }
            catch (Exception e)
            {
                CurrentState = BuilderState.Error;
                Logger.WriteError(e);
            }
            finally
            {
            }
        }

        public List<Action> CreateCdmPkIdxActionList(DbSource db, List<string> queries)
        {

            var actions = new List<Action>();
            //var queries = Settings.Current.CreateCdmPkIdxScripts();

            foreach (var query in queries)
            {
                actions.Add(
                        () => { db.ExecuteQuery(query); }
                );
            };
            return actions;
        }

        public List<Action> CreateDataCleaningIndexesActionList(DbCleaning dbCleaning) {

            var actions = new List<Action>();
            var queries = Settings.Current.CreateDataCleaningIndexesScripts();

            foreach (var query in queries)
            {
                actions.Add(
                        () => { dbCleaning.ExecuteQuery(query); }
                );
            };
            return actions;
        }

        public List<Action> CreateDataCleaningActionList(DbCleaning dbCleaning) {
            var actions = new List<Action>();

            foreach (string tableName in sourceTables)
            {
                if (tableName != sourceTables[0]) {
                    actions.Add(
                        () => {
                            if (!Settings.Current.Building.DataCleaningSteps.Contains(tableName))
                            {
                                Debug.WriteLine("Data Cleaning in " + tableName + " started");
                                Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in " + tableName + " started");

                                string query = $"CALL pr_DataCleaning('{tableName}')";
                                Debug.WriteLine("query=" + query);
                                dbCleaning.DataCleaning(query);

                                Settings.Current.Building.DataCleaningSteps.Add(tableName);

                                Debug.WriteLine("Data Cleaning in " + tableName + " ended");
                                Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in " + tableName + " ended");
                            }

                        }
                    );
                }
            }

            Debug.WriteLine("Number of actions=" + actions.Count);

            return actions;
        }

        public List<Action> CreateMappingIndexesActionList(DbCleaning dbCleaning)
        {
            var actions = new List<Action>();
            var queries = Settings.Current.CreateMappingIndexesScripts();

            foreach (string query in queries)
            {
                actions.Add(
                         () => { dbCleaning.ExecuteQuery(query); }
                );
            }

            return actions;

        }

        public List<Action> CreateCdmPKConstraintsActionList(DbDestination dbDestination)
        {
            var actions = new List<Action>();
            var queries = Settings.Current.CreateCdmPKScripts;

            foreach (var query in queries.Split(new[] {";"}, StringSplitOptions.None))
            {
                actions.Add(
                         () => { dbDestination.ExecuteQuery(query); }
                );
            }

            return actions;

        }

        public List<Action> CreateCdmIndexesActionList(DbDestination dbDestination)
        {
            var actions = new List<Action>();
            var queries = Settings.Current.CreateCdmIndexesScripts();

            foreach (string query in queries)
            {
                actions.Add(
                         () => { dbDestination.ExecuteQuery(query); }
                );
            }

            return actions;

        }

        public List<Action> CreateCdmFKConstraintsActionList(DbDestination dbDestination)
        {
            var actions = new List<Action>();
            var queries = Settings.Current.CreateCdmFKScripts();

            foreach (string query in queries)
            {
                actions.Add(
                         () => { dbDestination.ExecuteQuery(query); }
                );
            }

            return actions;

        }

        public void DataCleaning()
        {
            PerformAction(() =>
            {
                var timer = new Stopwatch();
                timer.Start();

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Data Cleaning Started ====================");

                var dbCleaning = new DbCleaning(Settings.Current.Building.SourceConnectionString, Settings.Current.Building.SourceSchema);
                //var createDataCleaningMappingIndexesActions = CreateDataCleaningIndexesActionList(dbCleaning);
                //var datacleaningActions = CreateDataCleaningActionList(dbCleaning);
                var createMappingIndexesActions = CreateMappingIndexesActionList(dbCleaning);

                /* Clean invalid patient only
                 * patid is PK in patient -> no extra index required
                //1. Add indexes for data cleaning in the source tables
                if (Settings.Current.Building.DataCleaningSteps.Contains(INX_FOR_DATA_CLEAN_CREATED))
                {
                    Logger.Write(null, LogMessageTypes.Info, $"Indexes for Data Cleaning are already created");
                }
                else {
                    //In parallel by table
                    Parallel.ForEach(createDataCleaningMappingIndexesActions, action => action());
                    Settings.Current.Building.DataCleaningSteps.Add(INX_FOR_DATA_CLEAN_CREATED);

                    Logger.Write(null, LogMessageTypes.Info, $"Indexes for Data Cleaning are created");
                }
                */

                //2. Create functions and store procedure in db 
                if (Settings.Current.Building.DataCleaningSteps.Contains(PROC_CREATED))
                {
                    Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning Procedures already created");
                }
                else {
                    dbCleaning.CreateProcedure(Settings.Current.DataCleaningScript);
                    Settings.Current.Building.DataCleaningSteps.Add(PROC_CREATED);

                    Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning Procedures created");
                }
                
                //3. Perform Data Cleaning
                //Patient MUST clean first!!!
                if (Settings.Current.Building.DataCleaningSteps.Contains(PROC_CREATED) && 
                    !Settings.Current.Building.DataCleaningSteps.Contains(sourceTables[0]))
                {
                    Debug.WriteLine("Data Cleaning in " + sourceTables[0] + " started");
                    Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in " + sourceTables[0] + " started");

                    string query = $"CALL pr_DataCleaning('{sourceTables[0]}')";

                    dbCleaning.DataCleaning(query);
                    Settings.Current.Building.DataCleaningSteps.Add(sourceTables[0]);
                    Settings.Current.Building.DataCleaningSteps.Add(DATA_CLEAN_DONE);

                    Debug.WriteLine("Data Cleaning in " + sourceTables[0] + " ended");
                    Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in " + sourceTables[0] + " ended");
                }

                /*
                if(Settings.Current.Building.DataCleaningSteps.Contains(PROC_CREATED) &&
                    Settings.Current.Building.DataCleaningSteps.Contains(sourceTables[0]))
                {
                    Logger.Write(null, LogMessageTypes.Info, sourceTables[0] + $" is clean");

                    //Parallel Run
                    Parallel.ForEach(datacleaningActions, action => action());
                    Settings.Current.Building.DataCleaningSteps.Add(DATA_CLEAN_DONE);

                    Debug.WriteLine("Data Cleaning in all tables ended");
                    Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in all tables ended");

                }
                */
                /*
                //4. Add indexes for Mapping in the source tables              
                if (Settings.Current.Building.DataCleaningSteps.Contains(DATA_CLEAN_DONE) && 
                    !Settings.Current.Building.DataCleaningSteps.Contains(IDX_FOR_MAPPING_CREATED))
                {

                    //In parallel by table
                    Parallel.ForEach(createMappingIndexesActions, action => action());
                    Settings.Current.Building.DataCleaningSteps.Add(IDX_FOR_MAPPING_CREATED);
                    Logger.Write(null, LogMessageTypes.Info, $"Indexes for Mapping are created");

                }
                */
                /*
                //To-Do, update as store procedure
                //5. Create Daysupply Tables
                if (Settings.Current.Building.DataCleaningSteps.Contains(DATA_CLEAN_DONE) &&
                    Settings.Current.Building.DataCleaningSteps.Contains(IDX_FOR_MAPPING_CREATED) &&
                    !Settings.Current.Building.DataCleaningSteps.Contains(DAYSUPPLY_TABLES_CREATED))
                {
                    //Run by sequence as running in parallel can slow down the process
                    dbCleaning.CreateProcedure(Settings.Current.CreateDaySupplyTablesScript);
                    string query = "CALL pr_CreateDaySupplyTables()";
                    dbCleaning.DataCleaning(query);

                    Settings.Current.Building.DataCleaningSteps.Add(DAYSUPPLY_TABLES_CREATED);
                    Logger.Write(null, LogMessageTypes.Info, $"DaySupply Tables are created");
                }     
                */

                timer.Stop();

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Data Cleaning Ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");
            });
        }

        public void CreateDestination()
        {
            PerformAction(() =>
            {
                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Create Destination Started ====================");

                var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                    Settings.Current.Building.CdmSchema);

                dbDestination.CreateDatabase(Settings.Current.CreateCdmDatabaseScript);
                dbDestination.ExecuteQuery(Settings.Current.CreateCdmTablesScript);

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Create Destination Ended ====================");
            });
        }

        public void PopulateCdmSource() {
            PerformAction(() =>
            {
                Logger.Write(null, LogMessageTypes.Info,
                   $"==================== Populate CDM SOURCE Started ====================");

                var timer = new Stopwatch();
                timer.Start();

                var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                    Settings.Current.Building.CdmSchema);
                dbDestination.ExecuteQuery(Settings.Current.PopulateCdmSourceScript.Replace("{CdmVersion}", Settings.Current.CdmVersion));


                timer.Stop();
                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Populate CDM SOURCE Ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");
            });
        }

        public void CreateCdmPkIdxForVisitEra() {
            PerformAction(() =>
            {
                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Create PK and Indexes in Visit and Era Tables Started ====================");

                var timer = new Stopwatch();
                timer.Start();

                var dbDestination = new DbSource(Settings.Current.Building.DestinationConnectionString,
                                            "",
                                            Settings.Current.Building.SourceSchema,
                                            Settings.Current.Building.ChunkSize,
                                            Settings.Current.Building.CdmSchema);

                var createCdmPkIdxActions = CreateCdmPkIdxActionList(dbDestination, Settings.Current.PostCreateCdmPkIdx());

                Parallel.ForEach(createCdmPkIdxActions, action => action());

                timer.Stop();
                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Create PK and Indexes in Visit and Era Tables Ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");
            });
        }

        public void CreateTablesStep()
        {
            var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                Settings.Current.Building.CdmSchema);

            dbDestination.ExecuteQuery(Settings.Current.CreateCdmTablesScript);
        }

        public void DropDestination()
        {
            var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                Settings.Current.Building.CdmSchema);

            dbDestination.ExecuteQuery(Settings.Current.DropTablesScript);
        }

        public void TruncateLookup()
        {
            var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                Settings.Current.Building.CdmSchema);

            dbDestination.ExecuteQuery(Settings.Current.TruncateLookupScript);
        }

        public void TruncateTables()
        {
            var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                Settings.Current.Building.CdmSchema);

            dbDestination.ExecuteQuery(Settings.Current.TruncateTablesScript);
        }

        public void TruncateWithoutLookupTables()
        {
            var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                Settings.Current.Building.CdmSchema);

            dbDestination.ExecuteQuery(Settings.Current.TruncateWithoutLookupTablesScript);
        }

        public void ResetVocabularyStep()
        {
            var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                Settings.Current.Building.CdmSchema);

            dbDestination.ExecuteQuery(Settings.Current.DropVocabularyTablesScript);
        }

        public void MapLocationAndCareSite(IVocabulary vocabulary) {

            var locationConcepts = new List<Location>();
            var careSiteConcepts = new List<CareSite>();

            Console.WriteLine("Loading locations...");
            var location = Settings.Current.Building.SourceQueryDefinitions.FirstOrDefault(qd => qd.Locations != null);
            if (location != null)
            {
                FillList<Location>(locationConcepts, location, location.Locations[0], -1);
            }

            //if (locationConcepts.Count == 0)
            //    locationConcepts.Add(new Location { Id = Entity.GetId(null) });
            Console.WriteLine("Locations was loaded");

            Console.WriteLine("Loading care sites...");
            var careSite = Settings.Current.Building.SourceQueryDefinitions.FirstOrDefault(qd => qd.CareSites != null);
            if (careSite != null)
            {
                FillList<CareSite>(careSiteConcepts, careSite, careSite.CareSites[0], -1);
            }

            //if (careSiteConcepts.Count == 0)
            //    careSiteConcepts.Add(new CareSite { Id = 0, LocationId = 0, OrganizationId = 0, PlaceOfSvcSourceValue = null });
            Console.WriteLine("Care sites was loaded");

            Console.WriteLine("Saving locations and CareSite...");
            var saver = Settings.Current.Building.DestinationEngine.GetSaver();
            using (saver.Create(Settings.Current.Building.DestinationConnectionString,
                Settings.Current.Building.Cdm,
                Settings.Current.Building.SourceSchema,
                Settings.Current.Building.CdmSchema))
            {
                try
                {

                    saver.SaveEntity(locationConcepts, "LOCATION");
                    saver.SaveEntity(careSiteConcepts, "CARE_SITE");
                    saver.Commit();
                }
                catch (Exception e)
                {
                    Logger.WriteError(e);
                    saver.Rollback();
                    throw;
                }
                finally
                {
                    locationConcepts.Clear();
                    careSiteConcepts.Clear();
                    locationConcepts = null;
                    careSiteConcepts = null;
                }
            }

            Console.WriteLine("locations and CareSite were saved ");

        }

        public void MapProvider(IVocabulary vocabulary) {

            Console.WriteLine("Loading providers...");
            var provider = Settings.Current.Building.SourceQueryDefinitions.FirstOrDefault(qd => qd.Providers != null);

            var pagesize = 50000;

            var dbDestination = new DbSource(Settings.Current.Building.DestinationConnectionString,
                                            "",
                                            Settings.Current.Building.SourceSchema,
                                            Settings.Current.Building.ChunkSize,
                                            Settings.Current.Building.CdmSchema);


            var staffCount = dbDestination.GetStaffCount();


            var n = (int)Math.Ceiling(staffCount / pagesize);

            Debug.WriteLine("n=" + n);

            Parallel.For(0, n,
                new ParallelOptions { MaxDegreeOfParallelism = Settings.Current.DegreeOfParallelism }, (i) => {
                    Console.WriteLine($"page={i}");

                    var timer = new Stopwatch();
                    timer.Start();

                    var providerConcepts = new List<Provider>();

                    if (provider != null)
                    {
                        FillEntityByPage<Provider>(providerConcepts, provider, provider.Providers[0], pagesize, i);
                    }
                    Console.WriteLine("Providers was loaded");

                    var saver = Settings.Current.Building.DestinationEngine.GetSaver();
                    try
                    {

                        using (saver.Create(Settings.Current.Building.DestinationConnectionString,
                            Settings.Current.Building.Cdm,
                            Settings.Current.Building.SourceSchema,
                            Settings.Current.Building.CdmSchema))
                        {
                            saver.SaveEntity(providerConcepts, "PROVIDER");
                            saver.Commit();
                        }

                    }
                    catch (Exception e)
                    {
                        Logger.WriteError(e);
                        saver.Rollback();
                        throw;
                    }
                    finally {
                        providerConcepts.Clear();
                        providerConcepts = null;
                    }

                    timer.Stop();

                    //Logger.Write(null, LogMessageTypes.Info,
                    //    $"==================== PROVIDER Page {i} ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");

           });

        }

        public void MapDeath(IVocabulary vocabulary)
        {
            //vocabulary.Fill(false, false);

            Logger.Write(null, LogMessageTypes.Info,
                   $"==================== Map ALL Death started ====================");

            Console.WriteLine("Loading Death...");

            var dbDestination = new DbSource(Settings.Current.Building.DestinationConnectionString,
                                            "",
                                            Settings.Current.Building.SourceSchema,
                                            Settings.Current.Building.ChunkSize,
                                            Settings.Current.Building.CdmSchema);



            var personCount = dbDestination.GetPersonCount();
            var n = (int)Math.Ceiling(personCount / pagesize);

            Debug.WriteLine("personCount=" + personCount);
            Debug.WriteLine("pagesize=" + pagesize);
            Debug.WriteLine("n=" + n);

            var dead = Settings.Current.Building.SourceQueryDefinitions.FirstOrDefault(qd => qd.Death != null);

            Parallel.For(0, n,
                new ParallelOptions { MaxDegreeOfParallelism = Settings.Current.DegreeOfParallelism }, (i) => {
                    Console.WriteLine($"page={i}");
                    var timer = new Stopwatch();
                    timer.Start();

                    List<Death> death = new List<Death>();
                    List<Death> deathRaw = new List<Death>();

                    List<ObservationPeriod> ops = loadDeathObservationPeriod(Settings.Current.Building.SourceConnectionString, Settings.Current.Building.SourceSchema, Settings.Current.Building.CdmSchema, i);

                    if (dead != null)
                        FillEntityByPage<Death>(deathRaw, dead, dead.Death[0], pagesize, i);

                    foreach (var d in deathRaw)
                    {
                        if (d.StartDate.Year <= DateTime.Now.Year)
                        {
                            // If WithinTheObservationPeriod = true
                            // Reject Death if coming before the start of observation period
                            // and after 3 months from the end of it
                            if (Settings.Current.WithinTheObservationPeriod)
                            {
                                var op = ops.FirstOrDefault(op => op.PersonId == d.PersonId);

                                if (d.StartDate >= op.StartDate && d.StartDate.Date <= op.EndDate.Value.Date.AddMonths(3))
                                    death.Add(d);

                            }
                            else {
                                death.Add(d);
                            }
                        }

                    }

                    Console.WriteLine("Death was loaded");

                    var saver = Settings.Current.Building.DestinationEngine.GetSaver();
                    using (saver.Create(Settings.Current.Building.DestinationConnectionString,
                        Settings.Current.Building.Cdm,
                        Settings.Current.Building.SourceSchema,
                        Settings.Current.Building.CdmSchema))
                    {
                        try
                        {
                            saver.SaveEntity(death, "DEATH");
                            saver.Commit();
                        }
                        catch (Exception e)
                        {
                            Logger.WriteError(e);
                            saver.Rollback();
                            throw;
                        }
                        finally
                        {
                            death.Clear();
                            deathRaw.Clear();
                            death = null;
                            deathRaw = null;
                        }
                    }

                    Console.WriteLine("Death is saved ");

                    timer.Stop();

                    //Logger.Write(null, LogMessageTypes.Info,
                    //    $"==================== Page {i} ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");

                }
            );
            
            Logger.Write(null, LogMessageTypes.Info,
                $"==================== Map ALL Death ended ====================");

        }

        public void CreateLookup(IVocabulary vocabulary)
        {
            PerformAction(() =>
            {
                var timer = new Stopwatch();
                timer.Start();

                vocabulary.Fill(true, false);
                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Mapping Care site, Location and Provider Started ====================");

                MapLocationAndCareSite(vocabulary);
                MapProvider(vocabulary);

                Console.WriteLine("Lookups was saved ");
                timer.Stop();

                GC.Collect();

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Mapping Care site, Location and Provider Ended ====================");
            });
        }

        public void CreatChunks()
        {
            var timer = new Stopwatch();
            timer.Start();

            Console.WriteLine("Creating chunks...");
            Logger.Write(null, LogMessageTypes.Info,
                $"==================== Create Chunks started ====================");

            Settings.Current.Building.ChunksCount = _chunkController.CreateChunk();
            Console.WriteLine($"{Settings.Current.Building.ChunksCount} chunks have been created");
            timer.Stop();

            Logger.Write(null, LogMessageTypes.Info,
                $"==================== Create Chunks ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");

        }

        public PatientData loadPatients(QueryDefinition qd, int pagesize, int i){
            //var timer = new Stopwatch();
            //timer.Start();
            PatientData data = new PatientData();

            //Logger.Write(null, LogMessageTypes.Info,
            //    $"==================== Loading Patients... ====================");
            Console.WriteLine("Loading Patients...");

            if (qd != null)
                FillEntityByPage<Person>(data.personList, qd, qd.Persons[0], pagesize, i);

            foreach (var p in data.personList)
            {

                bool validPerson = true;

                if (p.YearOfBirth > DateTime.Now.Year)
                {
                    data.metadataList.Add(new Metadata { PersonId = p.PersonId, Name = "Implausible year of birth - post earliest observation period" });
                    validPerson = false;
                }

                if (!(p.StartDate < p.EndDate))
                {
                    data.metadataList.Add(new Metadata { PersonId = p.PersonId, Name = "Invalid observation time" });
                    validPerson = false;
                }

                // Delete any individual that has an OBSERVATION_PERIOD that is >= 2 years prior to the YEAR_OF_BIRTH
                if (p.YearOfBirth - p.StartDate.Year >= 2)
                {
                    data.metadataList.Add(new Metadata { PersonId = p.PersonId, Name = "Implausible year of birth - post earliest observation period" });
                    validPerson = false;
                }

                if (validPerson)
                {
                    data.ObservationPeriodList.Add(
                        new ObservationPeriod
                        {
                            PersonId = p.PersonId,
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            TypeConceptId = p.TypeConceptId,
                            AdditionalFields = p.AdditionalFields
                        }
                    );
                }
            }

            Console.WriteLine("Patient was loaded");

            Console.WriteLine("Excluding invalid patients...");
            var pb = new PersonBuilder();
            var result = pb.BuildPerson(data.personList);

            var person = result.Key;

            // Exclude person 
            foreach (var tmp in data.metadataList)
            {
                Person xPerson = data.personList.Find(item => item.PersonId == tmp.PersonId);
                data.personList.Remove(xPerson);
            }
            Console.WriteLine("Excluding invalid patients ended");

            Debug.WriteLine($"personList.Count={data.personList.Count}");
            Debug.WriteLine($"ObservationPeriodList.Count={data.ObservationPeriodList.Count}");
            Debug.WriteLine($"metadataList.Count={data.metadataList.Count}");
            //Debug.WriteLine($"deathList.Count={deathList.Count}");

            //timer.Stop();

            //Logger.Write(null, LogMessageTypes.Info,
            //    $"==================== Loading Patients ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");

            return data;
        }

        public void SavePatients(PatientData data) {

            //var timer = new Stopwatch();
            //timer.Start();

            //Logger.Write(null, LogMessageTypes.Info,
            //    $"==================== Saving Patients ====================");

            Console.WriteLine("Saving Person...");
            Console.WriteLine("Saving Observation Period...");
            Console.WriteLine("Saving Metadata_tmp...");
            //Console.WriteLine("Saving death...");

            Debug.WriteLine($"data.personList.Count={data.personList.Count}");
            Debug.WriteLine($"data.ObservationPeriodList.Count={data.ObservationPeriodList.Count}");
            Debug.WriteLine($"data.metadataList.Count={data.metadataList.Count}");

            var saver = Settings.Current.Building.DestinationEngine.GetSaver();

            try
            {

                using (saver.Create(Settings.Current.Building.DestinationConnectionString,
                    Settings.Current.Building.Cdm,
                    Settings.Current.Building.SourceSchema,
                    Settings.Current.Building.CdmSchema))
                {
                    saver.SaveEntity(data.personList, "PERSON");
                    saver.SaveEntity(data.ObservationPeriodList, "OBSERVATION_PERIOD");

                    //saver.SaveEntity(deathList, "DEATH");
                    saver.SaveMetadata(data.metadataList);
                    saver.Commit();
                }

            }
            catch (Exception e)
            {
                Logger.WriteError(e);
                saver.Rollback();
                throw;
            }
            finally {
                data.Clean();
            }
            //timer.Stop();

            //Logger.Write(null, LogMessageTypes.Info,
            //    $"==================== Save Patients ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");
        }

        public void MapAllPatients(IVocabulary vocabulary) {

            //vocabulary.Fill(false, false);

            Logger.Write(null, LogMessageTypes.Info,
                   $"==================== Mapping ALL Patients started ====================");
            //var pagesize = 50000;

            var dbDestination = new DbSource(Settings.Current.Building.DestinationConnectionString,
                                            "",
                                            Settings.Current.Building.SourceSchema,
                                            Settings.Current.Building.ChunkSize,
                                            Settings.Current.Building.CdmSchema);


            var patientCount = dbDestination.GetPatientCount();

            var n = (int)Math.Ceiling(patientCount / pagesize);

            Debug.WriteLine("n=" + n);
            
            var ppl = Settings.Current.Building.SourceQueryDefinitions.FirstOrDefault(qd => qd.Persons != null);

            Parallel.For(0, n,
                new ParallelOptions { MaxDegreeOfParallelism = Settings.Current.DegreeOfParallelism }, (i) => {
                    Console.WriteLine($"page={i}");
                    var timer = new Stopwatch();
                    timer.Start();

                    var data = loadPatients(ppl, pagesize, i);
                    SavePatients(data);

                    timer.Stop();

                }
            );

            Logger.Write(null, LogMessageTypes.Info,
                   $"==================== Mapping ALL Patients ended ====================");

            Logger.Write(null, LogMessageTypes.Info, "==================== Add indexes to Observational Period started ====================");
            var timer = new Stopwatch();
            timer.Start();

            //var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString, Settings.Current.Building.CdmSchema);
            var createCdmPkIdxActions = CreateCdmPkIdxActionList(dbDestination, Settings.Current.CreateCdmPkIdxForMappingScripts());

            Parallel.ForEach(createCdmPkIdxActions, action => action());

            Logger.Write(null, LogMessageTypes.Info, 
                    $"==================== Add indexes to Person and Observational Period ended - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins ====================");
        }

        public List<ObservationPeriod> loadDeathObservationPeriod(String connectionString, string sourceSchemaName, string destinationSchemaName, int page)
        {
            List<ObservationPeriod> ops = new List<ObservationPeriod>();

            var sql = $"With a AS(" +
                                    $"SELECT person_id " +
                                    $"from {sourceSchemaName}.chunk_person " +
                                    $"order by person_id " +
                                    $"limit {pagesize} offset {pagesize*page}" +
                      $"),ch AS(" +
                                    $"select b.patid " +
                                    $"from a " +
                                    $"join {sourceSchemaName}.patient b on b.patid = a.person_id " +
                                    $"where b.deathdate is not null) " +
                      $"select op.* from ch " +
                      $"join {destinationSchemaName}.observation_period op on ch.patid = op.person_id";

            if (page == 0)
            {
                sql = sql.Replace("offset {pagesize*page}", "");
            }
            else
            {
                sql = sql.Replace("{pagesize*page}", (pagesize * page).ToString());
            }

            Debug.WriteLine($"sql={sql}");

            using (var sourceConnection = new OdbcConnection(connectionString))
            {
                sourceConnection.Open();

                using (var command = new OdbcCommand(sql, sourceConnection))
                {
                    command.CommandTimeout = 0;
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        ops.Add(
                                    new ObservationPeriod
                                    {
                                        Id = Convert.ToInt32(reader["observation_period_id"]),
                                        PersonId = (long)reader["person_id"],
                                        StartDate = Convert.ToDateTime(reader["observation_period_start_date"]),
                                        EndDate = Convert.ToDateTime(reader["observation_period_end_date"]),
                                        TypeConceptId = Convert.ToInt32(reader["period_type_concept_id"])
                                    }
                        );
                    }
                }
                sourceConnection.Close();
            }

            return ops;
        }


        private void FillEntityByPage<T>(ICollection<T> list, QueryDefinition qd, EntityDefinition ed, int pagesize, int page) where T : IEntity {

            var sql = GetSqlHelper.GetSql(Settings.Current.Building.SourceEngine.Database,
                qd.GetSql(Settings.Current.Building.Vendor, Settings.Current.Building.SourceSchema), Settings.Current.Building.SourceSchema);

            if (string.IsNullOrEmpty(sql)) return;

            //if(!qd.FileName.Contains("Patient")) return;

            sql = sql.Replace("{pagesize}", pagesize.ToString());
           
            if (page == 0) {
                sql = sql.Replace("offset {pagesize*page}", "");
            }
            else {
                sql = sql.Replace("{pagesize*page}", (pagesize*page).ToString());
            }

            Debug.WriteLine($"sql={sql}");
            
            var keys = new Dictionary<string, bool>();

            using (var connection = new OdbcConnection(Settings.Current.Building.SourceConnectionString))
            {
                connection.Open();
                using (var c = new OdbcCommand(sql, connection))
                {
                    c.CommandTimeout = 0;
                    using (var reader = c.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Concept conceptDef = null;
                            if (ed.Concepts != null && ed.Concepts.Any())
                                conceptDef = ed.Concepts[0];

                            var concept = (T)ed.GetConcepts(conceptDef, reader, null).ToList()[0];

                            if (!(qd.FileName.Contains("Patient") || qd.FileName.Contains("Death")))
                            {
                                var key = concept.GetKey();
                                if (key == null) continue;

                                if (keys.ContainsKey(key))
                                    continue;

                                keys.Add(key, false);

                            }

                            list.Add(concept);

                            if (CurrentState != BuilderState.Running)
                                break;
                        }
                    }
                }
                connection.Close();
            }
            
        }


        private void FillList<T>(ICollection<T> list, QueryDefinition qd, EntityDefinition ed, int chunkId) where T : IEntity
        {
            var sql = GetSqlHelper.GetSql(Settings.Current.Building.SourceEngine.Database,
                qd.GetSql(Settings.Current.Building.Vendor, Settings.Current.Building.SourceSchema), Settings.Current.Building.SourceSchema);

            sql = string.Format(sql, chunkId);

            Debug.WriteLine($"sql={sql}");

            if (string.IsNullOrEmpty(sql)) return;

            var keys = new Dictionary<string, bool>();
            
            using (var connection = new OdbcConnection(Settings.Current.Building.SourceConnectionString))
            {
                connection.Open();
                using (var c = new OdbcCommand(sql, connection))
                {
                    c.CommandTimeout = 0;
                    using (var reader = c.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Concept conceptDef = null;
                            if (ed.Concepts != null && ed.Concepts.Any())
                                conceptDef = ed.Concepts[0];

                                var concept = (T)ed.GetConcepts(conceptDef, reader, null).ToList()[0];

                            if (!(qd.FileName.Contains("Patient") || qd.FileName.Contains("Death")))
                            {
                                var key = concept.GetKey();
                                if (key == null) continue;

                                if (keys.ContainsKey(key))
                                    continue;

                                keys.Add(key, false);

                            }
                                list.Add(concept);

                            if (CurrentState != BuilderState.Running)
                                break;
                        }
                    }
                }
            }
        }

        public void Build(IVocabulary vocabulary)
        {
            var saveQueue = new BlockingCollection<DatabaseChunkPart>();

            PerformAction(() =>
            {
                /*
                if (Settings.Current.Building.ChunksCount == 0)
                {
                    Logger.Write(null, LogMessageTypes.Info, "==================== Create Chunks started ====================");
                    //Settings.Current.Building.ChunksCount = _chunkController.CreateChunks();
                    Settings.Current.Building.ChunksCount = _chunkController.CreateChunk();
                    Debug.WriteLine("Settings.Current.Building.ChunksCount=" + Settings.Current.Building.ChunksCount);
 
                    Logger.Write(null, LogMessageTypes.Info, "==================== Create Chunks ended ====================");
                }
                else
                {
                    Logger.Write(null, LogMessageTypes.Info, "==================== Chunks are already created ====================");
                }
                */
                //vocabulary.Fill(false, false);
                
                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Conversion to CDM was started ====================");

                List<int> incompleteChunkIds = _chunkController.GetIncompleteChunkId();
                incompleteChunkIds.Sort();
                //CompleteChunksCount = Settings.Current.Building.ChunksCount - incompleteChunkIds.Count();
                
                var save = Task.Run(() =>
                {
                    while (!saveQueue.IsCompleted)
                    {

                        DatabaseChunkPart data = null;
                        try
                        {
                            data = saveQueue.Take();
                        }
                        catch (InvalidOperationException)
                        {

                        }

                        
                        if (data != null)
                        {
                            var timer = new Stopwatch();
                            timer.Start();
                            
                            data.Save(Settings.Current.Building.Cdm,
                                Settings.Current.Building.DestinationConnectionString,
                                Settings.Current.Building.DestinationEngine,
                                Settings.Current.Building.SourceSchema,
                                Settings.Current.Building.CdmSchema);
                            
                            CompleteChunksCount++;

                            timer.Stop();
                            /*
                            Logger.Write(data.ChunkData.ChunkId, LogMessageTypes.Info,
                                $"ChunkId={data.ChunkData.ChunkId} was saved - {timer.ElapsedMilliseconds} ms | {GC.GetTotalMemory(false) / 1024f / 1024f} Mb");
                            */
                    
                                Logger.Write(data.ChunkData.ChunkId, LogMessageTypes.Info,
                                    $"ChunkId={data.ChunkData.ChunkId} was saved - {timer.ElapsedMilliseconds* 0.000016666666666666667:0.00} mins");
                            }

                            if (CurrentState != BuilderState.Running)
                                break;
                        }


                        CurrentState = BuilderState.Stopped;
                    
                });


                Parallel.For(0, incompleteChunkIds.Count(),
                        new ParallelOptions { MaxDegreeOfParallelism = Settings.Current.DegreeOfParallelism }, (i, state) => {
                            
                            var chunkId = incompleteChunkIds[i];

                            Debug.WriteLine($"i={i} incompleteChunkIds.Count={incompleteChunkIds.Count} chunkId={chunkId}");

                            if (CurrentState != BuilderState.Running)
                                state.Break();

                            if (!Settings.Current.Building.CompletedChunkIds.Contains(chunkId))
                            {
                                var chunk = new DatabaseChunkBuilder(chunkId, CreatePersonBuilder, Settings.Current.Building.ChunkSize);
                                //var chunk = new DatabaseChunkBuilder(chunkId, CreatePersonBuilder);

                                using (var connection =
                                new OdbcConnection(Settings.Current.Building.SourceConnectionString))
                                {
                                    connection.Open();
                                    
                                    saveQueue.Add(chunk.Process(Settings.Current.Building.SourceEngine,
                                    Settings.Current.Building.SourceSchema,
                                    Settings.Current.Building.CdmSchema,
                                    Settings.Current.Building.SourceQueryDefinitions,
                                    connection,
                                    Settings.Current.Building.Vendor));
                                    /*
                                    saveQueue.Add(chunk.Process(Settings.Current.Building.SourceEngine,
                                      Settings.Current.Building.SourceSchema,
                                      Settings.Current.Building.SourceQueryDefinitions,
                                      connection,
                                      Settings.Current.Building.Vendor));
                                    */
                                }

                                Settings.Current.Save(false);

                                while (saveQueue.Count > 0)
                                {
                                    Thread.Sleep(1000);
                                }
                            }
                            

                        });

                saveQueue.CompleteAdding();

                save.Wait();
            });

        }

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
        private static IPersonBuilder CreatePersonBuilder()
        {
            var objectType = Type.GetType(Settings.Current.Building.PersonBuilder);
            IPersonBuilder builder = Activator.CreateInstance(objectType) as IPersonBuilder;

            return builder;
        }

        #endregion
    }
}
