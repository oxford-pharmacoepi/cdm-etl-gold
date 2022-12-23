using org.ohdsi.cdm.framework.common.Base;
using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Enums;
using org.ohdsi.cdm.framework.desktop.Databases;
using org.ohdsi.cdm.framework.desktop.DbLayer;
using org.ohdsi.cdm.framework.desktop.Enums;
using org.ohdsi.cdm.framework.desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Text;

namespace org.ohdsi.cdm.framework.desktop.Base
{
    public class DatabaseChunkPart : ChunkPart
    {
        public ChunkData ChunkData { get; private set; }

        public DatabaseChunkPart(int chunkId, Func<IPersonBuilder> createPersonBuilder, string prefix, int attempt) : base(chunkId, createPersonBuilder, prefix, attempt)
        {
            ChunkData = new ChunkData(ChunkId, int.Parse(Prefix));
            PersonBuilders = new Dictionary<long, Lazy<IPersonBuilder>>();
            OffsetManager = new KeyMasterOffsetManager(ChunkId, int.Parse(Prefix), 0);
        }

        public void Reset()
        {
            ChunkData = new ChunkData(ChunkId, int.Parse(Prefix));
            PersonBuilders = new Dictionary<long, Lazy<IPersonBuilder>>();
            OffsetManager = new KeyMasterOffsetManager(ChunkId, int.Parse(Prefix), 0);
        }

        public KeyValuePair<string, Exception> Load(IDatabaseEngine sourceEngine, string sourceSchemaName, List<QueryDefinition> sourceQueryDefinitions, OdbcConnection sourceConnection, string vendor)
        {
            var fileName = string.Empty;
            var query = string.Empty;
            var connectionString = string.Empty;

            try
            {
                var timer = new Stopwatch();
                timer.Start();

                foreach (var qd in sourceQueryDefinitions)
                {
                    if (qd.Providers != null) continue;
                    if (qd.Locations != null) continue;
                    if (qd.CareSites != null) continue;

                    fileName = qd.FileName;

                    var sql = GetSqlHelper.GetSql(sourceEngine.Database,
                        qd.GetSql(vendor, sourceSchemaName),
                        sourceSchemaName);

                    if (string.IsNullOrEmpty(sql))
                        continue;

                    var q = string.Format(sql, ChunkId);

                    foreach (var subQuery in q.Split(new[] { "GO" + "\r\n", "GO" + "\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {

                        using (var cdm = sourceEngine.GetCommand(subQuery, sourceConnection))
                        {
                            cdm.CommandTimeout = 0;
                            using (var reader = sourceEngine.ReadChunkData(sourceConnection, cdm, qd, ChunkId, Prefix))
                            {
                                while (reader.Read())
                                {
                                    PopulateData(qd, reader);
                                }
                            }
                        }
                        
                    }
                    
                }

                timer.Stop();
            }
            catch (Exception e)
            {
                var info = new StringBuilder();
                info.AppendLine("SourceEngine=" + sourceEngine);
                info.AppendLine("SourceConnectionString=" + connectionString);
                info.AppendLine("File name=" + fileName);
                info.AppendLine("Query:");
                info.AppendLine(query);

                return new KeyValuePair<string, Exception>(info.ToString(), e);
            }


            return new KeyValuePair<string, Exception>(null, null);
        }




        public void Build()
        {
            Console.WriteLine($"Building CDM chunkId={ChunkId} ...");
            Debug.WriteLine($"Building CDM chunkId={ChunkId} ...");

            foreach (var pb in PersonBuilders)
            {
                var result = pb.Value.Value.Build(ChunkData, OffsetManager);
                ChunkData.AddAttrition(pb.Key, result);
            }

            PersonBuilders.Clear();
            PersonBuilders = null;

            Console.WriteLine($"Building CDM chunkId={ChunkId} - complete");
            Debug.WriteLine($"Building CDM chunkId={ChunkId} - complete");
        }

        public void Save(CdmVersions cdm, string destinationConnectionString, IDatabaseEngine destinationEngine, string sourceSchema, string destinationSchema)
        {
            
            if (ChunkData.Persons.Count == 0)
            {
                ChunkData.Clean();
                return;
            }
            

            var saver = destinationEngine.GetSaver();

            var timer = new Stopwatch();
            timer.Start();
            using (saver)
            {
                saver.Create(destinationConnectionString, cdm, sourceSchema, destinationSchema).Save(ChunkData, OffsetManager);
            }
            timer.Stop();

            Console.WriteLine($"Saving chunkId={ChunkId} - complete");

            ChunkData.Clean();
            GC.Collect();
        }

        public void Clean()
        {
            ChunkData.Clean();
            ChunkData = null;
        }

    }
}
