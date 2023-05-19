using org.ohdsi.cdm.framework.common.Base;
using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Enums;
using org.ohdsi.cdm.framework.common.Omop;
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

        public DatabaseChunkPart(int chunkId, Func<IPersonBuilder> createPersonBuilder, string prefix, int attempt, int chunkSize) : base(chunkId, createPersonBuilder, prefix, attempt)
        //public DatabaseChunkPart(int chunkId, Func<IPersonBuilder> createPersonBuilder, string prefix, int attempt) : base(chunkId, createPersonBuilder, prefix, attempt)
        {
            ChunkData = new ChunkData(ChunkId, int.Parse(Prefix), chunkSize);
            //ChunkData = new ChunkData(ChunkId, int.Parse(Prefix));
            PersonBuilders = new Dictionary<long, Lazy<IPersonBuilder>>();
            OffsetManager = new KeyMasterOffsetManager(ChunkId, int.Parse(Prefix), 0);
        }

        public void Reset()
        {
            ChunkData = new ChunkData(ChunkId, int.Parse(Prefix), 0);
            //ChunkData = new ChunkData(ChunkId, int.Parse(Prefix));
            PersonBuilders = new Dictionary<long, Lazy<IPersonBuilder>>();
            OffsetManager = new KeyMasterOffsetManager(ChunkId, int.Parse(Prefix), 0);
        }

        public void loadPersonObservationPeriodByChunk(OdbcConnection sourceConnection, string sourceSchemaName, string destinationSchemaName, int chunkId) { 


        var sql = $"With ch as (" +
                $"select person_id from {sourceSchemaName}.chunk_person ch " +
                $"where chunk_id = {chunkId}) " +
            $"select p.person_id as PersonId, " +
            $"op.observation_period_start_date as StartDate," +
            $"op.observation_period_end_date as EndDate," +
            $"p.person_source_value as PersonSourceValue," +
            $"p.gender_source_value as GenderSourceValue," +
            $"p.gender_concept_id as GenderConceptId," +
            $"p.year_of_birth as YearOfBirth," +
            $"p.month_of_birth as MonthOfBirth," +
            $"p.race_source_value as RaceSourceValue," +
            $"p.race_concept_id as RaceConceptId," +
            $"op.observation_period_id," +
            $"op.period_type_concept_id as TypeConceptId " + 
            $"from ch " +
            $"join {destinationSchemaName}.person p on p.person_id = ch.person_id " +
            $"join {destinationSchemaName}.observation_period op on op.person_id = p.person_id";

            Debug.WriteLine($"sql={sql}");

            using var command = new OdbcCommand(sql, sourceConnection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ChunkData.Persons.Add(
                            new Person
                            {
                                ObservationPeriodGap = 32,
                                //AdditionalFields = additionalFields,
                                PersonId = (long)reader["PersonId"],
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                PersonSourceValue = reader["PersonSourceValue"].ToString(),
                                GenderSourceValue = reader["GenderSourceValue"].ToString(),
                                GenderConceptId = Convert.ToInt32(reader["GenderConceptId"]),
                                //LocationId = reader.GetInt32(8),
                                YearOfBirth = Convert.ToInt32(reader["YearOfBirth"]),
                                //MonthOfBirth = reader["MonthOfBirth"] == DBNull.Value ? null : Convert.ToInt32(reader["MonthOfBirth"],
                                //DayOfBirth = (int)reader.GetInt32(4),
                                //LocationSourceValue = locationSourceValue,
                                //CareSiteId = (int)reader.GetInt32(10),
                                //EthnicitySourceValue = reader.GetString(15),
                                //EthnicityConceptId = (int)reader.GetInt32(7),
                                RaceSourceValue = reader["RaceSourceValue"].ToString(),
                                RaceConceptId = Convert.ToInt32(reader["RaceConceptId"])
                                //ProviderId = (int)reader.GetInt32(9),
                                //GenderSourceConceptId = (int)reader.GetInt32(13), // CCAE
                                //RaceSourceConceptId = (int)reader.GetInt32(15),
                                //EthnicitySourceConceptId = (int)reader.GetInt32(17)
                            }
                );

                ChunkData.ObservationPeriods.Add(
                            new ObservationPeriod
                            {
                                Id = Convert.ToInt32(reader["observation_period_id"]),
                                PersonId = (long)reader["PersonId"],
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                TypeConceptId = Convert.ToInt32(reader["TypeConceptId"])
                            }
                );
            }
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
                    if (qd.Persons   != null) continue;
                    if (qd.Death     != null) continue;

                    fileName = qd.FileName;

                    var sql = GetSqlHelper.GetSql(sourceEngine.Database,
                        qd.GetSql(vendor, sourceSchemaName),
                        sourceSchemaName);

                    if (string.IsNullOrEmpty(sql))
                        continue;

                    var q = string.Format(sql, ChunkId);

                    foreach (var subQuery in q.Split(new[] { "GO" + "\r\n", "GO" + "\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Debug.WriteLine("subQuery=" + subQuery);

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

        public void Build(bool withinTheObservationPeriod)
        {
            Console.WriteLine($"Building CDM chunkId={ChunkId} ...");
            Debug.WriteLine($"Building CDM chunkId={ChunkId} ...");

            Debug.WriteLine($"withinTheObservationPeriod={withinTheObservationPeriod} ...");

            foreach (var pb in PersonBuilders)
            {
                var result = pb.Value.Value.BuildCdm(ChunkData, OffsetManager, pb.Key, withinTheObservationPeriod);
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
