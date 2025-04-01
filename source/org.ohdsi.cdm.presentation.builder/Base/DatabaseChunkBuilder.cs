using org.ohdsi.cdm.framework.common.Base;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Omop;
using org.ohdsi.cdm.framework.desktop.Base;
using org.ohdsi.cdm.framework.desktop.Databases;
using org.ohdsi.cdm.framework.desktop.DbLayer;
using org.ohdsi.cdm.framework.desktop.Enums;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Windows.Controls;

namespace org.ohdsi.cdm.presentation.builder.Base
{
    public class DatabaseChunkBuilder
    {
        #region Variables

        private readonly int _chunkId;
        private readonly int _chunkSize;
        private readonly Func<IPersonBuilder> _createPersonBuilder;
        #endregion

        #region Constructors

        public DatabaseChunkBuilder(int chunkId, Func<IPersonBuilder> createPersonBuilder, int chunkSize)
        //public DatabaseChunkBuilder(int chunkId, Func<IPersonBuilder> createPersonBuilder)
        {
            _chunkId = chunkId;
            _chunkSize = chunkSize;
            _createPersonBuilder = createPersonBuilder;
        }
        #endregion

        #region Methods
        //public DatabaseChunkPart Process(IDatabaseEngine sourceEngine, string sourceSchemaName, List<QueryDefinition> sourceQueryDefinitions, OdbcConnection sourceConnection, string vendor)
        public DatabaseChunkPart Process(IDatabaseEngine sourceEngine, string sourceSchemaName, string destinationSchemaName, List<QueryDefinition> sourceQueryDefinitions, OdbcConnection sourceConnection, string vendor)
        {
            try
            {
                Debug.WriteLine("_chunkId=" + _chunkId);

                var part = new DatabaseChunkPart(_chunkId, _createPersonBuilder, "0", 0, _chunkSize);
                //var part = new DatabaseChunkPart(_chunkId, _createPersonBuilder, "0", 0);

                var timer = new Stopwatch();
                timer.Start();

                part.loadPersonObservationPeriodByChunk(sourceConnection, sourceSchemaName, destinationSchemaName, _chunkId);

                var result = part.Load(sourceEngine, sourceSchemaName, sourceQueryDefinitions, sourceConnection, vendor);



                if (result.Value != null)
                {
                    try
                    {
                        Logger.Write(_chunkId, LogMessageTypes.Info, result.Key);
                    }
                    catch (System.InvalidOperationException)
                    {
                        Debug.WriteLine($"Fail to write log: {result.Key}");
                    }

                    throw result.Value;
                }

                try
                {
                    Logger.Write(_chunkId, LogMessageTypes.Info,
                       $"ChunkId={_chunkId} was loaded - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins | {GC.GetTotalMemory(false) / 1024f / 1024f} Mb");
                }
                catch (InvalidOperationException) {
                    Debug.WriteLine("Fail to write log");
                }

                part.Build(Settings.Current.WithinTheObservationPeriod);

                try
                {
                    Logger.Write(_chunkId, LogMessageTypes.Info,
                       $"ChunkId={_chunkId} was built - {timer.ElapsedMilliseconds * 0.000016666666666666667:0.00} mins | {GC.GetTotalMemory(false) / 1024f / 1024f} Mb");
                }
                catch (InvalidOperationException) {
                    Debug.WriteLine("Fail to write log");
                }

                return part;
                }

            catch (Exception e)
            {
                Logger.WriteError(_chunkId, e);

                throw;
            }
        }
        #endregion
    }
}
