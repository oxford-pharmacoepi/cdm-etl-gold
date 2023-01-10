create index IF NOT EXISTS idx_test_patid_consid on {sc}.test(patid, consid);	
create index IF NOT EXISTS idx_test_eventdate ON {sc}.test(eventdate ASC);
cluster {sc}.test using idx_test_patid_consid;