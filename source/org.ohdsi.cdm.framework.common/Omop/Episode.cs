using org.ohdsi.cdm.framework.common.Enums;
using System;
using System.Collections.Generic;

namespace org.ohdsi.cdm.framework.common.Omop
{
    public class Episode : Entity, IEquatable<Episode>
    {
        public long? EpisodeParentId { get; set; }

        public int EpisodeNumber { get; set; }

        public int EpisodeObjectConeptId { get; set; }


        public Episode(Entity ent)
        {
            Init(ent);
        }

        public bool Equals(Episode other)
        {
            return this.PersonId.Equals(other.PersonId) &&
                   this.ConceptId == other.ConceptId &&
                   this.StartDate.Equals(other.StartDate) &&
                   this.EndDate.Equals(other.EndDate) &&
                   this.EpisodeParentId == other.EpisodeParentId &&
                   this.EpisodeNumber == other.EpisodeNumber &&
                   this.EpisodeObjectConeptId == other.EpisodeObjectConeptId &&
                   this.TypeConceptId == other.TypeConceptId &&
                   this.SourceValue.Equals(other.SourceValue) &&
                   this.SourceConceptId == other.SourceConceptId;

        }

        public override int GetHashCode()
        {
            return PersonId.GetHashCode() ^
                   ConceptId.GetHashCode() ^
                   (StartDate.GetHashCode()) ^
                   (EndDate.GetHashCode()) ^
                   (EpisodeParentId.GetHashCode()) ^
                   (EpisodeNumber.GetHashCode()) ^
                   (EpisodeObjectConeptId.GetHashCode()) ^
                   TypeConceptId.GetHashCode() ^
                   (SourceValue != null ? SourceValue.GetHashCode() : 0) ^
                   SourceConceptId.GetHashCode();
        }
        
        public override EntityType GeEntityType()
        {
           return EntityType.Episode;
        }
        
    }
}
