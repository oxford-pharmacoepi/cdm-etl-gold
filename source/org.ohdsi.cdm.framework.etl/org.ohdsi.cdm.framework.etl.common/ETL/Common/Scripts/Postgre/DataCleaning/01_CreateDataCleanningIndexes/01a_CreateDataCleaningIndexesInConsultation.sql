-- alter table {sc}.patient add constraint pk_patient primary key (patid);			-- already exist
create index IF NOT EXISTS idx_consultation_eventdate ON {sc}.consultation(eventdate ASC);