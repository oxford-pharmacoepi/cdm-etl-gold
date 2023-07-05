using org.ohdsi.cdm.framework.desktop.Enums;
using org.ohdsi.cdm.presentation.builder.Controllers;
using System;
using System.Xml.Serialization;

namespace org.ohdsi.cdm.presentation.builder
{
    public class Building
    {
        #region Properties

        public DateTime? DataCleaningStart { get; set; }
        public DateTime? DataCleaningEnd { get; set; }
        public DateTime? CreateDestinationDbStart { get; set; }
        public DateTime? CreateDestinationDbEnd { get; set; }
        public DateTime? CreateSchemaSchemaStart { get; set; }
        public DateTime? CreateSchemaSchemaEnd { get; set; }
        public DateTime? CreateChunksStart { get; set; }
        public DateTime? CreateChunksEnd { get; set; }
        public DateTime? CreateLookupStart { get; set; }
        public DateTime? CreateLookupEnd { get; set; }
        public DateTime? MapAllPatientsStart { get; set; }
        public DateTime? MapAllPatientsEnd { get; set; }
        public DateTime? BuildingStart { get; set; }
        public DateTime? BuildingEnd { get; set; }

        public DateTime? CreateCdmIndexesStart { get; set; }
        public DateTime? CreateCdmIndexesEnd { get; set; }
        public DateTime? MapAllDeathStart { get; set; }
        public DateTime? MapAllDeathEnd { get; set; }
        public DateTime? CopyVocabularyStart { get; set; }
        public DateTime? CopyVocabularyEnd { get; set; }
        public DateTime? CreateIndexesStart { get; set; }
        public DateTime? CreateIndexesEnd { get; set; }
        public DateTime? PostprocessStart { get; set; }
        public DateTime? PostprocessEnd { get; set; }

        [XmlIgnore]
        public bool DataCleaningStarted
        {
            get
            {
                if (DataCleaningDone)
                    return false;

                if (!DataCleaningStart.HasValue)
                    return false;

                return DataCleaningStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool DataCleaningDone
        {
            get
            {
                if (!DataCleaningEnd.HasValue)
                    return false;

                return DataCleaningEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool CreateCdmIndexesDone
        {
            get
            {
                if (!CreateCdmIndexesEnd.HasValue)
                    return false;

                return CreateCdmIndexesEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool MapAllDeathStarted
        {
            get
            {
                if (MapAllDeathDone)
                    return false;

                if (!MapAllDeathStart.HasValue)
                    return false;

                return MapAllDeathStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool MapAllDeathDone
        {
            get
            {
                if (!MapAllDeathEnd.HasValue)
                    return false;

                return MapAllDeathEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool CreateCdmIndexesStarted
        {
            get
            {
                if (CreateCdmIndexesDone)
                    return false;

                if (!CreateCdmIndexesStart.HasValue)
                    return false;

                return CreateCdmIndexesStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool DestinationStarted
        {
            get
            {
                if (DestinationCreated)
                    return false;

                if (!CreateDestinationDbStart.HasValue)
                    return false;

                return CreateDestinationDbStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool DestinationCreated
        {
            get
            {
                if (!CreateDestinationDbEnd.HasValue)
                    return false;

                return CreateDestinationDbEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool ScratchSchemaStarted
        {
            get
            {
                if (ScratchSchemaCreated)
                    return false;

                if (!CreateSchemaSchemaStart.HasValue)
                    return false;

                return CreateSchemaSchemaStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool ScratchSchemaCreated
        {
            get
            {
                if (!CreateSchemaSchemaEnd.HasValue)
                    return false;

                return CreateSchemaSchemaEnd.Value != DateTime.MinValue;
            }
        }


        [XmlIgnore]
        public bool MapAllPatientsStarted
        {
            get
            {
                if (MapAllPatientsDone)
                    return false;

                if (!MapAllPatientsStart.HasValue)
                    return false;

                return MapAllPatientsStart.Value != DateTime.MinValue;
            }

        }


        [XmlIgnore]
        public bool MapAllPatientsDone
        {
            get
            {
                if (!MapAllPatientsEnd.HasValue)
                    return false;

                return MapAllPatientsEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool ChunksStarted
        {
            get
            {
                if (ChunksCreated)
                    return false;

                if (!CreateChunksStart.HasValue)
                    return false;

                return CreateChunksStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool ChunksCreated
        {
            get
            {
                if (!CreateChunksEnd.HasValue)
                    return false;

                return CreateChunksEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool LookupStarted
        {
            get
            {
                if (LookupCreated)
                    return false;

                if (!CreateLookupStart.HasValue)
                    return false;

                return CreateLookupStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool LookupCreated
        {
            get
            {
                if (!CreateLookupEnd.HasValue)
                    return false;

                return CreateLookupEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool BuildingStarted
        {
            get
            {
                if (!BuildingStart.HasValue)
                    return false;

                return BuildingStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool BuildingComplete
        {
            get
            {
                if (!BuildingEnd.HasValue)
                    return false;

                return BuildingEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool IndexesStarted
        {
            get
            {
                if (IndexesCreated)
                    return false;

                if (!CreateIndexesStart.HasValue)
                    return false;

                return CreateIndexesStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool IndexesCreated
        {
            get
            {
                if (!CreateIndexesEnd.HasValue)
                    return false;

                return CreateIndexesEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool VocabularyStarted
        {
            get
            {
                if (VocabularyCopied)
                    return false;

                if (!CopyVocabularyStart.HasValue)
                    return false;

                return CopyVocabularyStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool VocabularyCopied
        {
            get
            {
                if (!CopyVocabularyEnd.HasValue)
                    return false;

                return CopyVocabularyEnd.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool PostprocessStarted
        {
            get
            {
                if (PostprocessFinished)
                    return false;

                if (!PostprocessStart.HasValue)
                    return false;

                return PostprocessStart.Value != DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public bool PostprocessFinished
        {
            get
            {
                if (!PostprocessEnd.HasValue)
                    return false;

                return PostprocessEnd.Value != DateTime.MinValue;
            }
        }

        #endregion
    }
}
