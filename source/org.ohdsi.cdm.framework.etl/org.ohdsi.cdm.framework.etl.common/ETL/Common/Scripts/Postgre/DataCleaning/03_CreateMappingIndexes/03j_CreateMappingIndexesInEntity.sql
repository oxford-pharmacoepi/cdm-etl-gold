-- Entity
-- alter table {sc}.entity add constraint pk_entity primary key (enttype);							-- already have (enttype, filetype, data_fields) as a PK
create index IF NOT EXISTS idx_entity_enttype_data_fields on {sc}.entity(enttype, data_fields); 	-- for joining additional, test					
create index IF NOT EXISTS idx_entity_data1_lkup on {sc}.entity(data1_lkup);
create index IF NOT EXISTS idx_entity_data2_lkup on {sc}.entity(data2_lkup);
create index IF NOT EXISTS idx_entity_data3_lkup on {sc}.entity(data3_lkup);
create index IF NOT EXISTS idx_entity_data4_lkup on {sc}.entity(data4_lkup);
create index IF NOT EXISTS idx_entity_data5_lkup on {sc}.entity(data5_lkup);
create index IF NOT EXISTS idx_entity_data6_lkup on {sc}.entity(data6_lkup);
create index IF NOT EXISTS idx_entity_data7_lkup on {sc}.entity(data7_lkup);
create index IF NOT EXISTS idx_entity_data8_lkup on {sc}.entity(data8_lkup);
create index IF NOT EXISTS idx_entity_data9_lkup on {sc}.entity(data9_lkup);
create index IF NOT EXISTS idx_entity_data10_lkup on {sc}.entity(data10_lkup);
create index IF NOT EXISTS idx_entity_data11_lkup on {sc}.entity(data11_lkup);
create index IF NOT EXISTS idx_entity_data12_lkup on {sc}.entity(data12_lkup);