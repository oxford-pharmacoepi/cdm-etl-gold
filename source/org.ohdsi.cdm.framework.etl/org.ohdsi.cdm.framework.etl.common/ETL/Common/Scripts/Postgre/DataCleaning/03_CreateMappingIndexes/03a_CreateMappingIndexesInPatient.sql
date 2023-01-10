-- Patient
-- alter table {sc}.patient add constraint pk_patient primary key (patid);							-- created before data cleaning
create index IF NOT EXISTS idx_patient_pracid on {sc}.patient(pracid);	