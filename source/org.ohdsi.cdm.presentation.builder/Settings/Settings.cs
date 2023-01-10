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

        public bool OnlyEvenChunks => bool.Parse(ConfigurationManager.AppSettings["OnlyEvenChunks"]);
        public bool OnlyOddChunks => bool.Parse(ConfigurationManager.AppSettings["OnlyOddChunks"]);

        public int ChunksFrom => int.Parse(ConfigurationManager.AppSettings["ChunksFrom"]);
        public int ChunksTo => int.Parse(ConfigurationManager.AppSettings["ChunksTo"]);


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

        public string CreateCdmDatabaseScript => File.ReadAllText(
            Path.Combine(new[] {
                BuilderFolder,
                "ETL",
                "Common",
                "Scripts",
                Building.DestinationEngine.Database.ToString(),
                "CreateDestination.sql"
            }));

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
                        "02_DataCleaning.sql"));

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

        public string CreateDaySupplyTablesScript => File.ReadAllText(
           Path.Combine(
                       BuilderFolder,
                       "ETL",
                       "Common",
                       "Scripts",
                       Building.SourceEngine.Database.ToString(),
                       "DataCleaning",
                       "04_Create_daysupply_tables.sql"));

        /*
         public string CreateCdmIndexesScript => File.ReadAllText(
            Path.Combine(
                        BuilderFolder,
                        "ETL",
                        "Common",
                        "Scripts",
                        Building.SourceEngine.Database.ToString(),
                        GetCdmVersionFolder(),
                        "CreateIndexes.sql"));
        */

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
            return ConfigurationManager.AppSettings["CDM"];
        }

        #endregion
    }
}
