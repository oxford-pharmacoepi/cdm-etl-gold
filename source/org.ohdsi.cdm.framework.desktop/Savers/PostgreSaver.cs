﻿using MySqlConnector;
using Npgsql;
using NpgsqlTypes;
using org.ohdsi.cdm.framework.common.Enums;
using org.ohdsi.cdm.framework.desktop.Enums;
using org.ohdsi.cdm.framework.desktop.Helpers;
using System;
using System.Data.Odbc;
using System.Diagnostics;
using System.Transactions;

namespace org.ohdsi.cdm.framework.desktop.Savers
{
    public class PostgreSaver : Saver
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public override ISaver Create(string connectionString, CdmVersions cdmVersion, string sourceSchema, string destinationSchema)
        {
            CdmVersion = cdmVersion;
            SourceSchema = sourceSchema;
            DestinationSchema = destinationSchema;

            var odbc = new OdbcConnectionStringBuilder(connectionString);

            //var connectionStringTemplate = "Server={server};Port=5432;Database={database};User Id={username};Password={password};SslMode=Require;Trust Server Certificate=true";
            //There is no way to set no connection Timeout.
            //Timeout=300 (5 mins) for insert data is enough at the moment even though the database is slow
            var connectionStringTemplate = "Server={server};Port=5432;Database={database};User Id={username};Password={password};CommandTimeout=0;Keepalive=300;Timeout=600;";

            var npgsqlConnectionString = connectionStringTemplate.Replace("{server}", odbc["server"].ToString())
                .Replace("{database}", odbc["database"].ToString()).Replace("{username}", odbc["uid"].ToString())
                .Replace("{password}", odbc["pwd"].ToString());

            _connection = SqlConnectionHelper.OpenNpgsqlConnection(npgsqlConnectionString);
            _transaction = _connection.BeginTransaction();

            return this;
        }

        public override Database GetDatabaseType()
        {
            return Database.Postgre;
        }

        public override void Write(int? chunkId, int? subChunkId, System.Data.IDataReader reader, string tableName)
        {
            if (reader == null)
                return;

            if (tableName.ToLower().StartsWith("_chunks"))
            {
                tableName = SourceSchema + "." + tableName;
            }
            else
            {
                tableName = DestinationSchema + "." + tableName;
            }

            var fields = new string[reader.FieldCount];
            for (var i = 0; i < reader.FieldCount; i++)
            {
                fields[i] = reader.GetName(i);
            }

            var q = $"COPY {tableName} ({string.Join(",", fields)}) from STDIN (FORMAT BINARY)";

            Debug.WriteLine("q=" + q);

            using (var inStream = _connection.BeginBinaryImport(q))
            {
                while (reader.Read())
                {
                    inStream.StartRow();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i);

                        if (value is null)
                        {
                            inStream.WriteNull();
                        }
                        else
                        {
                            var fd = GetFieldType(reader.GetFieldType(i), reader.GetName(i));
                            inStream.Write(value, fd);
                        }
                    }
                }
                inStream.Complete();
            }
        }

        public override void UpdateChunkStatus(int? chunkId) {
            //Update Chunk completed = 1
            var sql = $" UPDATE {SourceSchema}.chunk SET completed=1 WHERE chunk_id={chunkId}";
            Debug.WriteLine("UpdateCompletedChunk sql=" + sql);

            var command = new NpgsqlCommand(sql, _connection);
            command.ExecuteNonQuery();

        }


        private static NpgsqlDbType GetFieldType(Type type, string fieldName)
        {
            var name = type.Name;
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                name = underlyingType.Name;
            }

            switch (name)
            {
                case "Int64":
                    return NpgsqlDbType.Bigint;
                case "Int32":
                    return NpgsqlDbType.Integer;
                case "Decimal":
                    return NpgsqlDbType.Numeric;
                case "DateTime":
                    {
                        if (fieldName.ToLower().Contains("time"))
                            return NpgsqlDbType.Timestamp;

                        return NpgsqlDbType.Date;
                    }
                case "TimeSpan":
                    return NpgsqlDbType.Time;
                case "String":
                    {
                        // workaround for 5.3 schema, string used for Timestamp
                        if (fieldName.ToLower().Contains("time"))
                            return NpgsqlDbType.Time;


                        return NpgsqlDbType.Varchar;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public override void Commit()
        {
            _transaction.Commit();
        }

        public override void Rollback()
        {
            _transaction.Rollback();
        }

        public override void Dispose()
        {
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
