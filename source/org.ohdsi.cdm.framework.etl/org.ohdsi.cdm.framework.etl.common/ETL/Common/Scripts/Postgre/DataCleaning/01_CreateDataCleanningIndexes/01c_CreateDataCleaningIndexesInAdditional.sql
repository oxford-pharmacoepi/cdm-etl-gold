create index IF NOT EXISTS idx_additional_patid_adid on {sc}.additional(patid, adid);
cluster {sc}.additional using idx_additional_patid_adid;