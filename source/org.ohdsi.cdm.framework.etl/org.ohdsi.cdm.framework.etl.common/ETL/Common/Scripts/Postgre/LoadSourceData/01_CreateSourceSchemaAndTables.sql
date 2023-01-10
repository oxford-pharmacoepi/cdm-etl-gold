drop table if exists {sc}.practice;
drop table if exists {sc}.patient;
drop table if exists {sc}.staff;
drop table if exists {sc}.consultation;
drop table if exists {sc}.clinical;
drop table if exists {sc}.additional;
drop table if exists {sc}.referral;
drop table if exists {sc}.therapy;
drop table if exists {sc}.immunisation;
drop table if exists {sc}.test;


drop table if exists {sc}.commondosage;
drop table if exists {sc}.covid_test_mappings;
drop table if exists {sc}.daysupply_decodes;
drop table if exists {sc}.daysupply_modes;
drop table if exists {sc}.entity;
drop table if exists {sc}.lookup;
drop table if exists {sc}.lookuptype;
drop table if exists {sc}.medical;
drop table if exists {sc}.product;
drop table if exists {sc}.scoringmethod;
drop table if exists {sc}.source_to_concept_map;
drop table if exists {sc}.source_to_source;
drop table if exists {sc}.source_to_standard;



create table {sc}.practice (
	pracid 		integer 	NOT NULL,
	region 		smallint 	NOT NULL
	lcd 		date 		NOT NULL,
	uts 		date 		NOT NULL
);

create table {sc}.patient (
	patid 		bigint		NOT NULL,
	pracid 		integer,
	gender 		smallint	NOT NULL,
	yob 		smallint	NOT NULL,
	mob 		smallint	NOT NULL,
	frd 		date,
	tod 		date,
	toreason 	smallint	NOT NULL,
	deathdate	date,
	accept		smallint	NOT NULL
);

create table {sc}.staff (
	staffid 	bigint		NOT NULL,
	gender 		smallint,
	role 		smallint	NOT NULL
);

create sequence {sc}.consultation_id_seq 
START WITH 1 
INCREMENT BY 1;

create table {sc}.consultation (
	id			bigint		NOT NULL	DEFAULT	nextval('{sc}.consultation_id_seq'::regclass)
	patid 		bigint		NOT NULL,
	eventdate	date,
	constype	smallint,
	consid		integer		NOT NULL,
	staffid		bigint		NOT NULL
);

create sequence {sc}.clinical_id_seq 
START WITH 1 
INCREMENT BY 1;

create table IF NOT EXISTS source.clinical
(
    id 			bigint 		NOT NULL 	DEFAULT nextval('{sc}.clinical_id_seq'::regclass),
    patid 		bigint 		NOT NULL,
    eventdate 	date,
    constype 	smallint,
    consid 		integer 	NOT NULL,
    medcode 	bigint 		NOT NULL,
    staffid 	bigint,
    adid 		integer 	NOT NULL
);

create sequence {sc}.additional_id_seq 
START WITH 1 
INCREMENT BY 1;

CREATE TABLE {sc}.additional
(
    id 			bigint 			NOT NULL 	DEFAULT nextval('{sc}.additional_id_seq'::regclass),
    patid 		bigint 			NOT NULL,
    enttype 	integer 		NOT NULL,
    adid 		integer 		NOT NULL,
    data1 character varying(20) 			DEFAULT NULL::character varying,
    data2 character varying(20) 			DEFAULT NULL::character varying,
    data3 character varying(20) 			DEFAULT NULL::character varying,
    data4 character varying(20) 			DEFAULT NULL::character varying,
    data5 character varying(20) 			DEFAULT NULL::character varying,
    data6 character varying(20) 			DEFAULT NULL::character varying,
    data7 character varying(20) 			DEFAULT NULL::character varying
)

create sequence {sc}.referral_id_seq 
START WITH 1 
INCREMENT BY 1;

CREATE TABLE {sc}.referral
(
    id 			bigint 		NOT NULL 	DEFAULT nextval('{sc}.referral_id_seq'::regclass),
    patid 		bigint 		NOT NULL,
    eventdate 	date,
    consid 		integer 	NOT NULL,
    medcode 	bigint 		NOT NULL,
    staffid 	bigint
)

create sequence {sc}.therapy_id_seq 
START WITH 1 
INCREMENT BY 1;

CREATE TABLE {sc}.therapy
(
    id 			bigint 				NOT NULL 	DEFAULT nextval('{sc}.therapy_id_seq'::regclass),
    patid 		bigint 				NOT NULL,
    eventdate 	date,
    consid 		integer 			NOT NULL,
    prodcode 	bigint 				NOT NULL,
    staffid 	bigint 				NOT NULL,
    dosageid 	character varying(64) 			DEFAULT NULL::character varying,
    qty numeric(11,3) 							DEFAULT NULL::numeric,
    numdays integer,
    numpacks numeric(10,3) 						DEFAULT NULL::numeric,
    packtype integer,
    issueseq integer,
)


create sequence {sc}.immunisation_id_seq 
START WITH 1 
INCREMENT BY 1;


CREATE TABLE {sc}.immunisation
(
    id 			bigint 				NOT NULL 	DEFAULT nextval('{sc}.immunisation_id_seq'::regclass),
    patid 		bigint 				NOT NULL,
    eventdate 	date,
    consid 		integer 			NOT NULL,
    medcode 	bigint 				NOT NULL,
    staffid 	bigint 				NOT NULL
)

create sequence {sc}.test_id_seq 
START WITH 1 
INCREMENT BY 1;


CREATE TABLE {sc}.test
(
    id 			bigint 					NOT NULL 	DEFAULT nextval('{sc}.test_id_seq'::regclass),
    patid 		bigint 					NOT NULL,
    eventdate 	date,
    consid 		integer 				NOT NULL,
    medcode 	bigint 					NOT NULL,
    staffid 	bigint,
    enttype 	integer 				NOT NULL,
    data1 		character varying(20) 				DEFAULT NULL::character varying,
    data2 		character varying(20) 				DEFAULT NULL::character varying,
    data3 		character varying(20) 				DEFAULT NULL::character varying,
    data4 		character varying(20) 				DEFAULT NULL::character varying,
    data5 		character varying(20) 				DEFAULT NULL::character varying,
    data6 		character varying(20) 				DEFAULT NULL::character varying,
    data7 		character varying(20) 				DEFAULT NULL::character varying,
    data8 		character varying(20) 				DEFAULT NULL::character varying
)


CREATE TABLE {sc}.commondosages
(
    dosageid 			character varying(64) 	NOT NULL,
    dosage_text 		character varying(1000) 			DEFAULT NULL::character varying,
    daily_dose 			numeric(15,3) 						DEFAULT NULL::numeric,
    dose_number 		numeric(15,3) 						DEFAULT NULL::numeric,
    dose_unit 			character varying(7) 				DEFAULT NULL::character varying,
    dose_frequency 		numeric(15,3) 						DEFAULT NULL::numeric,
    dose_interval 		numeric(15,3) 						DEFAULT NULL::numeric,
    choice_of_dose 		smallint,
    dose_max_average 	smallint,
    change_dose 		smallint,
    dose_duration 		numeric(15,3) 						DEFAULT NULL::numeric
)

CREATE TABLE {sc}.covid_test_mappings
(
    measurement_source_value 	character varying(250) 		NOT NULL,
    concept_id 					integer,
    value_as_concept_id 		integer 					NOT NULL
)

create sequence {sc}.daysupply_decodes_id_seq 
START WITH 1 
INCREMENT BY 1;

CREATE TABLE {sc}.daysupply_decodes
(
    id 				bigint 			NOT NULL 	DEFAULT nextval('{sc}.daysupply_decodes_id_seq'::regclass),
    prodcode 		integer 		NOT NULL,
    daily_dose 		numeric(15,3),
    qty 			numeric(9,2),
    numpacks 		integer,
    numdays 		smallint
)



daysupply_modes;
entity;
lookup;
lookuptype;
medical;
product;
scoringmethod;
source_to_concept_map;
source_to_source;
source_to_standard;