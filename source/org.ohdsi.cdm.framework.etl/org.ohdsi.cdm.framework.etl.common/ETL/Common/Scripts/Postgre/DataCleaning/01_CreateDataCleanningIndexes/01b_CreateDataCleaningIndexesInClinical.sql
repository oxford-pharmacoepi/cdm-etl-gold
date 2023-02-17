create index IF NOT EXISTS idx_clinical_patid_consid on {sc}.clinical(patid, consid);			
create index IF NOT EXISTS idx_clinical_eventdate ON {sc}.clinical(eventdate ASC);
cluster {sc}.clinical using idx_clinical_patid_consid;	