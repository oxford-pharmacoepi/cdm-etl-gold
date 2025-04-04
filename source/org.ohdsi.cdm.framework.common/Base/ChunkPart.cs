﻿using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Enums;
using org.ohdsi.cdm.framework.common.Extensions;
using org.ohdsi.cdm.framework.common.Omop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace org.ohdsi.cdm.framework.common.Base
{
    public class ChunkPart : IDisposable
    {
        public Dictionary<long, Lazy<IPersonBuilder>> PersonBuilders;

        protected readonly int ChunkId;
        protected readonly Func<IPersonBuilder> CreatePersonBuilder;
        protected readonly string Prefix;
        protected readonly int Attempt;
        protected KeyMasterOffsetManager OffsetManager;
        protected long? LastSavedPersonId;

        public ChunkPart(int chunkId, Func<IPersonBuilder> createPersonBuilder, string prefix, int attempt)
        {
            ChunkId = chunkId;
            CreatePersonBuilder = createPersonBuilder;
            Prefix = prefix;
            Attempt = attempt;

            PersonBuilders = new Dictionary<long, Lazy<IPersonBuilder>>();
            OffsetManager = new KeyMasterOffsetManager(ChunkId, int.Parse(Prefix), attempt);
            LastSavedPersonId = null;
        }


        public void PopulateData(QueryDefinition queryDefinition, IDataReader reader)
        {
            var recordGuid = Guid.NewGuid();

            
            if(queryDefinition.Persons != null) {
                AddEntity(queryDefinition, queryDefinition.Persons, reader, recordGuid, "Persons");
            }

            if(queryDefinition.Death != null) {
                AddEntity(queryDefinition, queryDefinition.Death, reader, recordGuid, "Death");
            }          

            if(queryDefinition.VisitOccurrence != null)
            {
                AddEntity(queryDefinition, queryDefinition.VisitOccurrence, reader, recordGuid, "VisitOccurrence");
            }

            if (queryDefinition.Observation != null)
            {
                AddEntity(queryDefinition, queryDefinition.Observation, reader, recordGuid, "Observation");
            }
            if (queryDefinition.ConditionOccurrence != null)
            {
                AddEntity(queryDefinition, queryDefinition.ConditionOccurrence, reader, recordGuid, "ConditionOccurrence");
            }

            if (queryDefinition.DrugExposure != null)
            {
                AddEntity(queryDefinition, queryDefinition.DrugExposure, reader, recordGuid, "DrugExposure");
            }

            if (queryDefinition.Measurement != null) {
                AddEntity(queryDefinition, queryDefinition.Measurement, reader, recordGuid, "Measurement");
            }

            // No Definition in GOLD
            //AddEntity(queryDefinition, queryDefinition.PayerPlanPeriods, reader, recordGuid, "PayerPlanPeriods");
            //AddEntity(queryDefinition, queryDefinition.VisitDetail, reader, recordGuid, "VisitDetail");
            //AddEntity(queryDefinition, queryDefinition.Cohort, reader, recordGuid, "Cohort");
            //AddEntity(queryDefinition, queryDefinition.Note, reader, recordGuid, "Note");

            //v5.4
            //AddEntity(queryDefinition, queryDefinition.Episode, reader, recordGuid, "Episode");
            //AddEntity(queryDefinition, queryDefinition.EpisodeEvent, reader, recordGuid, "EpisodeEvent");

        }


        private void AddEntity(IEntity entity)
        {
            PersonBuilders[entity.PersonId].Value.AddData(entity);
        }

        private void AddEntity(QueryDefinition queryDefinition, IEnumerable<EntityDefinition> definitions,
           IDataRecord reader, Guid recordGuid, string definitionName)
        {

            if (definitions == null) return;

            foreach (var d in queryDefinition.FindDefinition(definitions, reader))
            {
                var personId = reader.GetLong(d.PersonId);

                if (LastSavedPersonId.HasValue && personId <= LastSavedPersonId) continue;

                if (!queryDefinition.ProcessedPersonIds.ContainsKey(personId.Value))
                    queryDefinition.ProcessedPersonIds.TryAdd(personId.Value, 0);               

                try
                {
                    Concept conceptDef = null;
                    if (d.Concepts != null && d.Concepts.Any())
                        conceptDef = d.Concepts[0];

                    bool added;
                    var pb =
                       PersonBuilders.GetOrAdd(personId.Value, key => new Lazy<IPersonBuilder>(() => CreatePersonBuilder()),
                          out added).Value;

                    pb.JoinToVocabulary(d.Vocabulary);

                    foreach (var entity in d.GetConcepts(conceptDef, reader, OffsetManager))
                    {
                        if (entity == null) continue;

                        entity.SourceRecordGuid = recordGuid;
                        AddEntity(entity);

                        switch (entity.GeEntityType())
                        {
                            case EntityType.DrugExposure:
                                {
                                    var parent = (DrugExposure)entity;
                                    if (queryDefinition.DrugCost != null && queryDefinition.DrugCost[0].Match(reader))
                                    {
                                        PersonBuilders[parent.PersonId].Value.AddChildData(parent, queryDefinition.DrugCost[0].CreateEnity(parent, reader));
                                    }
                                    else {
                                    }
                                    break;
                                }

                            case EntityType.ProcedureOccurrence:
                                {
                                    if (queryDefinition.ProcedureCost != null &&
                                        queryDefinition.ProcedureCost[0].Match(reader))
                                    {
                                        var parent = (ProcedureOccurrence)entity;
                                        PersonBuilders[parent.PersonId].Value.AddChildData(parent,
                                            queryDefinition.ProcedureCost[0].CreateEnity(parent,
                                                reader,
                                                OffsetManager));
                                    }

                                    break;
                                }

                            case EntityType.VisitOccurrence:
                                {
                                    if (queryDefinition.VisitCost != null && queryDefinition.VisitCost[0].Match(reader))
                                    {

                                        var parent = (VisitOccurrence)entity;
                                        PersonBuilders[parent.PersonId].Value.AddChildData(parent,
                                            queryDefinition.VisitCost[0].CreateEnity(parent, reader,
                                                OffsetManager));
                                    }

                                    break;
                                }

                            case EntityType.DeviceExposure:
                                {
                                    var parent = (DeviceExposure)entity;
                                    if (queryDefinition.DeviceCost != null && queryDefinition.DeviceCost[0].Match(reader))
                                        PersonBuilders[parent.PersonId].Value.AddChildData(parent,
                                            queryDefinition.DeviceCost[0].CreateEnity(parent, reader,
                                                OffsetManager));
                                    break;
                                }

                            case EntityType.Observation:
                                {
                                    var parent = (Observation)entity;
                                    if (queryDefinition.ObservationCost != null && queryDefinition.ObservationCost[0].Match(reader))
                                        PersonBuilders[parent.PersonId].Value.AddChildData(parent,
                                            queryDefinition.ObservationCost[0].CreateEnity(parent, reader));
                                    break;
                                }

                            case EntityType.Measurement:
                                {
                                    var parent = (Measurement)entity;
                                    if (queryDefinition.MeasurementCost != null && queryDefinition.MeasurementCost[0].Match(reader))
                                        PersonBuilders[parent.PersonId].Value.AddChildData(parent,
                                            queryDefinition.MeasurementCost[0].CreateEnity(parent, reader));
                                    break;
                                }
                        }
                    }

                    queryDefinition.RowProcessed();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    throw;
                }
            }
            
        }

        public virtual void Dispose()
        {
        }
    }
}