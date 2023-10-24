using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Extensions;
using org.ohdsi.cdm.framework.common.Omop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace org.ohdsi.cdm.framework.common.Definitions
{
    public class LocationDefinition : EntityDefinition
    {
        public string Zip { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string SourceValue { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        // CDM v5.4 props
        public string CountryConceptId { get; set; }
        public string CountrySourceValue { get; set; }

        /*
        public override IEnumerable<IEntity> GetConcepts(Concept concept, IDataRecord reader,
            KeyMasterOffsetManager keyOffset)
        {
            var loc = new Location
            {
                State = reader.GetString(State),
                SourceValue = reader.GetString(SourceValue),
                County = reader.GetString(Country),
                Address1 = reader.GetString(Address1),
                Address2 = reader.GetString(Address2),
                Zip = reader.GetString(Zip)
            };

            loc.Id = string.IsNullOrEmpty(Id) ? Entity.GetId(loc.GetKey()) : reader.GetLong(Id).Value;
            yield return loc;
        }
        */

        public override IEnumerable<IEntity> GetConcepts(Concept concept, IDataRecord reader,
            KeyMasterOffsetManager keyOffset)
        {
            if (concept == null)
            {
                yield return new Location
                {
                    Id = reader.GetInt(Id).Value,
                    State = reader.GetString(State),
                    SourceValue = reader.GetString(SourceValue),
                    County = reader.GetString(Country),
                    Address1 = reader.GetString(Address1),
                    Address2 = reader.GetString(Address2),
                    Zip = reader.GetString(Zip),
                    CountryConceptId = 0
                };

            }
            else
            {
                var conceptField = concept.Fields[0];

                var sourceValue = conceptField.DefaultSource;
                if (string.IsNullOrEmpty(sourceValue))
                    sourceValue = reader.GetString(conceptField.Key);

                if (!string.IsNullOrEmpty(conceptField.SourceKey))
                    sourceValue = reader.GetString(conceptField.SourceKey);

                if (sourceValue != null && sourceValue.Length == 0)
                    sourceValue = null;

                var countryConceptIds = concept.GetConceptIdValues(Vocabulary, conceptField, reader).ToList();

                int? countryConcept = null;

                if (countryConceptIds.Count > 0)
                {
                    if (countryConceptIds[0].ConceptId != 0)
                        countryConcept = countryConceptIds[0].ConceptId;

                }

                yield return new Location
                {
                    Id = reader.GetLong(Id).Value,
                    State = reader.GetString(State),
                    SourceValue = reader.GetString(SourceValue),
                    County = reader.GetString(Country),
                    Address1 = reader.GetString(Address1),
                    Address2 = reader.GetString(Address2),
                    Zip = reader.GetString(Zip),
                    CountryConceptId = countryConcept ?? 0,
                    CountrySourceValue = sourceValue
                };
            }

        }
    }
}
