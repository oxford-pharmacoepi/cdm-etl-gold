INSERT INTO {sc}.cdm_source
SELECT
CONCAT('Clinical Practice Research Datalink, GOLD ', RIGHT(current_database(), 6),'  release'),
CONCAT('CPRD GOLD ', RIGHT(current_database(), 6)),
'NDORMS',
'The Clinical Practice Research Datalink (CPRD) GOLD is a database of anonymised electronic health records (EHR) from General Practitioner (GP) clinics in the UK.',
'https://cprd.com/primary-care-data-public-health-research',
'https://github.com/oxford-pharmacoepi/cdm-etl-gold/',
TO_DATE(RIGHT(current_database(), 6),'YYYYMM'), 
NOW(),
CONCAT('CDM ', '{CdmVersion}'),
(SELECT vocabulary_version FROM public.vocabulary WHERE vocabulary_id = 'None');