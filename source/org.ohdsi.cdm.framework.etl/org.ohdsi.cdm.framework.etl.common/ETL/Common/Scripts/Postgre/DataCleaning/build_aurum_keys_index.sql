--PRACTICE
alter table {SOURCE_SCHEMA}.practice add constraint pk_practice primary key (pracid);
create index idx_practice_region on {SOURCE_SCHEMA}.practice(region);
cluster {SOURCE_SCHEMA}.practice using idx_practice_region;

--PATIENT
alter table {SOURCE_SCHEMA}.patient add constraint pk_patient primary key (patid);
create index idx_patient_staffid on {SOURCE_SCHEMA}.patient(usualgpstaffid);

--STAFF
alter table {SOURCE_SCHEMA}.staff add constraint pk_sfatt primary key (staffid);
create index idx_staff_jobid on {SOURCE_SCHEMA}.staff(jobcatid);

--CONSULTATION
alter table {SOURCE_SCHEMA}.consultation add constraint pk_consultation primary key (consid);
create index idx_consultation_patid on {SOURCE_SCHEMA}.consultation(patid);
cluster {SOURCE_SCHEMA}.consultation using idx_consultation_patid;
create index idx_consultation_consmedcodeid on {SOURCE_SCHEMA}.consultation (consmedcodeid);
create index idx_consultation_staffid on {SOURCE_SCHEMA}.consultation (staffid);

--DRUGISSUE
alter table {SOURCE_SCHEMA}.drugissue add constraint pk_drugissue primary key (issueid);
create index idx_drugissue_patid on {SOURCE_SCHEMA}.drugissue (patid);
cluster {SOURCE_SCHEMA}.drugissue using idx_drugissue_patid;
create index idx_drugissue_prodcodeid on {SOURCE_SCHEMA}.drugissue (prodcodeid);
create index idx_drugissue_probobsid on {SOURCE_SCHEMA}.drugissue (probobsid);
create index idx_drugissue_quantunitid on {SOURCE_SCHEMA}.drugissue (quantunitid);
create index idx_drugissue_staffid on {SOURCE_SCHEMA}.drugissue (staffid);

--OBSERVATION
alter table {SOURCE_SCHEMA}.observation add constraint pk_observation primary key (obsid);
create index idx_observation_patid on {SOURCE_SCHEMA}.observation (patid);
cluster {SOURCE_SCHEMA}.observation using idx_observation_patid;
create index idx_observation_medcodeid on {SOURCE_SCHEMA}.observation (medcodeid);
create index idx_observation_consid on {SOURCE_SCHEMA}.observation (consid);
create index idx_observation_numunitid on {SOURCE_SCHEMA}.observation (numunitid);
create index idx_observation_staffid on {SOURCE_SCHEMA}.observation (staffid);
create index idx_observation_obsdate on {SOURCE_SCHEMA}.observation (obsdate);

--PROBLEM
alter table {SOURCE_SCHEMA}.problem add constraint pk_problem primary key (obsid);
create index idx_problem_patid on {SOURCE_SCHEMA}.problem(patid);
cluster {SOURCE_SCHEMA}.problem using idx_problem_patid;
create index idx_problem_staffid on {SOURCE_SCHEMA}.problem(lastrevstaffid);

-- REFERRAL
alter table {SOURCE_SCHEMA}.referral add constraint pk_referral primary key (obsid);
create index idx_referral_patid on {SOURCE_SCHEMA}.referral(patid);
cluster {SOURCE_SCHEMA}.referral using idx_referral_patid;
create index idx_referral_refservicetypeid on {SOURCE_SCHEMA}.referral(refservicetypeid);
