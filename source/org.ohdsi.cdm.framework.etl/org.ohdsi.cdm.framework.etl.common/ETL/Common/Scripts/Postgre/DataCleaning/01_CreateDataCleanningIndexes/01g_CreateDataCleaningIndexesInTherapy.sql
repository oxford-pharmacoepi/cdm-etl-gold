create index IF NOT EXISTS idx_therapy_patid_consid on {sc}.therapy(patid, consid);
create index IF NOT EXISTS idx_therapy_eventdate ON {sc}.therapy(eventdate ASC);
cluster {sc}.therapy using idx_therapy_patid_consid;