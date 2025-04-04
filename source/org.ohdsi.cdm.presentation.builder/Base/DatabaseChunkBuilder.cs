﻿using org.ohdsi.cdm.framework.common.Base;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Omop;
using org.ohdsi.cdm.framework.desktop.Base;
using org.ohdsi.cdm.framework.desktop.Databases;
using org.ohdsi.cdm.framework.desktop.DbLayer;
using org.ohdsi.cdm.framework.desktop.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Threading;
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
        public DatabaseChunkPart Process(IDatabaseEngine sourceEngine, string sourceSchemaName, string destinationSchemaName, List<QueryDefinition> sourceQueryDefinitions, OdbcConnection sourceConnection, string vendor, Stopwatch timer)
        {
            try
            {
                Debug.WriteLine("_chunkId=" + _chunkId);

                var part = new DatabaseChunkPart(_chunkId, _createPersonBuilder, "0", 0, _chunkSize);
                //var part = new DatabaseChunkPart(_chunkId, _createPersonBuilder, "0", 0);

                part.loadPersonObservationPeriodByChunk(sourceConnection, sourceSchemaName, destinationSchemaName, _chunkId);

                var result = part.Load(sourceEngine, sourceSchemaName, sourceQueryDefinitions, sourceConnection, vendor);

                timer.Stop();

                TimeSpan elapsed = timer.Elapsed;

                if (result.Value != null)
                {
                    Logger.Write(_chunkId, LogMessageTypes.Info, result.Key);
                    throw result.Value;
                }

                Logger.Write(_chunkId, LogMessageTypes.Info,
                       $"ChunkId={_chunkId} was loaded -  {elapsed.Minutes} min {elapsed.Seconds} sec | {GC.GetTotalMemory(false) / 1024f / 1024f} Mb");

                timer.Restart();
                part.Build(Settings.Current.WithinTheObservationPeriod);
                
                timer.Stop();
                elapsed = timer.Elapsed;

                Logger.Write(_chunkId, LogMessageTypes.Info,
                       $"ChunkId={_chunkId} was built - {elapsed.Minutes} min {elapsed.Seconds} sec | {GC.GetTotalMemory(false) / 1024f / 1024f} Mb");


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
