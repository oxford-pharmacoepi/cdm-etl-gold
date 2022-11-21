using org.ohdsi.cdm.framework.common.Base;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Lookups;
using org.ohdsi.cdm.framework.common.Omop;
using org.ohdsi.cdm.framework.desktop.Base;
using org.ohdsi.cdm.framework.desktop.DbLayer;
using org.ohdsi.cdm.framework.desktop.Enums;
using org.ohdsi.cdm.framework.desktop.Helpers;
using org.ohdsi.cdm.presentation.builder.Base;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;


namespace org.ohdsi.cdm.presentation.builder.Controllers
{
    public class BuilderController
    {
        #region Variables

        private readonly ChunkController _chunkController;

        #endregion

        #region Properties
        private const string INX_FOR_DATA_CLEAN_CREATED = "DataCleaningIndexesCreated";
        private const string PROC_CREATED = "ProcedureCreated";
        private const string IDX_FOR_MAPPING_CREATED = "MappingIndexesCreated";
        private const string DATA_CLEAN_DONE = "DataCleaningIsDone";

        private string[] sourceTables = {  "patient",
                                            "consultation",
                                            "clinical",
                                            "additional",
                                            "referral",
                                            "immunisation",
                                            "test",
                                            "therapy"
                                        };

        public BuilderState CurrentState { get; set; }
        public int CompleteChunksCount => Settings.Current.Building.CompletedChunkIds.Count;

        #endregion

        #region Constructor

        public BuilderController()
        {
            _chunkController = new ChunkController();
        }

        #endregion

        #region Methods 

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

        //TO-DO: 1. Create source tables structures
        //       2. Load source data

        public List<Action> CreateActionList(DbCleaning dbCleaning) {
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

                                string query = "CALL pr_DataCleaning('{tableName}')".Replace("{tableName}", tableName);
                                dbCleaning.ExecuteQuery(query);
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

        public void DataCleaning()
        {
            PerformAction(() =>
            {
                var timer = new Stopwatch();
                timer.Start();

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Data Cleaning Started ====================");

                var dbCleaning = new DbCleaning(Settings.Current.Building.SourceConnectionString, Settings.Current.Building.SourceSchema);
                var actions = CreateActionList(dbCleaning);

                //1. Add indexes for data cleaning in the source tables
                if (Settings.Current.Building.DataCleaningSteps.Contains(INX_FOR_DATA_CLEAN_CREATED))
                {
                    Logger.Write(null, LogMessageTypes.Info, $"Indexes for Data Cleaning are already created");
                }
                else {
                    dbCleaning.ExecuteQuery(Settings.Current.CreateDataCleaningIndexesScript);
                    Settings.Current.Building.DataCleaningSteps.Add(INX_FOR_DATA_CLEAN_CREATED);

                    Logger.Write(null, LogMessageTypes.Info, $"Indexes for Data Cleaning are created");
                }


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

                    string query = "CALL pr_DataCleaning('{tableName}')".Replace("{tableName}", sourceTables[0]);
                    dbCleaning.ExecuteQuery(query);
                    Settings.Current.Building.DataCleaningSteps.Add(sourceTables[0]);

                    Debug.WriteLine("Data Cleaning in " + sourceTables[0] + " ended");
                    Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in " + sourceTables[0] + " ended");
                }

                
                if(Settings.Current.Building.DataCleaningSteps.Contains(PROC_CREATED) &&
                    Settings.Current.Building.DataCleaningSteps.Contains(sourceTables[0]))
                {
                    Logger.Write(null, LogMessageTypes.Info, sourceTables[0] + $" is clean");

                    //Parallel Run
                    Parallel.ForEach(actions, action => action());
                    Settings.Current.Building.DataCleaningSteps.Add(DATA_CLEAN_DONE);

                    Debug.WriteLine("Data Cleaning in all tables ended");
                    Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in all tables ended");

                }
                
                /*
                // Clean table one by one
                foreach (string tableName in sourceTables) {
 
                    if (!Settings.Current.Building.DataCleaningSteps.Contains(tableName))
                    {
                        Debug.WriteLine("Data Cleaning in " + tableName + "started");
                        Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in " + tableName + " started");

                        string query = "CALL pr_DataCleaning('{tableName}')".Replace("{tableName}", tableName);
                        dbCleaning.ExecuteQuery(query);
                        //dbCleaning.DataCleaning(tableName);
                        Settings.Current.Building.DataCleaningSteps.Add(tableName);
                        
                        Debug.WriteLine("Data Cleaning in " + tableName + "ended");
                        Logger.Write(null, LogMessageTypes.Info, $"Data Cleaning in " + tableName + " ended");

                    }
                    Logger.Write(null, LogMessageTypes.Info, tableName + $" is clean");
                }
                */
                
                //4. Add indexes for Mapping in the source tables
                if (Settings.Current.Building.DataCleaningSteps.Contains(DATA_CLEAN_DONE) && 
                    !Settings.Current.Building.DataCleaningSteps.Contains(IDX_FOR_MAPPING_CREATED))
                {

                    Debug.WriteLine("Create Mapping indexes started");
                    Logger.Write(null, LogMessageTypes.Info, $"Create Mapping indexes started");

                    dbCleaning.ExecuteQuery(Settings.Current.CreateMappingIndexesScript);
                    Settings.Current.Building.DataCleaningSteps.Add(IDX_FOR_MAPPING_CREATED);
                    Logger.Write(null, LogMessageTypes.Info, IDX_FOR_MAPPING_CREATED);

                    Debug.WriteLine("Create Mapping indexes ended");
                    Logger.Write(null, LogMessageTypes.Info, $"Create Mapping indexes ended");
                }
                
                timer.Stop();

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Data Cleaning Ended - {timer.ElapsedMilliseconds} ms ====================");
            });
        }
        /*
        public void CreateCdmIndexes()
        {
            PerformAction(() =>
            {
                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Create Indexes in CDM tables Started ====================");

                var dbDestination = new DbDestination(Settings.Current.Building.DestinationConnectionString,
                   Settings.Current.Building.CdmSchema);

                dbDestination.CreateIndexes(Settings.Current.CreateCdmIndexesScript);


                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Create Indexes in CDM tables Ended ====================");
            });

        }
        */

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

        public void CreateLookup(IVocabulary vocabulary)
        {
            PerformAction(() =>
            {
                var timer = new Stopwatch();
                timer.Start();

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Create Lookup Started ====================");

                vocabulary.Fill(true, false);
                var locationConcepts = new List<Location>();
                var careSiteConcepts = new List<CareSite>();
                var providerConcepts = new List<Provider>();

                Console.WriteLine("Loading locations...");
                var location = Settings.Current.Building.SourceQueryDefinitions.FirstOrDefault(qd => qd.Locations != null);
                if (location != null)
                {
                    FillList<Location>(locationConcepts, location, location.Locations[0]);
                }

                if (locationConcepts.Count == 0)
                    locationConcepts.Add(new Location { Id = Entity.GetId(null) });
                Console.WriteLine("Locations was loaded");

                Console.WriteLine("Loading care sites...");
                var careSite = Settings.Current.Building.SourceQueryDefinitions.FirstOrDefault(qd => qd.CareSites != null);
                if (careSite != null)
                {
                    FillList<CareSite>(careSiteConcepts, careSite, careSite.CareSites[0]);
                }

                if (careSiteConcepts.Count == 0)
                    careSiteConcepts.Add(new CareSite { Id = 0, LocationId = 0, OrganizationId = 0, PlaceOfSvcSourceValue = null });
                Console.WriteLine("Care sites was loaded");

                Console.WriteLine("Loading providers...");
                var provider = Settings.Current.Building.SourceQueryDefinitions.FirstOrDefault(qd => qd.Providers != null);
                if (provider != null)
                {
                    FillList<Provider>(providerConcepts, provider, provider.Providers[0]);
                }
                Console.WriteLine("Providers was loaded");

                Console.WriteLine("Saving lookups...");
                var saver = Settings.Current.Building.DestinationEngine.GetSaver();
                using (saver.Create(Settings.Current.Building.DestinationConnectionString,
                    Settings.Current.Building.Cdm,
                    Settings.Current.Building.SourceSchema,
                    Settings.Current.Building.CdmSchema))
                {
                    saver.SaveEntityLookup(Settings.Current.Building.Cdm, locationConcepts, careSiteConcepts, providerConcepts, null);
                }

                Console.WriteLine("Lookups was saved ");
                timer.Stop();
                Logger.Write(null, LogMessageTypes.Info,
                    $"Care site, Location and Provider tables were saved to CDM database - {timer.ElapsedMilliseconds} ms");

                locationConcepts.Clear();
                careSiteConcepts.Clear();
                providerConcepts.Clear();
                locationConcepts = null;
                careSiteConcepts = null;
                providerConcepts = null;
                GC.Collect();

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Create Lookup Ended ====================");
            });
        }

        private void FillList<T>(ICollection<T> list, QueryDefinition qd, EntityDefinition ed) where T : IEntity
        {
            var sql = GetSqlHelper.GetSql(Settings.Current.Building.SourceEngine.Database,
                qd.GetSql(Settings.Current.Building.Vendor, Settings.Current.Building.SourceSchema), Settings.Current.Building.SourceSchema);

            if (string.IsNullOrEmpty(sql)) return;

            var keys = new Dictionary<string, bool>();
            using (var connection = new OdbcConnection(Settings.Current.Building.SourceConnectionString))
            {
                connection.Open();
                using (var c = new OdbcCommand(sql, connection))
                {
                    c.CommandTimeout = 30000;
                    using (var reader = c.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Concept conceptDef = null;
                            if (ed.Concepts != null && ed.Concepts.Any())
                                conceptDef = ed.Concepts[0];

                            var concept = (T)ed.GetConcepts(conceptDef, reader, null).ToList()[0];

                            var key = concept.GetKey();
                            if (key == null) continue;

                            if (keys.ContainsKey(key))
                                continue;

                            keys.Add(key, false);

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

                if (Settings.Current.Building.ChunksCount == 0)
                {
                    Debug.WriteLine("Settings.Current.Building.ChunksCount == 0 --> need to drop _chunk");
                    Settings.Current.Building.ChunksCount = _chunkController.CreateChunks();
                }

                vocabulary.Fill(false, false);

                Logger.Write(null, LogMessageTypes.Info,
                    $"==================== Conversion to CDM was started ====================");

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
                            Settings.Current.Building.CompletedChunkIds.Add(data.ChunkData.ChunkId);
                            timer.Stop();
                            Logger.Write(data.ChunkData.ChunkId, LogMessageTypes.Info,
                                $"ChunkId={data.ChunkData.ChunkId} was saved - {timer.ElapsedMilliseconds} ms | {GC.GetTotalMemory(false) / 1024f / 1024f} Mb");
                        }

                        if (CurrentState != BuilderState.Running)
                            break;
                    }

                    CurrentState = BuilderState.Stopped;
                });

                /*
                if(Settings.Current.OnlyEvenChunks)
                    Logger.Write(null, LogMessageTypes.Info, "Only even chunk ids will be processed on this machine");

                if (Settings.Current.OnlyOddChunks)
                    Logger.Write(null, LogMessageTypes.Info, "Only odd chunk ids will be processed on this machine");

                if(Settings.Current.ChunksTo > 0)
                    Logger.Write(null, LogMessageTypes.Info, $"ChunkIds from {Settings.Current.ChunksFrom} to {Settings.Current.ChunksTo} will be converted");
                */

                Debug.WriteLine("End Task Run");


                Parallel.For(1, Settings.Current.Building.ChunksCount,
                    new ParallelOptions { MaxDegreeOfParallelism = Settings.Current.DegreeOfParallelism }, (chunkId, state) =>
                      {
                          if (CurrentState != BuilderState.Running)
                              state.Break();

                          if (!Settings.Current.Building.CompletedChunkIds.Contains(chunkId))
                          {
                              /*
                              if(IsOdd(chunkId))
                              {
                                  if (Settings.Current.OnlyEvenChunks)
                                  {
                                      Logger.Write(null, LogMessageTypes.Info, $"{chunkId} was skipped");
                                      return;
                                  }
                              }
                              else
                              {
                                  if (Settings.Current.OnlyOddChunks)
                                  {
                                      Logger.Write(null, LogMessageTypes.Info, $"{chunkId} was skipped");
                                      return;
                                  }
                              }

                              if(chunkId < Settings.Current.ChunksFrom)
                              {
                                  Logger.Write(null, LogMessageTypes.Info, $"{chunkId} was skipped");
                                  return;
                              }
                              
                              if (chunkId > Settings.Current.ChunksTo)
                              {
                                  Logger.Write(null, LogMessageTypes.Info, $"{chunkId} was skipped");
                                  return;
                              }
                              */

                              var chunk = new DatabaseChunkBuilder(chunkId, CreatePersonBuilder);

                              using (var connection =
                                  new OdbcConnection(Settings.Current.Building.SourceConnectionString))
                              {
                                  connection.Open();
                                  saveQueue.Add(chunk.Process(Settings.Current.Building.SourceEngine,
                                      Settings.Current.Building.SourceSchema,
                                      Settings.Current.Building.SourceQueryDefinitions,
                                      connection,
                                      Settings.Current.Building.Vendor));
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
