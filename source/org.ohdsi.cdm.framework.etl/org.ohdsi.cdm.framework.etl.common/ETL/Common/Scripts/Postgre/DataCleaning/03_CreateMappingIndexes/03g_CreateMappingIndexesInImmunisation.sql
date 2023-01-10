-- Immunisation 
-- alter table {sc}.immunisation add constraint pk_immunisation primary key (id);					-- already have an (id) as PK
-- create index IF NOT EXISTS idx_immunisation_patid_consid on {sc}.immunisation(patid, consid);	-- created before data cleaning
-- cluster {sc}.clinical using idx_immunisation_patid_consid;										-- created before data cleaning
create index IF NOT EXISTS idx_immunisation_medcode on {sc}.immunisation(medcode);
-- create index IF NOT EXISTS idx_immunisation_eventdate ON {sc}.immunisation(eventdate ASC);		-- created before data cleaning