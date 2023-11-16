using System.Collections.Generic;
using System;
using System.Configuration;
using System.IO;
using System.Windows.Documents;

namespace org.ohdsi.cdm.presentation.builder
{
    public class Settings
    {

        #region Properties
        public static Settings Current { get; set; }
        public BuildingSettings Building { get; set; }

        public string BuilderFolder { get; set; }

        public int DegreeOfParallelism => int.Parse(ConfigurationManager.AppSettings["DegreeOfParallelism"]);
        public bool WithinTheObservationPeriod => bool.Parse(ConfigurationManager.AppSettings["WithinTheObservationPeriod"]);

        public string CdmVersion = ConfigurationManager.AppSettings["CDM"];
        public string CdmMainVersion
        {
            get
            {
                var v = CdmVersion;

                int vf = v.IndexOf('.');
                int vL = v.LastIndexOf('.');

                if (vf != vL)
                {
                    v = v.Substring(0, vL);
                }

                return v;

            }
        }

    static Settings()
        {
            Current = new Settings();
        }

        public string DropVocabularyTablesScript => File.ReadAllText(
            Path.Combine(BuilderFolder, "ETL", "Common", "Scripts", Building.DestinationEngine.Database.ToString(), GetCdmVersionFolder(), "DropVocabularyTables.sql"));

        public string TruncateWithoutLookupTablesScript => File.ReadAllText(
            Path.Combine(BuilderFolder, "ETL", "Common", "Scripts", Building.DestinationEngine.Database.ToString(), GetCdmVersionFolder(), "TruncateWithoutLookupTables.sql"));

        public string TruncateTablesScript => File.ReadAllText(
            Path.Combine(BuilderFolder, "ETL", "Common", "Scripts", Building.DestinationEngine.Database.ToString(), GetCdmVersionFolder(), "TruncateTables.sql"));

        public string DropTablesScript => File.ReadAllText(
            Path.Combine(BuilderFolder, "ETL", "Common", "Scripts", Building.DestinationEngine.Database.ToString(), GetCdmVersionFolder(), "DropTables.sql"));

        public string TruncateLookupScript => File.ReadAllText(
            Path.Combine(BuilderFolder, "ETL", "Common", "Scripts", Building.DestinationEngine.Database.ToString(), GetCdmVersionFolder(), "TruncateLookup.sql"));

        public string CreateCdmTablesScript => File.ReadAllText(
            Path.Combine(BuilderFolder, "ETL", "Common", "Scripts", Building.DestinationEngine.Database.ToString(), GetCdmVersionFolder(), "CreateTables.sql"));

        public string PopulateCdmSourceScript => File.ReadAllText(
            Path.Combine(BuilderFolder, "ETL", "Common", "Scripts", Building.DestinationEngine.Database.ToString(), GetCdmVersionFolder(), "Populate_CdmSource.sql"));

        public string CreateCdmDatabaseScript => File.ReadAllText(
            Path.Combine(new[] {
                BuilderFolder,
                "ETL",
                "Common",
                "Scripts",
                Building.DestinationEngine.Database.ToString(),
                "CreateDestination.sql"
            })
        );

        public List<string> CreateDataCleaningIndexesScripts()
        {

            List<string> scripts = new List<string>();
            var path = Path.Combine(
                        BuilderFolder,
                        "ETL",
                        "Common",
                        "Scripts",
                        Building.SourceEngine.Database.ToString(),
                        "DataCleaning",
                        "01_CreateDataCleanningIndexes");

            foreach (string file in Directory.EnumerateFiles(path, "*.sql"))
            {
                scripts.Add(File.ReadAllText(file));
            }

            return scripts;
        }

        public string DataCleaningScript => File.ReadAllText(
            Path.Combine(
                        BuilderFolder, 
                        "ETL", 
                        "Common", 
                        "Scripts", 
                        Building.SourceEngine.Database.ToString(),
                        "DataCleaning",
                        "02_DataCleaning.sql")
        );

        public List<string> CreateMappingIndexesScripts() { 
        
            List<string> scripts = new List<string>();
            var path = Path.Combine(
                        BuilderFolder,
                        "ETL",
                        "Common",
                        "Scripts",
                        Building.SourceEngine.Database.ToString(),
                        "DataCleaning",
                        "03_CreateMappingIndexes");
            
            foreach (string file in Directory.EnumerateFiles(path, "*.sql"))
            {
                scripts.Add(File.ReadAllText(file));
            }

            return scripts;
        }

        public string CreateCdmPKScripts => File.ReadAllText(
            Path.Combine(
                        BuilderFolder,
                        "ETL",
                        "Common",
                        "Scripts",
                        Building.SourceEngine.Database.ToString(),
                        GetCdmVersionFolder(),
                        "CreateIndexes",
                        "01_CreatePKConstraints.sql")
        );

        public List<string> CreateCdmIndexesScripts()
        {
            List<string> scripts = new List<string>();
            var path = Path.Combine(
                        BuilderFolder,
                        "ETL",
                        "Common",
                        "Scripts",
                        Building.SourceEngine.Database.ToString(),
                        GetCdmVersionFolder(),
                        "CreateIndexes",
                        "02_CreateIndexes");

            foreach (string file in Directory.EnumerateFiles(path, "*.sql"))
            {
                scripts.Add(File.ReadAllText(file));
            }

            return scripts;
        }

        public List<string> CreateCdmFKScripts()
        {
            List<string> scripts = new List<string>();
            var path = Path.Combine(
                        BuilderFolder,
                        "ETL",
                        "Common",
                        "Scripts",
                        Building.SourceEngine.Database.ToString(),
                        GetCdmVersionFolder(),
                        "CreateIndexes",
                        "03_CreateFKConstraints");

            foreach (string file in Directory.EnumerateFiles(path, "*.sql"))
            {
                scripts.Add(File.ReadAllText(file));
            }

            return scripts;
        }
        

        public List<string> CreateCdmPkIdxForMappingScripts()
        {
            List<string> scripts = new List<string>();

            scripts.Add(File.ReadAllText(
                       Path.Combine(
                                   BuilderFolder,
                                   "ETL",
                                   "Common",
                                   "Scripts",
                                   Building.SourceEngine.Database.ToString(),
                                   "CreateIndexes",
                                   "pk_idx_in_Person.sql"))
            );

            scripts.Add(File.ReadAllText(
                       Path.Combine(
                                   BuilderFolder,
                                   "ETL",
                                   "Common",
                                   "Scripts",
                                   Building.SourceEngine.Database.ToString(),
                                   "CreateIndexes",
                                   "pk_idx_in_ObservationPeriod.sql"))
            );

            return scripts;
        }

        public List<string> PostCreateCdmPkIdx()
        {
            List<string> scripts = new List<string>();

            scripts.Add(File.ReadAllText(
                       Path.Combine(
                                   BuilderFolder,
                                   "ETL",
                                   "Common",
                                   "Scripts",
                                   Building.SourceEngine.Database.ToString(),
                                   "CreateIndexes",
                                   "pk_idx_visit_detail.sql"))
            );

            scripts.Add(File.ReadAllText(
                       Path.Combine(
                                   BuilderFolder,
                                   "ETL",
                                   "Common",
                                   "Scripts",
                                   Building.SourceEngine.Database.ToString(),
                                   "CreateIndexes",
                                   "pk_idx_visit_occurrence.sql"))
            );

            scripts.Add(File.ReadAllText(
                       Path.Combine(
                                   BuilderFolder,
                                   "ETL",
                                   "Common",
                                   "Scripts",
                                   Building.SourceEngine.Database.ToString(),
                                   "CreateIndexes",
                                   "pk_idx_drug_era.sql"))
            );

            scripts.Add(File.ReadAllText(
                      Path.Combine(
                                  BuilderFolder,
                                  "ETL",
                                  "Common",
                                  "Scripts",
                                  Building.SourceEngine.Database.ToString(),
                                  "CreateIndexes",
                                  "pk_idx_condition_era.sql"))
           );

            return scripts;
        }

        
        public string CreateCdmEraFksScript => File.ReadAllText(
            Path.Combine(new[] {
                BuilderFolder,
                "ETL",
                "Common",
                "Scripts",
                Building.DestinationEngine.Database.ToString(),
                "CreateIndexes",
                "fk_era_tables.sql"
            })
        );

        #endregion

        #region Methods
        public static void Initialize()
        {
            Current.Building = new BuildingSettings();
            Current.Building.Load();
        }

        public void Save(bool reloadVendorSettings)
        {
            Current.Building.Save(reloadVendorSettings);
        }

        public void Load()
        {
            Current.Building.Load();
        }

        private string GetCdmVersionFolder()
        {
            return CdmMainVersion;
        }

        #endregion
    }
}
