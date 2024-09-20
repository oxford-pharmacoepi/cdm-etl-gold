namespace org.ohdsi.cdm.framework.common.Enums
{
    public enum EntityType
    {
        Entity,
        ConditionOccurrence,
        Cohort,
        CohortDefinition,
        Death,
        DeviceExposure,
        DrugExposure,
        Measurement,
        Observation,
        PayerPlanPeriod,
        Person,
        ProcedureOccurrence,
        VisitOccurrence,
        VisitDetail,
        Cost,
        ObservationPeriod,
        ConditionEra,
        DrugEra,
        Note,
        Episode,            //New added in CDM v5.4
        EpisodeEvent,       //New added in CDM v5.4
        Specimen            //New added since 202407
    }
}
