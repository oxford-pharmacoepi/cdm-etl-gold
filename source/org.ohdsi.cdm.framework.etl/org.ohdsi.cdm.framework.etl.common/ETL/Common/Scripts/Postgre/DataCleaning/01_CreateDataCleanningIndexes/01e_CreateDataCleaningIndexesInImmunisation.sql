create index IF NOT EXISTS idx_immunisation_patid_consid on {sc}.immunisation(patid, consid);
create index IF NOT EXISTS idx_immunisation_eventdate ON {sc}.immunisation(eventdate ASC);
cluster {sc}.immunisation using idx_immunisation_patid_consid;