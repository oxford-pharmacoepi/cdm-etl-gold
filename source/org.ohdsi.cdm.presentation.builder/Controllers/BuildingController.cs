﻿using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Lookups;
using org.ohdsi.cdm.framework.common.Omop;
using org.ohdsi.cdm.framework.desktop.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace org.ohdsi.cdm.presentation.builder.Controllers
{
    public class BuildingController
    {
        #region Variables

        private readonly BuilderController _builderController;

        #endregion

        #region Properties

        public int ChunksCount => Settings.Current.Building.ChunksCount;

        public int CompleteChunksCount => _builderController.CompleteChunksCount;

        public BuilderState CurrentState => _builderController.CurrentState;

        #endregion

        #region Constructor

        public BuildingController()
        {
            _builderController = new BuilderController();
        }

        #endregion

        #region Methods

        public void Process()
        {
            if (_builderController.CurrentState != BuilderState.Stopping && _builderController.CurrentState != BuilderState.Running)
            {
                _builderController.CurrentState = BuilderState.Running;
            }
            else
            {
                Stop();
            }
        }

        public void Stop()
        {
            if (_builderController.CurrentState != BuilderState.Stopped)
                _builderController.CurrentState = BuilderState.Stopping;
        }

        private void TryToStop()
        {
            if (_builderController == null) return;

            _builderController.CurrentState = BuilderState.Stopped;
        }

        public void Refresh()
        {
            if (Settings.Current.Building.BuildingState == null) return;

            if (_builderController.CurrentState == BuilderState.Stopping)
                TryToStop();

            if (_builderController.CurrentState == BuilderState.Running)
            {

                //DataCleaning();
                CreateDestination();

                var vocabulary = new Vocabulary();
                CreateLookup(vocabulary);

                vocabulary.Fill(false, false);
                //Map Patient to Person First before create a chunk
                MapAllPatients(vocabulary);

                //Create chunk
                CreateChunks();

                //Map all death
                MapDeath(vocabulary);

                //Map other cdm 
                bool done = Build(vocabulary);

                if (done)
                {
                    PopulateCdmSource();

                    //Create PKs and idx in Visit and Era Tables
                    CreateCdmPkIdx();

                    //Create FKs in Era tables
                    CreateEraFks();
                }
            }
        }

        private void MapDeath(IVocabulary vocabulary) {
            if (!Settings.Current.Building.BuildingState.ChunksCreated || Settings.Current.Building.BuildingState.MapAllDeathDone) return;

            UpdateDate("MapAllDeathStart");
            _builderController.MapDeath(vocabulary);
            UpdateDate("MapAllDeathEnd");
        }

        private void DataCleaning()
        {
            if (Settings.Current.Building.BuildingState.DataCleaningDone) return;

            UpdateDate("DataCleaningStart");
            _builderController.DataCleaning();
            UpdateDate("DataCleaningEnd");
        }

        private void MapAllPatients(IVocabulary vocabulary) {

            if (!Settings.Current.Building.BuildingState.LookupCreated || Settings.Current.Building.BuildingState.MapAllPatientsDone) return;

            UpdateDate("MapAllPatientsStart");
            _builderController.MapAllPatients(vocabulary);
            UpdateDate("MapAllPatientsEnd");

        }

        private void CreateChunks()
        {
            if (!Settings.Current.Building.BuildingState.MapAllPatientsDone || Settings.Current.Building.BuildingState.ChunksCreated) return;

            UpdateDate("CreateChunksStart");
            _builderController.CreatChunks();
            UpdateDate("CreateChunksEnd");
        }

        private void CreateDestination()
        {
            if (Settings.Current.Building.BuildingState.DestinationCreated) return;

            UpdateDate("CreateDestinationDbStart");
            _builderController.CreateDestination();
            UpdateDate("CreateDestinationDbEnd");
        }

        private void PopulateCdmSource()
        {

            if (Settings.Current.Building.BuildingState.PopulateCdmSourceDone) return;
            UpdateDate("PopulateCdmSourceStart");
            _builderController.PopulateCdmSource ();
            UpdateDate("PopulateCdmSourceEnd");

        }

        private void CreateCdmPkIdx() {

            if (Settings.Current.Building.BuildingState.CdmPkIdxForVisitEraCreated) return;

            UpdateDate("CdmPkIdxForVisitEraStart");
            _builderController.CreateCdmPkIdxForVisitEra();
            UpdateDate("CdmPkIdxForVisitEraEnd");

        }

        private void CreateEraFks() {

            if (Settings.Current.Building.BuildingState.CdmEraFksDone) return;

            UpdateDate("CdmFksForEraStart");
            _builderController.CreateCdmFksForEra();
            UpdateDate("CdmFksForEraEnd");
        }

        private void CreateLookup(IVocabulary vocabulary)
        {

            if (!Settings.Current.Building.BuildingState.DestinationCreated || Settings.Current.Building.BuildingState.LookupCreated) return;

            UpdateDate("CreateLookupStart");
            _builderController.CreateLookup(vocabulary);
            UpdateDate("CreateLookupEnd");
        }


        private bool Build(IVocabulary vocabulary)
        {
            while (!Settings.Current.Building.BuildingState.LookupCreated || !Settings.Current.Building.BuildingState.MapAllDeathDone)
            {
                if (_builderController.CurrentState == BuilderState.Error)
                    return false;

                Thread.Sleep(1000);
            }

            var allChunksComplete = false;

            if (Settings.Current.Building.BuildingState.BuildingComplete) return true;
            if (_builderController.CurrentState == BuilderState.Error) return false;


            if (Settings.Current.Building.BuildingState.LookupCreated)
            {
                if (!Settings.Current.Building.BuildingState.BuildingStarted)
                {
                    UpdateDate("BuildingStart");
                }

                _builderController.Build(vocabulary);
                
                if (_builderController.CurrentState != BuilderState.Error && _builderController.CompleteChunksCount == Settings.Current.Building.ChunksCount)
                {
                    allChunksComplete = true;
                    UpdateDate("BuildingEnd");
                }
                
                
            }
            return allChunksComplete;
        }

        public void FillPostBuildTables()
        {
        }

        private DateTime? UpdateDate(string fieldName)
        {
            if (_builderController.CurrentState == BuilderState.Error) return null;

            var time = DateTime.Now;
            typeof(Building).GetProperty(fieldName).SetValue(Settings.Current.Building.BuildingState, time, null);
            Settings.Current.Save(false);
            return time;
        }

        private void UpdateDate(string fieldName, DateTime? time)
        {
            if (time.HasValue && time.Value.Year == DateTime.MaxValue.Year)
            {
                var currentValue = typeof(Building).GetProperty(fieldName).GetValue(Settings.Current.Building.BuildingState, null);
                if (currentValue is DateTime)
                {
                    if (((DateTime)currentValue).Year == DateTime.MaxValue.Year)
                    {
                        time = null;
                    }
                }
            }

            typeof(Building).GetProperty(fieldName).SetValue(Settings.Current.Building.BuildingState, time, null);

            Settings.Current.Save(true);
        }

        public IEnumerable<string> GetErrors()
        {
            return Logger.GetErrors();
        }

        public void DataCleaningStep()
        {
            _builderController.DataCleaning();
            UpdateDate("DataCleaningStart", DateTime.Now);
            UpdateDate("DataCleaningEnd", DateTime.Now);
        }

        public void ResetDbCreationStep()
        {
            _builderController.DropDestination();
            UpdateDate("CreateDestinationDbStart", null);
            UpdateDate("CreateDestinationDbEnd", null);
        }

        public void CreateTablesStep()
        {
            _builderController.CreateTablesStep();
            UpdateDate("CreateDestinationDbStart", DateTime.Now);
            UpdateDate("CreateDestinationDbEnd", DateTime.Now);
        }

        public void MapAllPatientsStep(IVocabulary vocabulary) {
            _builderController.MapAllPatients(vocabulary);
            UpdateDate("MapAllPatientStart", DateTime.Now);
            UpdateDate("MapAllPatientEnd", DateTime.Now);
        }

        public void CreateChunksStep() {
            _builderController.CreatChunks();
            UpdateDate("CreateChunksStart", DateTime.Now);
            UpdateDate("CreateChunksEnd", DateTime.Now);
        }

        public void MapAllDeathStep(IVocabulary vocabulary)
        {
            _builderController.MapDeath(vocabulary);
            UpdateDate("MapAllDeathStart", DateTime.Now);
            UpdateDate("MapAllDeathEnd", DateTime.Now);
        }

        public void SkipDbCreationStep()
        {
            UpdateDate("CreateDestinationDbStart", DateTime.MaxValue);
            UpdateDate("CreateDestinationDbEnd", DateTime.MaxValue);
        }

        public void SkipChunksCreationStep()
        {
            UpdateDate("CreateChunksStart", DateTime.MaxValue);
            UpdateDate("CreateChunksEnd", DateTime.MaxValue);
        }

        public void SkipDataCleaningStep()
        {
            UpdateDate("DataCleaningStart", DateTime.MaxValue);
            UpdateDate("DataCleaningEnd", DateTime.MaxValue);
        }

        public void SkipMapAllPatientsStep() {
            UpdateDate("MapAllPatientsStart", DateTime.MaxValue);
            UpdateDate("MapAllPatientsEnd", DateTime.MaxValue);
        }

        public void SkipCreateCdmIndexesStep()
        {
            UpdateDate("CreateCdmIndexesStart", DateTime.MaxValue);
            UpdateDate("CreateCdmIndexesEnd", DateTime.MaxValue);
        }

        public void RestartChunksCreationStep()
        {
            //CreateChunks(true);
        }

        public void ResetChunksCreationStep()
        {
            UpdateDate("CreateChunksStart", null);
            UpdateDate("CreateChunksEnd", null);
        }

        public void ResetLookupCreationStep(bool onlyDataUpdate)
        {
            if (!onlyDataUpdate)
                _builderController.TruncateLookup();

            UpdateDate("CreateLookupStart", null);
            UpdateDate("CreateLookupEnd", null);
        }

        public void SkipLookupCreationStep()
        {
            UpdateDate("CreateLookupStart", DateTime.MaxValue);
            UpdateDate("CreateLookupEnd", DateTime.MaxValue);
        }

        public void ResetBuildingStep(bool onlyDataUpdate)
        {
            if (!onlyDataUpdate)
            {
                _builderController.TruncateWithoutLookupTables();
            }

            UpdateDate("BuildingStart", null);
            UpdateDate("BuildingEnd", null);
            Settings.Current.Building.CompletedChunkIds = new List<int>();
            Settings.Current.Building.ChunksCount = 0;
        }

        public void SkipBuildingStep()
        {
            UpdateDate("BuildingStart", DateTime.MaxValue);
            UpdateDate("BuildingEnd", DateTime.MaxValue);
        }

        public void ResetVocabularyStep(bool onlyDataUpdate)
        {
            if (!onlyDataUpdate)
                _builderController.ResetVocabularyStep();

            UpdateDate("CopyVocabularyStart", null);
            UpdateDate("CopyVocabularyEnd", null);
        }

        public void SkipVocabularyStep()
        {
            UpdateDate("CopyVocabularyStart", DateTime.MaxValue);
            UpdateDate("CopyVocabularyEnd", DateTime.MaxValue);
        }

        public void SkipIndexesStep()
        {
            UpdateDate("CreateIndexesStart", DateTime.MaxValue);
            UpdateDate("CreateIndexesEnd", DateTime.MaxValue);
        }

        public void ResetPostprocessStep()
        {
            UpdateDate("PostprocessStart", null);
            UpdateDate("PostprocessEnd", null);
        }

        public void SkipPostprocessStep()
        {
            UpdateDate("PostprocessStart", DateTime.MaxValue);
            UpdateDate("PostprocessEnd", DateTime.MaxValue);
        }

        public void TruncateTables()
        {
            _builderController.TruncateTables();
        }

        public void TruncateWithoutLookupTables()
        {
            _builderController.TruncateWithoutLookupTables();
        }

        public void ResetSettings()
        {
            Settings.Current.Building.Reset();
            Settings.Current.Building.Save(true);
        }

        public void ResetErrors()
        {
            Logger.ResetErrors();
        }

        #endregion
    }
}
