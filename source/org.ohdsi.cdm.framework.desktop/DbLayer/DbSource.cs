using Microsoft.VisualBasic;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Extensions;
using org.ohdsi.cdm.framework.common.Omop;
using org.ohdsi.cdm.framework.desktop.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using Thrift.Protocol;

namespace org.ohdsi.cdm.framework.desktop.DbLayer
{
    public class DbSource
    {
        private readonly string _connectionString;
        private readonly string _folder;
        private readonly string _schemaName;
        private readonly int _chunkSize;
        private readonly string _destinationSchemaName;

        public DbSource(string connectionString, string folder, string schemaName, int chuckSize, string destinationSchemaName)
        {
            _connectionString = connectionString;
            _folder = folder;
            _schemaName = schemaName;
            _chunkSize = chuckSize;
            _destinationSchemaName = destinationSchemaName;
        }

        public int CreateChunkTable()
        {
            int chunkCount = 0;
            DropChunkTable();
            var query = File.ReadAllText(Path.Combine(_folder, "CreateChunkTable.sql"));
            query = query.Replace("{sc}", _schemaName);
            query = query.Replace("{CHUNK_SIZE}", _chunkSize.ToString());
            query = query.Replace("{des}", _destinationSchemaName);
            //query = query.Replace("{TARGET_SCHEMA}", _destinationSchemaName);

            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString)) {

                foreach (var subQuery in query.Split(new[] { ";" },
                   StringSplitOptions.RemoveEmptyEntries))
                {
                    using (var command = new OdbcCommand(subQuery, connection))
                    {
                        Debug.WriteLine($"subQuery={subQuery}");

                        command.CommandTimeout = 0;
                        int c = command.ExecuteNonQuery();

                        chunkCount = c;

                        Debug.WriteLine("c=" + c);          //the last sql return the total number of chunk
                    }
                }
            }

            Debug.WriteLine("chunkCount=" + chunkCount);

            return chunkCount;
        }

        public void DropChunkTable()
        {
            var query = File.ReadAllText(Path.Combine(_folder, "DropChunkTable.sql"));
            query = query.Replace("{sc}", _schemaName);
            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            using (var cmd = new OdbcCommand(query, connection) { CommandTimeout = 0 })
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateIndexesChunkTable()
        {
            var query = File.ReadAllText(Path.Combine(_folder, "CreateIndexesChunkTable.sql"));
            query = query.Replace("{sc}", _schemaName);
            
            if (string.IsNullOrEmpty(query.Trim())) return;

            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            {
                foreach (var subQuery in query.Split(new[] { "\r\nGO", "\nGO", ";" },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    using (var command = new OdbcCommand(subQuery, connection))
                    {
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        public IEnumerable<IDataReader> GetPersonKeys(string batchScript, long batches, int batchSize)
        {
            batchScript = batchScript.Replace("{sc}", _schemaName);

            Debug.WriteLine("batches=" + batches);

            var sql = batches > 0
                ? string.Format(batchScript, "TOP " + batches * batchSize)
                : string.Format(batchScript, "");

            Debug.WriteLine("string.Format(batchScript, \"TOP \" + batches * batchSize)=" + string.Format(batchScript, "TOP " + batches * batchSize));
            Debug.WriteLine("sql=" + sql);
            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            using (var c = new OdbcCommand(sql, connection) { CommandTimeout = 0 })
            {
                using (var reader = c.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }

        }

        public string GetSourceReleaseDate()
        {
            try
            {
                string query = "SELECT VERSION_DATE VERSION_DATE FROM " + _schemaName + "._Version";
                using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
                using (var c = new OdbcCommand(query, connection) { CommandTimeout = 0 })
                {
                    using (var reader = c.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var dateString = reader.GetString("VERSION_DATE");
                            var date = DateTime.Parse(dateString);

                            return date.ToShortDateString();
                        }
                    }
                }

            }
            catch (Exception)
            {
                return DateTime.MinValue.ToShortDateString();
            }

            return DateTime.MinValue.ToShortDateString();
        }


        public List<int> GetIncompleteChunkId()
        {
            List<int> list = new List<int>();

            var sql = $"select chunk_id from {_schemaName}.chunk where completed = 0";

            using var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString);
            using var command = new OdbcCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }

            return list;
        }

        public double GetStaffCount()
        {
            int i = 0;

            var sql = "select count(1) from {sc}.staff";
            sql = sql.Replace("{sc}", _schemaName);

            using var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString);
            using var command = new OdbcCommand(sql, connection);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                i = reader.GetInt32(0);
            }

            return (double)i;
        }

        public double GetPersonCount()
        {
            int i = 0;

            var sql = "select count(1) from {sc}.Person";
            sql = sql.Replace("{sc}", _destinationSchemaName);

            using var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString);
            using var command = new OdbcCommand(sql, connection);
            command.CommandTimeout = 0;
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                i = reader.GetInt32(0);
            }

            return (double)i;
        }

        public int GetVisitOccurenceIdSeq()
        {
            int i = 0;

            var sql = $"select nextval('{_schemaName}.visit_detail_id_seq')";

            using var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString);
            using var command = new OdbcCommand(sql, connection);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                i = reader.GetInt32(0);
            }

            connection.Close();
            return i;
        }

        public void ExecuteQuery(string query)
        {
            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            {
                query = query.Replace("{sc}", _destinationSchemaName);

                foreach (var subQuery in query.Split(new[] { "\r\nGO", "\nGO", ";" }, StringSplitOptions.None))
                {
                    if (string.IsNullOrEmpty(subQuery))
                        continue;

                    using (var command = new OdbcCommand(subQuery, connection))
                    {
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

    }

}
