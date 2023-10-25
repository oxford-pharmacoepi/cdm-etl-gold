﻿{base},
Standard as (
SELECT distinct SOURCE_CODE, TARGET_CONCEPT_ID, TARGET_DOMAIN_ID, SOURCE_VALID_START_DATE, SOURCE_VALID_END_DATE
FROM Source_to_Standard
WHERE lower(SOURCE_VOCABULARY_ID) IN ('ucum', 'gold_unit_stcm')
AND lower(TARGET_VOCABULARY_ID) IN ('ucum') 
AND (TARGET_INVALID_REASON IS NULL or TARGET_INVALID_REASON = '')
AND TARGET_STANDARD_CONCEPT = 'S'
)

select distinct Standard.*
from Standard