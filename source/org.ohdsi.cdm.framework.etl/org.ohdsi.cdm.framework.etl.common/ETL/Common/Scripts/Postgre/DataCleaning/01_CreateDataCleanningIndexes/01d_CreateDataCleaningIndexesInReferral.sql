create index IF NOT EXISTS idx_referral_patid_consid on {sc}.referral(patid, consid);	
create index IF NOT EXISTS idx_referral_eventdate ON {sc}.referral(eventdate ASC);
cluster {sc}.referral using idx_referral_patid_consid;