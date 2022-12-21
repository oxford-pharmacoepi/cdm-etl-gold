alter table {sc}.patient add constraint pk_patient primary key (patid);			-- already exist

create index IF NOT EXISTS idx_consultation_patid_consid on {sc}.consultation(patid, consid);	
cluster {sc}.consultation using idx_consultation_patid_consid;

create index IF NOT EXISTS idx_clinical_patid_consid on {sc}.clinical(patid, consid);	
cluster {sc}.clinical using idx_clinical_patid_consid;
	
create index IF NOT EXISTS idx_additional_patid_adid on {sc}.additional(patid, adid);	
cluster {sc}.additional using idx_additional_patid_adid;

create index IF NOT EXISTS idx_referral_patid_consid on {sc}.referral(patid, consid);	
cluster {sc}.referral using idx_referral_patid_consid;

create index IF NOT EXISTS idx_immunisation_patid_consid on {sc}.immunisation(patid, consid);
cluster {sc}.immunisation using idx_immunisation_patid_consid;

create index IF NOT EXISTS idx_test_patid_consid on {sc}.test(patid, consid);	
cluster {sc}.test using idx_test_patid_consid;

create index IF NOT EXISTS idx_therapy_patid_consid on {sc}.therapy(patid, consid);
cluster {sc}.therapy using idx_therapy_patid_consid;
	
	
create index IF NOT EXISTS idx_consultation_eventdate ON {sc}.consultation(eventdate ASC);
create index IF NOT EXISTS idx_clinical_eventdate ON {sc}.clinical(eventdate ASC);
create index IF NOT EXISTS idx_referral_eventdate ON {sc}.referral(eventdate ASC);
create index IF NOT EXISTS idx_immunisation_eventdate ON {sc}.immunisation(eventdate ASC);
create index IF NOT EXISTS idx_test_eventdate ON {sc}.test(eventdate ASC);
create index IF NOT EXISTS idx_therapy_eventdate ON {sc}.therapy(eventdate ASC);