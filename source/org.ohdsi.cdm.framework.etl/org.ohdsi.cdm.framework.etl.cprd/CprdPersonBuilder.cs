using org.ohdsi.cdm.framework.common.Base;
using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Enums;
using org.ohdsi.cdm.framework.common.Helpers;
using org.ohdsi.cdm.framework.common.Omop;
using org.ohdsi.cdm.framework.common.PregnancyAlgorithm;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace org.ohdsi.cdm.framework.etl.cprd
{
    /// <summary>
    ///  Implementation of PersonBuilder for CPRD, based on CDM Build spec
    /// </summary>
    public class CprdPersonBuilder : PersonBuilder
    {
        string[] covid19_vax_read_code = { "65F0100", "65F0200", "65F0900", "65F0A00", "65F0B00" };
        public override string GetFolder()
        {
            return "ETL\\CPRD";
        }

        public override IEnumerable<VisitDetail> BuildVisitDetails(VisitDetail[] visitDetails,
            VisitOccurrence[] visitOccurrences, ObservationPeriod[] observationPeriods, bool withinTheObservationPeriod)
        {

            foreach (var visitOccurrence in Clean(visitOccurrences, observationPeriods, withinTheObservationPeriod))
            {
                var visitDetail =
                    new VisitDetail(visitOccurrence)
                    {
                        Id = visitOccurrence.Id,
                        CareSiteId = visitOccurrence.CareSiteId,
                    };

                yield return visitDetail;
            }
        }

        /// <summary>
        /// Build person entity and all person related entities like: DrugExposures, ConditionOccurrences, ProcedureOccurrences... from raw data sets 
        /// </summary>
        public override Attrition Build(ChunkData data, KeyMasterOffsetManager om)
        {
            Offset = om;
            ChunkData = data;

            var result = BuildPerson(PersonRecords.ToList());
            var person = result.Key;
            if (person == null)
                return result.Value;

            if (!ObservationPeriodsRaw.Any(op => op.StartDate < op.EndDate))
                return Attrition.InvalidObservationTime;

            var op = ObservationPeriodsRaw.Where(op =>
                    op.PersonId == person.PersonId &&
                    op.StartDate < op.EndDate &&
                    op.StartDate.Year >= person.YearOfBirth &&
                    op.EndDate.Value.Year >= person.YearOfBirth &&
                    op.StartDate.Year <= DateTime.Now.Year).ToArray();

            if (op.Length == 0)
                return Attrition.InvalidObservationTime;

            var observationPeriods =
                BuildObservationPeriods(person.ObservationPeriodGap, op).ToArray();

            var payerPlanPeriods = BuildPayerPlanPeriods(PayerPlanPeriodsRaw.Where(pp =>
                    pp.StartDate.Year >= person.YearOfBirth &&
                    pp.EndDate.Value.Year >= person.YearOfBirth &&
                    pp.StartDate.Year <= DateTime.Now.Year).ToArray(), null).ToArray();


            var visitDetails = new Dictionary<long, VisitDetail>();
            var visitDetIds = new List<long>();


            foreach (var vd in BuildVisitDetails(null, VisitOccurrencesRaw.Where(vo =>
                    vo.StartDate.Year >= person.YearOfBirth &&
                    vo.EndDate.Value.Year >= person.YearOfBirth &&
                    vo.StartDate.Year <= DateTime.Now.Year &&
                    vo.EndDate.Value.Year <= DateTime.Now.Year).ToArray(), observationPeriods, false).ToArray())
            {

                if (person.MonthOfBirth.HasValue && vd.StartDate.Year < person.YearOfBirth.Value &&
                    vd.StartDate.Month < person.MonthOfBirth ||
                    vd.StartDate.Year < person.YearOfBirth.Value)
                    if (vd.StartDate.Year < person.YearOfBirth.Value)
                    {
                        if (DateTime.TryParse(person.AdditionalFields["frd"], out var frd))
                        {
                            vd.StartDate = frd;
                            vd.EndDate = frd;
                        }
                        else
                        {
                            continue;
                        }
                    }

                if (visitDetails.ContainsKey(vd.Id))
                    continue;

                visitDetails.Add(vd.Id, vd);
                visitDetIds.Add(vd.Id);

            }

            long? prevVisitDetId = null;
            foreach (var visitId in visitDetIds.OrderBy(v => v))
            {
                if (prevVisitDetId.HasValue)
                {
                    visitDetails[visitId].PrecedingVisitDetailId = prevVisitDetId;
                }

                prevVisitDetId = visitId;
            }

            var visitOccurrences = new Dictionary<long, VisitOccurrence>();
            var visitIds = new List<long>();
            foreach (var byStartDate in visitDetails.Values.GroupBy(v => v.StartDate))
            {
                var vd = byStartDate.First();
                var providerId = byStartDate.Min(v => v.ProviderId);
                var careSiteId = byStartDate.Min(v => v.CareSiteId);
                var sourceValue = byStartDate.Min(v => v.SourceValue);
                var visitOccurrenceId = byStartDate.Min(v => v.Id);

                var visitOccurrence = new VisitOccurrence(vd)
                {
                    //Id = Offset.GetKeyOffset(vd.PersonId).VisitOccurrenceId,
                    Id = visitOccurrenceId,
                    ProviderId = providerId,
                    CareSiteId = careSiteId,
                    SourceValue = sourceValue
                };

                foreach (var visitDetail in byStartDate)
                {
                    visitDetail.VisitOccurrenceId = visitOccurrence.Id;
                }

                visitOccurrences.Add(visitOccurrence.Id, visitOccurrence);
                visitIds.Add(visitOccurrence.Id);
            }


            long? prevVisitId = null;
            foreach (var visitId in visitIds.OrderBy(v => v))
            {
                if (prevVisitId.HasValue)
                {
                    visitOccurrences[visitId].PrecedingVisitOccurrenceId = prevVisitId;
                }

                prevVisitId = visitId;
            }

            SetVisitOccurrenceId(ConditionOccurrencesRaw, visitDetails);
            SetVisitOccurrenceId(ProcedureOccurrencesRaw, visitDetails);
            SetVisitOccurrenceId(DrugExposuresRaw, visitDetails);
            SetVisitOccurrenceId(DeviceExposureRaw, visitDetails);
            SetVisitOccurrenceId(ObservationsRaw, visitDetails);
            SetVisitOccurrenceId(MeasurementsRaw, visitDetails);

            var drugExposures = BuildDrugExposures(DrugExposuresRaw.ToArray(), visitOccurrences, observationPeriods, false).ToArray();
            var deviceExposure = BuildDeviceExposure(DeviceExposureRaw.ToArray(), visitOccurrences, observationPeriods, false).ToArray();
            var conditionOccurrences = BuildConditionOccurrences(ConditionOccurrencesRaw.ToArray(), visitOccurrences, observationPeriods, false).ToArray();
            var procedureOccurrences = BuildProcedureOccurrences(ProcedureOccurrencesRaw.ToArray(), visitOccurrences, observationPeriods, false).ToArray();

            var observations = BuildObservations(ObservationsRaw.ToArray(), visitOccurrences, observationPeriods, false).ToArray();
            var measurements = BuildMeasurement(MeasurementsRaw.ToArray(), visitOccurrences, observationPeriods, false).ToArray();

            var death = BuildDeath(DeathRecords.ToArray(), visitOccurrences, observationPeriods);

            
            if (death != null)
            {
                person.TimeOfDeath = death.StartDate;

                if (death.StartDate < observationPeriods.Min(op => op.StartDate))
                    return Attrition.UnacceptablePatientQuality;

                if (death.StartDate.Year < person.YearOfBirth || death.StartDate.Year > DateTime.Now.Year)
                    death = null;
            }
            

            // push built entities to ChunkBuilder for further save to CDM database

            AddToChunk(person,
                death,
                observationPeriods,
                payerPlanPeriods,
                Clean(drugExposures, person).ToArray(),
                Clean(conditionOccurrences, person).ToArray(),
                Clean(procedureOccurrences, person).ToArray(),
                Clean(observations, person).ToArray(),
                Clean(measurements, person).ToArray(),
                visitOccurrences.Values.ToArray(),
                visitDetails.Values.ToArray(), null,
                Clean(deviceExposure, person).ToArray(), null, false);

            var pg = new PregnancyAlgorithm();
            foreach (var r in pg.GetPregnancyEpisodes(Vocabulary, person, observationPeriods,
                ChunkData.ConditionOccurrences.Where(e => e.PersonId == person.PersonId).ToArray(),
                ChunkData.ProcedureOccurrences.Where(e => e.PersonId == person.PersonId).ToArray(),
                ChunkData.Observations.Where(e => e.PersonId == person.PersonId).ToArray(),
                ChunkData.Measurements.Where(e => e.PersonId == person.PersonId).ToArray(),
                ChunkData.DrugExposures.Where(e => e.PersonId == person.PersonId).ToArray()))
            {
                r.Id = Offset.GetKeyOffset(r.PersonId).ConditionEraId;
                ChunkData.ConditionEra.Add(r);
            }


            return Attrition.None;
        }

        /// <summary>
        /// Build person entity and all person related entities like: DrugExposures, ConditionOccurrences, ProcedureOccurrences... from raw data sets 
        /// </summary>
        public override Attrition BuildCdm(ChunkData data, KeyMasterOffsetManager om, long pkey, bool withinTheObservationPeriod)
        {
            Offset = om;
            ChunkData = data;

            PersonRecords = data.Persons.Where(p=>p.PersonId == pkey).ToList();

            var result = BuildPerson(PersonRecords);
            var person = result.Key;
            if (person == null)
                return result.Value;

            //Debug.WriteLine($"person.PersonId={person.PersonId}");

            //var observationPeriods = data.ObservationPeriods.ToArray();
            var observationPeriods = data.ObservationPeriods.Where(op => op.PersonId == person.PersonId).ToArray();

            var visitDetails = new Dictionary<long, VisitDetail>();
            var visitDetIds = new List<long>();
            var tempVisitDetails = new Dictionary<string, VisitDetail>();    //TempVisitDetailId

            
            //set visit_occurrence_id in vo_raw
            var visitDetailIds = new Dictionary<string, long>();
            var grpby = VisitOccurrencesRaw.GroupBy(u => u.AdditionalFields["temp_visit_occurrence_id"]).Select(x => x.OrderBy(u => u.Id).First());
            
            foreach (var obj in grpby)
            {
                var key = obj.AdditionalFields["temp_visit_occurrence_id"];

                if (visitDetailIds.ContainsKey(key))
                    continue;

                visitDetailIds.Add(key, obj.Id);
            }

            foreach (var vd in VisitOccurrencesRaw) {

                var key = vd.AdditionalFields["temp_visit_occurrence_id"];

                if (visitDetailIds.ContainsKey(key))
                    vd.Id = visitDetailIds[key];

            }

            foreach (var vd in BuildVisitDetails(null, VisitOccurrencesRaw.Where(vo =>
                    vo.StartDate.Year >= person.YearOfBirth &&
                    vo.EndDate.Value.Year >= person.YearOfBirth &&
                    vo.StartDate.Year <= DateTime.Now.Year &&
                    vo.EndDate.Value.Year <= DateTime.Now.Year).ToArray(), observationPeriods, withinTheObservationPeriod).ToArray().OrderBy(u=>u.StartDate).ThenBy(u=>u.AdditionalFields["constype"]))
            {

                if (person.MonthOfBirth.HasValue && vd.StartDate.Year < person.YearOfBirth.Value &&
                    vd.StartDate.Month < person.MonthOfBirth ||
                    vd.StartDate.Year < person.YearOfBirth.Value)

                    if (vd.StartDate.Year < person.YearOfBirth.Value)
                    {
                        if (DateTime.TryParse(person.AdditionalFields["frd"], out var frd))
                        {
                            vd.StartDate = frd;
                            vd.EndDate = frd;
                        }
                        else
                        {
                            continue;
                        }
                    }

                if (visitDetails.ContainsKey(vd.Id))
                    continue;

                visitDetails.Add(vd.Id, vd);
                visitDetIds.Add(vd.Id);

                tempVisitDetails.Add(vd.AdditionalFields["temp_visit_occurrence_id"], vd);

            }

            long? prevVisitDetId = null;
            //foreach (var visitId in visitDetIds.OrderBy(v => v))
            foreach (var visitId in visitDetIds)
            {
                if (prevVisitDetId.HasValue)
                {
                    visitDetails[visitId].PrecedingVisitDetailId = prevVisitDetId;
                }

                prevVisitDetId = visitId;
            }
            


            var visitOccurrences = new Dictionary<long, VisitOccurrence>();
            var visitIds = new List<long>();
            foreach (var byStartDate in visitDetails.Values.OrderBy(x => x.StartDate).GroupBy(v => v.StartDate))
            {
                var vd = byStartDate.First();
                var providerId = byStartDate.Min(v => v.ProviderId);
                var careSiteId = byStartDate.Min(v => v.CareSiteId);
                var sourceValue = byStartDate.Min(v => v.SourceValue);
                var visitOccurrenceId = byStartDate.Min(v => v.Id);

                var visitOccurrence = new VisitOccurrence(vd)
                {
                    //Id = Offset.GetKeyOffset(vd.PersonId).VisitOccurrenceId,
                    Id = visitOccurrenceId,
                    ProviderId = providerId,
                    CareSiteId = careSiteId,
                    SourceValue = sourceValue
                };

                foreach (var visitDetail in byStartDate)
                {
                    visitDetail.VisitOccurrenceId = visitOccurrence.Id;
                }

                visitOccurrences.Add(visitOccurrence.Id, visitOccurrence);
                visitIds.Add(visitOccurrence.Id);
            }
            

            long? prevVisitId = null;
            //foreach (var visitId in visitIds.OrderBy(v => v))
            foreach (var visitId in visitIds)
            {
                if (prevVisitId.HasValue)
                {
                    visitOccurrences[visitId].PrecedingVisitOccurrenceId = prevVisitId;
                }

                prevVisitId = visitId;
            }

            SetVisitOccurrenceId(ConditionOccurrencesRaw, tempVisitDetails);
            SetVisitOccurrenceId(ProcedureOccurrencesRaw, tempVisitDetails);
            SetVisitOccurrenceId(DrugExposuresRaw, tempVisitDetails);
            SetVisitOccurrenceId(DeviceExposureRaw, tempVisitDetails);
            SetVisitOccurrenceId(ObservationsRaw, tempVisitDetails);
            SetVisitOccurrenceId(MeasurementsRaw, tempVisitDetails);


            var drugExposures = BuildDrugExposures(DrugExposuresRaw.ToArray(), visitOccurrences, observationPeriods, withinTheObservationPeriod).ToArray();
            var deviceExposure = BuildDeviceExposure(DeviceExposureRaw.ToArray(), visitOccurrences, observationPeriods, withinTheObservationPeriod).ToArray();
            var conditionOccurrences = BuildConditionOccurrences(ConditionOccurrencesRaw.ToArray(), visitOccurrences, observationPeriods, withinTheObservationPeriod).ToArray();
            var procedureOccurrences = BuildProcedureOccurrences(ProcedureOccurrencesRaw.ToArray(), visitOccurrences, observationPeriods, withinTheObservationPeriod).ToArray();
 
            var observations = BuildObservations(ObservationsRaw.ToArray(), visitOccurrences, observationPeriods, withinTheObservationPeriod).ToArray();
            var measurements = BuildMeasurement(MeasurementsRaw.ToArray(), visitOccurrences, observationPeriods, withinTheObservationPeriod).ToArray();

            // push built entities to ChunkBuilder for further save to CDM database

            AddToChunk(null,
                null,
                observationPeriods,
                null,
                Clean(drugExposures, person).ToArray(),
                Clean(conditionOccurrences, person).ToArray(),
                Clean(procedureOccurrences, person).ToArray(),
                Clean(observations, person).ToArray(),
                Clean(measurements, person).ToArray(),
                visitOccurrences.Values.ToArray(),
                visitDetails.Values.ToArray(), null,
                Clean(deviceExposure, person).ToArray(), null, withinTheObservationPeriod);
            
            
            var pg = new PregnancyAlgorithm();
            foreach (var r in pg.GetPregnancyEpisodes(Vocabulary, person, observationPeriods,
                ChunkData.ConditionOccurrences.Where(e => e.PersonId == person.PersonId).ToArray(),
                ChunkData.ProcedureOccurrences.Where(e => e.PersonId == person.PersonId).ToArray(),
                ChunkData.Observations.Where(e => e.PersonId == person.PersonId).ToArray(),
                ChunkData.Measurements.Where(e => e.PersonId == person.PersonId).ToArray(),
                ChunkData.DrugExposures.Where(e => e.PersonId == person.PersonId).ToArray()))
            {
                r.Id = Offset.GetKeyOffset(r.PersonId).ConditionEraId;
                ChunkData.ConditionEra.Add(r);
            }
            

            return Attrition.None;
        }

        public IEnumerable<T> Clean<T>(IEnumerable<T> entities, Person person) where T : class, IEntity
        {
            foreach (var e in entities)
            {
                if (e.StartDate.Year < person.YearOfBirth || e.StartDate.Year > DateTime.Now.Year)
                    continue;

                if (e.EndDate.HasValue && (
                    e.EndDate.Value.Year < person.YearOfBirth ||
                    e.EndDate.Value.Year > DateTime.Now.Year))
                    continue;

                yield return e;
            }
        }

        public override IEnumerable<T> BuildEntities<T>(IEnumerable<T> entitiesToBuild, IDictionary<long, VisitOccurrence> visitOccurrences, IEnumerable<ObservationPeriod> observationPeriods,
            bool withinTheObservationPeriod)
        {
            var uniqueEntities = new HashSet<T>();

            //foreach (var e in entitiesToBuild)
            foreach (var e in Clean(entitiesToBuild, observationPeriods, withinTheObservationPeriod))
            {
                if (e.VisitOccurrenceId == null || visitOccurrences.ContainsKey(e.VisitOccurrenceId.Value))
                {
                    if (e.IsUnique)
                    {
                        uniqueEntities.Add(e);
                    }
                    else
                    {
                        yield return e;
                    }
                }
            }

            foreach (var ue in uniqueEntities)
            {
                yield return ue;
            }
        }

        private void SetVisitOccurrenceId<T>(IEnumerable<T> inputRecords, IDictionary<long, VisitDetail> vd)
            where T : class, IEntity
        {
            foreach (var e in inputRecords)
            {
                if (!e.VisitDetailId.HasValue)
                    continue;

                if (vd.ContainsKey(e.VisitDetailId.Value))
                {
                    e.VisitOccurrenceId = vd[e.VisitDetailId.Value].VisitOccurrenceId;
                }
            }
        }

        private void SetVisitOccurrenceId<T>(IEnumerable<T> inputRecords, IDictionary<string, VisitDetail> vd)
            where T : class, IEntity
        {
            foreach (var e in inputRecords)
            {
                //if (!e.VisitDetailId.HasValue)
                //    continue;
                //if (e.AdditionalFields==null)
                //    continue;

                if (vd.ContainsKey(e.AdditionalFields["temp_visit_occurrence_id"]))
                {
                    
                    var obj = vd[e.AdditionalFields["temp_visit_occurrence_id"]];
                    e.VisitDetailId = obj.Id;
                    e.VisitOccurrenceId = obj.VisitOccurrenceId;

                }
            }
        }

        public override KeyValuePair<Person, Attrition> BuildPerson(List<Person> records)
        {

            if (records == null || records.Count == 0)
                return new KeyValuePair<Person, Attrition>(null, Attrition.UnacceptablePatientQuality);

            //var ordered = records.OrderByDescending(p => p.StartDate).ToArray();
            var ordered = records.ToArray();
            var person = ordered.Take(1).First();
            /*
            if (person.AdditionalFields["accept"] != "1")
                return new KeyValuePair<Person, Attrition>(null, Attrition.UnacceptablePatientQuality);
            */
            /*

            person.StartDate = ordered.Take(1).Last().StartDate;
            var gender =
                records.GroupBy(p => p.GenderConceptId).OrderByDescending(gp => gp.Count()).Take(1).First().First();
            var race = records.GroupBy(p => p.RaceConceptId).OrderByDescending(gp => gp.Count()).Take(1).First()
                .First();

            person.GenderConceptId = gender.GenderConceptId;
            person.GenderSourceValue = gender.GenderSourceValue;
            person.RaceConceptId = race.RaceConceptId;
            person.RaceSourceValue = race.RaceSourceValue;
            */
            /*
            if (person.YearOfBirth < 1875)
                return new KeyValuePair<Person, Attrition>(null, Attrition.ImplausibleYOBPast);

            if (person.GenderConceptId == 8551) //UNKNOWN
            {
                return new KeyValuePair<Person, Attrition>(null, Attrition.UnknownGender);
            }

            if (person.GenderConceptId == 8507 && records.Any(p => p.GenderConceptId == 8532))
            {
                return new KeyValuePair<Person, Attrition>(null, Attrition.GenderChanges);
            }

            if (person.GenderConceptId == 8532 && records.Any(p => p.GenderConceptId == 8507))
            {
                return new KeyValuePair<Person, Attrition>(null, Attrition.GenderChanges);
            }
            */
            return new KeyValuePair<Person, Attrition>(person, Attrition.None);
        }

        private string GetDomain2(string entityDomain, string conceptDomain)
        {
            if (conceptDomain == null)
                return entityDomain;

            if (conceptDomain.StartsWith("Condition", StringComparison.OrdinalIgnoreCase))
                return "Condition";

            if (conceptDomain.StartsWith("Measurement", StringComparison.OrdinalIgnoreCase))
                return "Measurement";

            if (conceptDomain.StartsWith("Observation", StringComparison.OrdinalIgnoreCase))
                return "Observation";

            if (conceptDomain.StartsWith("Procedure", StringComparison.OrdinalIgnoreCase))
                return "Procedure";

            if (conceptDomain.StartsWith("Device", StringComparison.OrdinalIgnoreCase))
                return "Device";

            if (conceptDomain.StartsWith("Drug", StringComparison.OrdinalIgnoreCase))
                return "Drug";


            return entityDomain;
        }

        private int? GetValueAsConceptId(IEntity e, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (e.AdditionalFields == null || !e.AdditionalFields.ContainsKey("data"))
                return null;

            switch (e.AdditionalFields["data"].ToLower().Trim())
            {
                case "read code for condition":
                    var result = Vocabulary.Lookup(value, "Read_Code", DateTime.MinValue);
                    return result.Any() ? result[0].ConceptId : null;

                case "drug code":
                    var result1 = Vocabulary.Lookup(value, "Drug", DateTime.MinValue);
                    return result1.Any() ? result1[0].ConceptId : null;

                default:
                    return null;
            }
        }

        public override void AddToChunk(string domain, IEnumerable<IEntity> entities)
        {
            foreach (var entity in entities)
            {
                //Debug.WriteLine($"domain=" + domain + " entity.Domain =" + entity.Domain);
                var entityDomain = GetDomain2(domain, entity.Domain);

                switch (entityDomain)
                {
                    case "Condition":

                        var cond = entity as ConditionOccurrence ??
                                   new ConditionOccurrence(entity)
                                   {
                                       Id = Offset.GetKeyOffset(entity.PersonId).ConditionOccurrenceId
                                   };
                        cond.TypeConceptId = 32817; //32020 EHR encounter diagnosis -> 32817 EHR
                        ConditionForEra.Add(cond);
                        ChunkData.AddData(cond);

                        break;

                    case "Measurement":
                        var mes = entity as Measurement ?? new Measurement(entity)
                        {
                            Id = Offset.GetKeyOffset(entity.PersonId).MeasurementId
                        };

                        mes.TypeConceptId = 32856; //44818702 Lab result -> 32856 Lab

                        if (!string.IsNullOrEmpty(mes.SourceValue))
                        {
                            var result = Vocabulary.Lookup(mes.SourceValue, "Read_Code", mes.StartDate);

                            if (result.Any()
                                && result.Count > 0
                                && result[0].SourceConceptId > 0
                                /*&& result[0].Domain == "Measurement"*/)
                            {
                                mes.SourceConceptId = result[0].SourceConceptId;
                            }
                        }

                        if (mes.ConceptId > 0)
                        {
                            if (string.Equals(mes.SourceValue, "4J3R200", StringComparison.Ordinal))
                            {
                                mes.ConceptId = 756065;
                                mes.ValueSourceValue = "Not Detected";
                                mes.ValueAsConceptId = 9190;
                            }
                            else if (string.Equals(mes.SourceValue, "4J3R100", StringComparison.Ordinal))
                            {
                                mes.ConceptId = 756065;
                                mes.ValueSourceValue = "Detected";
                                mes.ValueAsConceptId = 4126681;
                            }
                            else if (string.Equals(mes.SourceValue, "4J3R.00", StringComparison.Ordinal))
                            {
                                mes.ConceptId = 706179;
                            }

                            ChunkData.AddData(mes);
                        }
                        else
                        {
                            var o = new Observation(entity)
                            {
                                Id = Offset.GetKeyOffset(entity.PersonId).ObservationId
                            };
                            ChunkData.AddData(o);
                        }
                        break;

                    case "Observation":
                        var obs = entity as Observation ?? new Observation(entity)
                        {
                            Id = Offset.GetKeyOffset(entity.PersonId).ObservationId
                        };
                        obs.TypeConceptId = 32817; //38000280 Observation recorded from EHR -> 32817 EHR
                        var valueAsConceptId = GetValueAsConceptId(obs, obs.ValueAsString);
                        if (valueAsConceptId.HasValue)
                            obs.ValueAsConceptId = valueAsConceptId.Value;

                        ChunkData.AddData(obs);
                        break;

                    case "Procedure":
                        var proc = entity as ProcedureOccurrence ??
                                   new ProcedureOccurrence(entity)
                                   {
                                       Id = Offset.GetKeyOffset(entity.PersonId)
                                           .ProcedureOccurrenceId
                                   };
                        proc.TypeConceptId = 38000275; //EHR order list entry
                        ChunkData.AddData(proc);
                        break;

                    case "Device":
                        ChunkData.AddData(entity as DeviceExposure ??
                                          new DeviceExposure(entity)
                                          {
                                              Id = Offset.GetKeyOffset(entity.PersonId).DeviceExposureId
                                          });
                        break;

                    case "Drug":
                        var drg = entity as DrugExposure ??
                                  new DrugExposure(entity)
                                  {
                                      Id = Offset.GetKeyOffset(entity.PersonId).DrugExposureId
                                  };

                        //38000177 = “Prescription written” =>  32838 = “EHR prescription” (STANDARD)
                        //drg.TypeConceptId = 38000177; •	
                        drg.TypeConceptId = 32838;


                        //Drug|CVX                                                                                          
                        if ((entity.Domain != null && entity.Domain.EndsWith("CVX", StringComparison.OrdinalIgnoreCase))
                            //From immunisation -> vaccines
                            || entity.AdditionalFields.ContainsKey("immstype")
                            //Covid 19 Vaccination from others event (not immunisation) e.g. Clinical
                            || Array.IndexOf(covid19_vax_read_code, drg.ConceptIdKey) >= 0)
                        {

                            //38000179 = “Physician administered drug(identified as procedure)”  => 32818 = “EHR administration” (STANDARD)
                            //drg.TypeConceptId = 38000179; // Physician administered drug (identified as procedure)
                            drg.TypeConceptId = 32818;

                        }

                        
                        //Covid-19 vaccine brand name mapping
                        //*** if the immunisation records came from Clinical not Immunisation, there is no immstype
                        //the concept id remains unchanged
                        if (Array.IndexOf(covid19_vax_read_code, drg.ConceptIdKey) >= 0 && entity.AdditionalFields.ContainsKey("immstype"))
                        {
                            //Debug.WriteLine($"Before: ConceptId={drg.ConceptId}, ConceptIdKey={drg.ConceptIdKey}, PersonId={drg.PersonId}");

                            int new_concept_id = UpdateConceptIdByCov19VaxBrandName(entity);

                            //Debug.WriteLine($"After: ConceptId={drg.ConceptId}, ConceptIdKey={drg.ConceptIdKey}, PersonId={drg.PersonId}");
                        }
                        


                        DrugForEra.Add(drg);
                        ChunkData.AddData(drg);
                        break;

                }

            }
        }

        public override IEnumerable<ProcedureOccurrence> BuildProcedureOccurrences(
            ProcedureOccurrence[] procedureOccurrences, Dictionary<long, VisitOccurrence> visitOccurrences,
            ObservationPeriod[] observationPeriods, bool withinTheObservationPeriod)
        {
            //return BuildEntities(procedureOccurrences, visitOccurrences, observationPeriods, false);
            return BuildEntities(procedureOccurrences, visitOccurrences, observationPeriods, withinTheObservationPeriod);
        }

        public override Death BuildDeath(Death[] death, Dictionary<long, VisitOccurrence> visitOccurrences,
            ObservationPeriod[] observationPeriods)
        {
            var ds = Clean(death, observationPeriods, false).ToList();
            if (ds.Any())
            {
                var pd = ds.Where(d => d.Primary).ToList();
                var result = pd.Any() ? pd.OrderBy(d => d.StartDate).Last() : ds.OrderBy(d => d.StartDate).Last();

                if (result.StartDate.Year < 1900)
                    return null;

                return result;
            }

            return null;
        }

        public override IEnumerable<Observation> BuildObservations(Observation[] observations,
            Dictionary<long, VisitOccurrence> visitOccurrences, ObservationPeriod[] observationPeriods, bool withinTheObservationPeriod)
        {
            //return BuildEntities(observations, visitOccurrences, observationPeriods, false);
            return BuildEntities(observations, visitOccurrences, observationPeriods, withinTheObservationPeriod);
        }

        public override IEnumerable<Measurement> BuildMeasurement(Measurement[] measurements,
            Dictionary<long, VisitOccurrence> visitOccurrences,
            ObservationPeriod[] observationPeriods, bool withinTheObservationPeriod)
        {
            //foreach (var mes in BuildEntities(measurements, visitOccurrences, observationPeriods, false))
            foreach (var mes in BuildEntities(measurements, visitOccurrences, observationPeriods, withinTheObservationPeriod))
            {
                //if (!string.IsNullOrEmpty(mes.SourceValue))
                //{
                //    var result = Vocabulary.Lookup(mes.SourceValue, "Read_Code", mes.StartDate);
                //    mes.SourceConceptId = result.Any() ? result[0].ConceptId ?? 0 : 0;
                //}

                yield return mes;
            }
        }

        public override IEnumerable<EraEntity> BuildConditionEra(ConditionOccurrence[] conditionOccurrences, ObservationPeriod[] observationPeriods, bool withinTheObservationPeriod)
        {
            foreach (var eraEntity in EraHelper.GetEras(
                //Clean(conditionOccurrences, observationPeriods, false), 30, 38000247)
                Clean(conditionOccurrences, observationPeriods, withinTheObservationPeriod), 30, 38000247)
             )
            {
                eraEntity.Id = Offset.GetKeyOffset(eraEntity.PersonId).ConditionEraId;
                yield return eraEntity;
            }
        }
        public override IEnumerable<EraEntity> BuildDrugEra(DrugExposure[] drugExposures, ObservationPeriod[] observationPeriods, bool withinTheObservationPeriod)
        {
            foreach (var eraEntity in EraHelper.GetEras(
                //Clean(drugExposures, observationPeriods, false), 30, 38000182)
                Clean(drugExposures, observationPeriods, withinTheObservationPeriod), 30, 38000182)
                )
            {
                eraEntity.Id = Offset.GetKeyOffset(eraEntity.PersonId).DrugEraId;
                yield return eraEntity;
            }
        }

        public override IEnumerable<DeviceExposure> BuildDeviceExposure(DeviceExposure[] devExposure,
            Dictionary<long, VisitOccurrence> visitOccurrences,
            ObservationPeriod[] observationPeriods, bool withinTheObservationPeriod)
        {
            //return BuildEntities(devExposure, visitOccurrences, observationPeriods, false);
            return BuildEntities(devExposure, visitOccurrences, observationPeriods, withinTheObservationPeriod);
        }

        public override int UpdateConceptIdByCov19VaxBrandName(IEntity e)
        {
            switch (e.AdditionalFields["immstype"])
            {
                case "130": //COVTALENT -> 724905 SARS-COV-2 (COVID-19) vaccine, vector non-replicating, recombinant spike protein-ChAdOx1, preservative free, 0.5 mL
                    return 724905;
                case "132": //COVOXFORD -> 724905 SARS-COV-2 (COVID-19) vaccine, vector non-replicating, recombinant spike protein-ChAdOx1, preservative free, 0.5 mL
                    return 724905;
                case "133": //COVPFIZER -> 37003436 SARS-CoV-2 (COVID-19) vaccine, mRNA-BNT162b2 0.1 MG/ML Injectable Suspension
                    return 37003436;
                case "134": //COVJANSSEN -> 739906 SARS-COV-2 (COVID-19) vaccine, vector - Ad26 100000000000 UNT/ML Injectable Suspension
                    return 739906;
                case "135": //COVMODERNA -> 37003518 SARS-CoV-2 (COVID-19) vaccine, mRNA-1273 0.2 MG/ML Injectable Suspension
                    return 37003518;
                case "143": //COVVALNEVA -> 702666 SARS-COV-2 COVID-19 Inactivated Virus Non-US Vaccine Product (COVAXIN)
                    return 702666;
                case "145": //COVNOVAVAX -> 36119721 COVID-19 vaccine, recombinant, full-length nanoparticle spike (S) protein, adjuvanted with Matrix-M Injectable Suspension
                    return 36119721;
                //To-Do: new immstype 
                //COVMEDICAGO -> 36119722	COVID-19 vaccine, recombinant, plant-derived Virus-Like Particle (VLP) spike (S) protein, adjuvanted with AS03 Injectable Suspension
                default:
                    //Debug.WriteLine($"PersonId={e.PersonId}, immstype={e.AdditionalFields["immstype"]}");  // 127 COVOTHER -> 724904 (SARS-COV-2 (COVID-19) vaccine, UNSPECIFIED)
                    break;

            }
            return e.ConceptId;
        }

    }
}
