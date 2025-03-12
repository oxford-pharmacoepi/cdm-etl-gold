SELECT distinct SOURCE_CODE, TARGET_CONCEPT_ID, 'None' as Domain, NULL AS valid_start_date, NULL AS valid_end_date
FROM {sc}.SOURCE_TO_CONCEPT_MAP
where source_vocabulary_id in ('GOLD_SPECIALITY_STCM')