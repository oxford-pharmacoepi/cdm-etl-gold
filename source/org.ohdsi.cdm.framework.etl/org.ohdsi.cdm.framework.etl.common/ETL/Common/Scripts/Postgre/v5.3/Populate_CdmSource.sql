INSERT INTO {sc}.cdm_source
SELECT
CASE 
	WHEN LEFT(current_database(),10) = 'cdm_gold_p' THEN
		current_database()
	ELSE
		CONCAT('Clinical Practice Research Datalink, GOLD ', RIGHT(current_database(), 6),'  release')
END,
CASE 
	WHEN LEFT(current_database(),10) = 'cdm_gold_p' THEN
		current_database()
	ELSE
		CONCAT('CPRD GOLD ', RIGHT(current_database(), 6))
END,
'NDORMS',
'The Clinical Practice Research Datalink (CPRD) GOLD is a database of anonymised electronic health records (EHR) from General Practitioner (GP) clinics in the UK.',
'https://cprd.com/primary-care-data-public-health-research',
'https://github.com/oxford-pharmacoepi/cdm-etl-gold/',
CASE 
	WHEN LEFT(current_database(),10) = 'cdm_gold_p' THEN
		to_date('{SOURCE_RELEASE_DATE}', 'YYYY-MM-DD')
	ELSE
		TO_DATE(RIGHT(current_database(), 6),'YYYYMM')
END,
NOW(),
CONCAT('CDM ', '{CdmVersion}'),
(SELECT vocabulary_version FROM public.vocabulary WHERE vocabulary_id = 'None');