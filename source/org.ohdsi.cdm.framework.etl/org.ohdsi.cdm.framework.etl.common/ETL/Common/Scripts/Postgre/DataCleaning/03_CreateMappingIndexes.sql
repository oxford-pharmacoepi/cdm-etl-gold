-- Patient
-- alter table {sc}.patient add constraint pk_patient primary key (patid);							-- created before data cleaning
create index IF NOT EXISTS idx_patient_pracid on {sc}.patient(pracid);	

-- Practice
-- alter table {sc}.practice add constraint pk_practice primary key (pracid, region);				-- already exists
create index IF NOT EXISTS idx_practice_region on {sc}.practice(region);

-- Consultation
-- alter table {sc}.consultation add constraint pk_consultation primary key (consid);				-- already have an (id) as PK
-- create index IF NOT EXISTS idx_consultation_patid_consid on {sc}.clinical(patid, consid);		-- created before data cleaning
-- cluster {sc}.consultation using idx_consultation_patid_consid;									-- created before data cleaning
-- create index IF NOT EXISTS idx_consultation_eventdate ON {sc}.consultation(eventdate ASC);		-- created before data cleaning
create index IF NOT EXISTS idx_consultation_constype on {sc}.consultation(constype);				-- constype is used during retreving Clinical, Therapy, Referral, Test and Immunisation 


-- Clinical
-- alter table {sc}.clinical add constraint pk_clinical primary key (patid, consid);				-- already have an (id) as PK
-- create index IF NOT EXISTS idx_clinical_patid_consid on {sc}.clinical(patid, consid);			-- created before data cleaning
-- cluster {sc}.clinical using idx_clinical_patid_consid;											-- created before data cleaning
-- create index IF NOT EXISTS idx_clinical_eventdate ON {sc}.clinical(eventdate ASC);				-- created before data cleaning
create index IF NOT EXISTS idx_clinical_staffid on {sc}.clinical(staffid);		
create index IF NOT EXISTS idx_clinical_medcode on {sc}.clinical(medcode);	
create index IF NOT EXISTS idx_clinical_patid_adid on {sc}.clinical(patid, adid);


-- Additional 
-- alter table {sc}.additional add constraint pk_additional primary key (id);						-- already have an (id) as PK
-- create index IF NOT EXISTS idx_additional_patid_adid on {sc}.additional(patid, adid);			-- created before data cleaning
-- cluster {sc}.clinical using idx_additional_patid_adid;											-- created before data cleaning
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


-- Referral
-- alter table {sc}.referral add constraint pk_referral primary key (id);							-- already have an (id) as PK
-- create index IF NOT EXISTS idx_referral_patid_consid on {sc}.referral(patid, consid);			-- created before data cleaning
-- cluster {sc}.clinical using idx_referral_patid_consid;											-- created before data cleaning
create index IF NOT EXISTS idx_referral_medcode on {sc}.referral(medcode);
-- create index IF NOT EXISTS idx_referral_eventdate ON {sc}.referral(eventdate ASC);				-- created before data cleaning


-- Immunisation 
-- alter table {sc}.immunisation add constraint pk_immunisation primary key (id);					-- already have an (id) as PK
-- create index IF NOT EXISTS idx_immunisation_patid_consid on {sc}.immunisation(patid, consid);	-- created before data cleaning
-- cluster {sc}.clinical using idx_immunisation_patid_consid;										-- created before data cleaning
create index IF NOT EXISTS idx_immunisation_medcode on {sc}.immunisation(medcode);
-- create index IF NOT EXISTS idx_immunisation_eventdate ON {sc}.immunisation(eventdate ASC);		-- created before data cleaning


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


-- Therapy
-- alter table {sc}.therapy add constraint pk_therapy primary key (id);								-- already have an (id) as PK
-- create index IF NOT EXISTS idx_therapy_patid_consid on {sc}.therapy(patid, consid);				-- created before data cleaning
-- cluster {sc}.clinical using idx_therapy_patid_consid;
create index IF NOT EXISTS idx_therapy_dosageid on {sc}.therapy(dosageid);
create index IF NOT EXISTS idx_therapy_daysupply_decodes on {sc}.therapy(prodcode, qty, numpacks);
-- create index IF NOT EXISTS idx_therapy_eventdate ON {sc}.therapy(eventdate ASC);					-- created before data cleaning



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


-- Lookup
																									-- already have (lookup_id, lookup_type_id, code) as a PK
create index IF NOT EXISTS idx_lookup_lookup_type_id on {sc}.lookup(lookup_type_id);
create index IF NOT EXISTS idx_lookup_code on {sc}.lookup(code);

-- Lookuptype
																									-- already have (lookup_type_id, name) as a PK
create index IF NOT EXISTS idx_lookuptype_name on {sc}.lookuptype(name);


--Staff
-- alter table {sc}.staff add constraint pk_staff primary key (staffid);							-- already exists
create index IF NOT EXISTS idx_staff_role on {sc}.staff(role);	


-- daysupply_decodes
-- CONSTRAINT daysupply_decodes_pkey PRIMARY KEY (id)
--TO BE ADDED JUST BEFORE MAPPING 	create index idx_daysupply_decodes_prodcode on {sc}.daysupply_decodes(prodcode);   -- prodcode is not unique


-- daysupply_modes
--  CONSTRAINT commondosages_pkey PRIMARY KEY (dosageid)
--TO BE ADDED JUST BEFORE MAPPING 	alter table {sc}.daysupply_modes drop constraint daysupply_modes_pkey;  -- by default id is the PK called daysupply_modes_pkey in daysupply_modes
--TO BE ADDED JUST BEFORE MAPPING 	alter table {sc}.daysupply_modes add constraint pk_daysupply_modes primary key (prodcode);	-- prodcode is unique


-- Medical
-- alter table {sc}.medical add constraint pk_medical primary key (medcode);						-- already exists


-- Product 
-- alter table {sc}.product add constraint pk_product primary key (prodcode);						-- already exists