-- Consultation
-- alter table {sc}.consultation add constraint pk_consultation primary key (consid);				-- already have an (id) as PK
-- create index IF NOT EXISTS idx_consultation_patid_consid on {sc}.clinical(patid, consid);		-- created before data cleaning
-- cluster {sc}.consultation using idx_consultation_patid_consid;									-- created before data cleaning
-- create index IF NOT EXISTS idx_consultation_eventdate ON {sc}.consultation(eventdate ASC);		-- created before data cleaning
create index IF NOT EXISTS idx_consultation_constype on {sc}.consultation(constype);				-- constype is used during retreving Clinical, Therapy, Referral, Test and Immunisation 