-- Referral
-- alter table {sc}.referral add constraint pk_referral primary key (id);							-- already have an (id) as PK
-- create index IF NOT EXISTS idx_referral_patid_consid on {sc}.referral(patid, consid);			-- created before data cleaning
-- cluster {sc}.clinical using idx_referral_patid_consid;											-- created before data cleaning
create index IF NOT EXISTS idx_referral_medcode on {sc}.referral(medcode);
-- create index IF NOT EXISTS idx_referral_eventdate ON {sc}.referral(eventdate ASC);				-- created before data cleaning