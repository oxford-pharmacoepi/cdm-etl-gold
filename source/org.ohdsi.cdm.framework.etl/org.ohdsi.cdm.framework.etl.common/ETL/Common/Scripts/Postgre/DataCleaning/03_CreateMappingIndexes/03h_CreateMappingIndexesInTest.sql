-- Test
-- alter table {sc}.test add constraint pk_test primary key (id);									-- already have an (id) as PK
-- create index IF NOT EXISTS idx_test_patid_consid on {sc}.test(patid, consid);					-- created before data cleaning
-- cluster {sc}.clinical using idx_test_patid_consid;												-- created before data cleaning
create index IF NOT EXISTS idx_test_medcode on {sc}.test(medcode);
create index IF NOT EXISTS idx_test_data1 on {sc}.test(data1);
create index IF NOT EXISTS idx_test_data2 on {sc}.test(data2);
create index IF NOT EXISTS idx_test_data3 on {sc}.test(data3);
create index IF NOT EXISTS idx_test_data4 on {sc}.test(data4);
create index IF NOT EXISTS idx_test_data5 on {sc}.test(data5);
create index IF NOT EXISTS idx_test_data6 on {sc}.test(data6);
create index IF NOT EXISTS idx_test_data7 on {sc}.test(data7);
create index IF NOT EXISTS idx_test_data8 on {sc}.test(data8);
-- create index IF NOT EXISTS idx_test_eventdate ON {sc}.test(eventdate ASC);						-- created before data cleaning