-- Additional 
-- alter table {sc}.additional add constraint pk_additional primary key (id);						-- already have an (id) as PK
-- create index IF NOT EXISTS idx_additional_patid_adid on {sc}.additional(patid, adid);			-- created before data cleaning
-- cluster {sc}.additional using idx_additional_patid_adid;											-- created before data cleaning
create index IF NOT EXISTS idx_additional_enttype on {sc}.additional(enttype);
create index IF NOT EXISTS idx_additional_data1 on {sc}.additional(data1);
create index IF NOT EXISTS idx_additional_data2 on {sc}.additional(data2);
create index IF NOT EXISTS idx_additional_data3 on {sc}.additional(data3);
create index IF NOT EXISTS idx_additional_data4 on {sc}.additional(data4);
create index IF NOT EXISTS idx_additional_data5 on {sc}.additional(data5);
create index IF NOT EXISTS idx_additional_data6 on {sc}.additional(data6);
create index IF NOT EXISTS idx_additional_data7 on {sc}.additional(data7);
-- create index IF NOT EXISTS idx_additional_data8 on {sc}.additional(data8);						-- missing in cdm_gold_202201
-- create index IF NOT EXISTS idx_additional_data9 on {sc}.additional(data9);						-- missing in cdm_gold_202201
-- create index IF NOT EXISTS idx_additional_data10 on {sc}.additional(data10);						-- missing in cdm_gold_202201
-- create index IF NOT EXISTS idx_additional_data11 on {sc}.additional(data11);						-- missing in cdm_gold_202201
-- create index IF NOT EXISTS idx_additional_data12 on {sc}.additional(data12);						-- missing in cdm_gold_202201