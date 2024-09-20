using org.ohdsi.cdm.framework.common.Enums;
using System;

namespace org.ohdsi.cdm.framework.common.Omop
{
    public class Specimen : Entity, IEquatable<Specimen>
    {
        public decimal? Quantity { get; set; }
        public int UnitConceptId { get; set; }
        public int AnatomicSiteConceptId { get; set; }
        public int DiseaseStatusConceptId { get; set; }
        public string UnitSourceValue { get; set; }
        public string AnatomicSiteSourceValue { get; set; }
        public string DiseaseStatusSourceValue { get; set; }

        public string SpecimenSourceId { get; set; }
        public string SpecimenSourceValue { get; set; }

        public Specimen(IEntity ent)
        {
            Init(ent);
        }

        public bool Equals(Specimen other)
        {
            return this.PersonId.Equals(other.PersonId) &&
                   this.ConceptId.Equals(other.ConceptId) &&
                   this.StartDate.Equals(other.StartDate) &&
                   this.EndDate.Equals(other.EndDate) &&
                   this.VisitOccurrenceId.Equals(other.VisitOccurrenceId) &&
                   this.TypeConceptId.Equals(other.TypeConceptId) &&
                   this.Quantity == other.Quantity &&
                   this.UnitConceptId == other.UnitConceptId &&
                   this.AnatomicSiteConceptId == other.AnatomicSiteConceptId &&
                   this.DiseaseStatusConceptId == other.DiseaseStatusConceptId &&
                   this.UnitSourceValue == other.UnitSourceValue &&
                   this.AnatomicSiteSourceValue == other.AnatomicSiteSourceValue &&
                   this.DiseaseStatusSourceValue == other.DiseaseStatusSourceValue &&
                   this.SpecimenSourceId == other.SpecimenSourceId &&
                   this.ProviderId.Equals(other.ProviderId) &&
                   this.SourceValue.Equals(other.SourceValue) &&
                   this.SpecimenSourceValue.Equals(other.SpecimenSourceValue);
        }

        public override int GetHashCode()
        {
            return PersonId.GetHashCode() ^
                   ConceptId.GetHashCode() ^
                   (StartDate.GetHashCode()) ^
                   (EndDate.GetHashCode()) ^
                   TypeConceptId.GetHashCode() ^
                   Quantity.GetHashCode() ^
                   VisitOccurrenceId.GetHashCode() ^
                   SpecimenSourceId.GetHashCode() ^
                   SpecimenSourceValue.GetHashCode() ^
                   ProviderId.GetHashCode() ^
                   (SourceValue != null ? SourceValue.GetHashCode() : 0) ^
                   UnitConceptId.GetHashCode() ^
                   (AnatomicSiteConceptId != null ? AnatomicSiteConceptId.GetHashCode() : 0) ^
                   (DiseaseStatusConceptId != null ? DiseaseStatusConceptId.GetHashCode() : 0) ^
                   (UnitSourceValue != null ? UnitSourceValue.GetHashCode() : 0) ^
                   (AnatomicSiteSourceValue != null ? AnatomicSiteSourceValue.GetHashCode() : 0) ^
                   (DiseaseStatusSourceValue != null ? DiseaseStatusSourceValue.GetHashCode() : 0) 
                   ;
        }

        public override EntityType GeEntityType()
        {
            return EntityType.Specimen;
        }
    }
}
