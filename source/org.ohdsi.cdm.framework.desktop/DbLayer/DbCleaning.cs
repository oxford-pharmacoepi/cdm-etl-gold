using MySqlConnector.Logging;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.desktop.Helpers;
using Parquet.Data.Rows;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Transactions;

namespace org.ohdsi.cdm.framework.desktop.DbLayer
{
    public class DbCleaning
    {
        private readonly string _connectionString;
        private readonly string _sourceSchemaName;
        private readonly string _schemaName;

        public DbCleaning(string connectionString, string schemaName)
        {
            _connectionString = connectionString;
            _sourceSchemaName = schemaName;
            _schemaName = schemaName + "_nok";
        }

        public void CreateProcedure(string query)
        {
            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            {
                query = query.Replace("{sc}", _sourceSchemaName);
                query = query.Replace("{SOURCE_NOK_SCHEMA}", _schemaName);


                foreach (var subQuery in query.Split(new[] { "\r\nGO", "\nGO" }, StringSplitOptions.None))
                {
                    Debug.WriteLine("query=" + subQuery);

                    
                    using (var command = new OdbcCommand(subQuery, connection))
                    {
                        command.CommandTimeout = 30000;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void DataCleaning(string tableName) {

                //calling store proc is a single transaction

                string query = "CALL pr_DataCleaning('{tableName}')".Replace("{tableName}", tableName);
                Debug.WriteLine(query);

                using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
                {
                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.CommandTimeout = 0;         //A value of 0 indicates no limit (an attempt to execute a command will wait indefinitely)                              
                        command.ExecuteNonQuery();
                    }
                }
 
        }


        public void ExecuteQuery(string query) {

            query = query.Replace("{sc}", _sourceSchemaName);
            query = query.Replace("{SOURCE_NOK_SCHEMA}", _schemaName);

            Debug.WriteLine("query=" + query);

            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            {
                OdbcTransaction transaction = null;

                try
                {
                    transaction = connection.BeginTransaction();

                    foreach (var subQuery in query.Split(new[] { "\r\nGO", "\nGO", ";" }, StringSplitOptions.None))
                    {
                        if (string.IsNullOrEmpty(subQuery))
                            continue;

                        using (var command = new OdbcCommand(subQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.CommandTimeout = 0;         //A value of 0 indicates no limit (an attempt to execute a command will wait indefinitely)
                            command.ExecuteNonQuery();
                        }

                    }

                    transaction.Commit();
                    Debug.WriteLine("Tranction Committed");


                }
                catch (Exception e)
                {

                    transaction.Rollback();
                    Debug.WriteLine("Transaction Rollback");
                    Debug.WriteLine(e);
                    throw e;

                }
            }

        }

    }
}


