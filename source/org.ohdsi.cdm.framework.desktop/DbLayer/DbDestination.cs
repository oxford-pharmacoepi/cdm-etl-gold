using MySqlConnector.Logging;
using org.ohdsi.cdm.framework.desktop.Helpers;
using System;
using System.Data.Common;
using System.Data.Odbc;
using System.Diagnostics;
using System.Threading;
using System.Transactions;

namespace org.ohdsi.cdm.framework.desktop.DbLayer
{
    public class DbDestination
    {
        private readonly string _connectionString;
        private readonly string _schemaName;

        public DbDestination(string connectionString, string schemaName)
        {
            _connectionString = connectionString;
            _schemaName = schemaName;
        }

        public void CreateIndexes(string query)
        {
            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            {
                OdbcTransaction transaction = null;

                try
                {
                    query = query.Replace("{sc}", _schemaName);

                    transaction = connection.BeginTransaction();

                    foreach (var subQuery in query.Split(new[] { "\r\nGO", "\nGO", ";" }, StringSplitOptions.None))
                    {
                        if (string.IsNullOrEmpty(subQuery))
                            continue;

                        using (var command = new OdbcCommand(subQuery, connection))
                        {
                            command.CommandTimeout = 30000;
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
                    throw e;

                }
            }

        }

        public void CreateDatabase(string query)
        {
            var sqlConnectionStringBuilder = new OdbcConnectionStringBuilder(_connectionString);
            var database = sqlConnectionStringBuilder["database"];


            // TMP
            var mySql = _connectionString.ToLower().Contains("mysql");

            if (mySql)
                sqlConnectionStringBuilder["database"] = "mysql";
            else if (_connectionString.ToLower().Contains("amazon redshift"))
                sqlConnectionStringBuilder["database"] = "poc";
            else if (_connectionString.ToLower().Contains("postgres"))
                sqlConnectionStringBuilder["database"] = "postgres";
            else
                sqlConnectionStringBuilder["database"] = "master";

            using (var connection = SqlConnectionHelper.OpenOdbcConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                
                if (_connectionString.ToLower().Contains("postgres")) {

                    if (CheckIfDatabaseExistInPostgres(connection, database))
                    {
                        Console.WriteLine(database + " has already exists");
                    }
                    else {
                        query = string.Format(query, database);

                        foreach (var subQuery in query.Split(new[] { "\r\nGO", "\nGO" }, StringSplitOptions.None))
                        {
                            using (var command = new OdbcCommand(subQuery, connection))
                            {
                                command.CommandTimeout = 30000;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                
            }

            //TO_DO: Create cleaning scheme 




            if (!mySql && _schemaName.ToLower().Trim() != "dbo")
            {
                CreateSchema();
            }

        }


        public Boolean CheckIfDatabaseExistInPostgres(OdbcConnection connection, object databaseName)
        {
            int isExist = 0;

            var query = "select exists(SELECT datname FROM pg_database WHERE lower(datname) = lower('" + databaseName + "'))";

            using (var command = new OdbcCommand(query, connection))
            {
                command.CommandTimeout = 30000;
                isExist = Convert.ToInt32(command.ExecuteScalar());
            }

           Debug.WriteLine("isExist=" + (isExist > 0));
            
            return isExist>0;


        }



        public void CreateSchema()
        {
            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            {
                var query = "";

                if (_connectionString.ToLower().Contains("postgres"))
                    query = $"CREATE SCHEMA IF NOT EXISTS {_schemaName}";
                else
                {
                    query = $"create schema [{_schemaName}]";
                }

                using (var command = new OdbcCommand(query, connection))
                {
                    command.CommandTimeout = 0;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ExecuteQuery(string query)
        {
            using (var connection = SqlConnectionHelper.OpenOdbcConnection(_connectionString))
            {
                query = query.Replace("{sc}", _schemaName);

                foreach (var subQuery in query.Split(new[] { "\r\nGO", "\nGO", ";" }, StringSplitOptions.None))
                {
                    if (string.IsNullOrEmpty(subQuery))
                        continue;

                    using (var command = new OdbcCommand(subQuery, connection))
                    {
                        command.CommandTimeout = 30000;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}
