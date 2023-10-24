using org.ohdsi.cdm.framework.common.Enums;
using System;
using System.Collections.Generic;

namespace org.ohdsi.cdm.framework.common.Omop
{
    public class DeviceExposure : Entity, IEquatable<DeviceExposure>
    {
        public string UniqueDeviceId { get; set; }
        public int Quantity { get; set; }

        // CDM v5.4 props
        public string ProductionId { get; set; }
        public int UnitConceptId { get; set; }
        public string UnitSourceValue { get; set; }
        public int UnitSourceConceptId { get; set; }

        public List<DeviceCost> DeviceCosts { get; set; }

        public DeviceExposure(IEntity ent)
        {
            Init(ent);
        }

        public bool Equals(DeviceExposure other)
        {
            return this.PersonId.Equals(other.PersonId) &&
                   this.ConceptId == other.ConceptId &&
                   this.TypeConceptId == other.TypeConceptId &&
                   this.SourceValue == other.SourceValue &&
                   this.StartDate == other.StartDate &&
                   this.SourceConceptId == other.SourceConceptId &&
                   this.UniqueDeviceId == other.UniqueDeviceId &&
                   this.Quantity == other.Quantity &&
                   this.ProviderId == other.ProviderId &&
                   this.EndDate == other.EndDate &&
                   this.ProductionId == other.ProductionId &&
                   this.UnitConceptId == other.UnitConceptId &&
                   this.UnitSourceValue == other.UnitSourceValue &&
                   this.UnitSourceConceptId == other.UnitSourceConceptId;

        }

        public override int GetHashCode()
        {
            return PersonId.GetHashCode() ^
                   ConceptId.GetHashCode() ^
                   TypeConceptId.GetHashCode() ^
                   ProviderId.GetHashCode() ^
                   Quantity.GetHashCode() ^
                   (SourceValue != null ? SourceValue.GetHashCode() : 0) ^
                   SourceConceptId.GetHashCode() ^
                   (UniqueDeviceId != null ? UniqueDeviceId.GetHashCode() : 0) ^
                   (StartDate.GetHashCode()) ^
                   (EndDate.GetHashCode()) ^
                   (ProductionId.GetHashCode()) ^
                   (UnitConceptId.GetHashCode()) ^
                   (UnitSourceValue.GetHashCode()) ^
                   (UnitSourceConceptId.GetHashCode());
        }

        public override EntityType GeEntityType()
        {
            return EntityType.DeviceExposure;
        }
    }
}
