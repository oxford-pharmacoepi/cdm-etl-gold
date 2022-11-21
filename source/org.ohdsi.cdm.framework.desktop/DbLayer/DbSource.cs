﻿using org.ohdsi.cdm.framework.common.Extensions;
using org.ohdsi.cdm.framework.desktop.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;

namespace org.ohdsi.cdm.framework.desktop.DbLayer
{
    public class DbSource
    {
        private readonly string _connectionString;
        private readonly string _folder;
        private readonly string _schemaName;

        public DbSource(string connectionString, string folder, string schemaName)
        {
            _connectionString = connectionString;
            _folder = folder;
            _schemaName = schemaName;
        }

        public void CreateChunkTable()
        {
            DropChunkTable();
            var query = File.ReadAllText(Path.Combine(_folder, "CreateChunkTable.sql"));
            query = query.Replace("{sc}", _schemaName);
            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            using (var cmd = new OdbcCommand(query, connection) { CommandTimeout = 0 })
            {
                cmd.ExecuteNonQuery();
            }
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
                foreach (var subQuery in query.Split(new[] { "GO" + "\r\n", "GO" + "\n" },
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
    }
}
