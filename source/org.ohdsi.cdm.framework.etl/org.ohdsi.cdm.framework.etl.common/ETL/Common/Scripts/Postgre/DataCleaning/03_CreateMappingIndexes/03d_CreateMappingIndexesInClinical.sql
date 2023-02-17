-- Clinical
-- alter table {sc}.clinical add constraint pk_clinical primary key (patid, consid);				-- already have an (id) as PK
-- create index IF NOT EXISTS idx_clinical_patid_consid on {sc}.clinical(patid, consid);			-- created before data cleaning
-- cluster {sc}.clinical using idx_clinical_patid_consid;											-- created before data cleaning
-- create index IF NOT EXISTS idx_clinical_eventdate ON {sc}.clinical(eventdate ASC);				-- created before data cleaning
create index IF NOT EXISTS idx_clinical_staffid on {sc}.clinical(staffid);		
-- create index IF NOT EXISTS idx_clinical_medcode on {sc}.clinical(medcode);						-- already exists
create index IF NOT EXISTS idx_clinical_patid_adid on {sc}.clinical(patid, adid);