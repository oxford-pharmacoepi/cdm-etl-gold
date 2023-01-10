-- Therapy
-- alter table {sc}.therapy add constraint pk_therapy primary key (id);								-- already have an (id) as PK
-- create index IF NOT EXISTS idx_therapy_patid_consid on {sc}.therapy(patid, consid);				-- created before data cleaning
-- cluster {sc}.clinical using idx_therapy_patid_consid;
create index IF NOT EXISTS idx_therapy_dosageid on {sc}.therapy(dosageid);
create index IF NOT EXISTS idx_therapy_daysupply_decodes on {sc}.therapy(prodcode, qty, numpacks);
-- create index IF NOT EXISTS idx_therapy_eventdate ON {sc}.therapy(eventdate ASC);					-- created before data cleaning