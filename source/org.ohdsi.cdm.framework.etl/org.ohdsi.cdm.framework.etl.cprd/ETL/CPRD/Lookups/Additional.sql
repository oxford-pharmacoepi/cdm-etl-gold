﻿{base},
Standard as (
SELECT distinct SOURCE_CODE, TARGET_CONCEPT_ID, TARGET_DOMAIN_ID, SOURCE_VALID_START_DATE, SOURCE_VALID_END_DATE
FROM Source_to_Standard
WHERE SOURCE_VOCABULARY_ID IN ('GOLD_ADD_ETYPE_STCM')  AND (TARGET_INVALID_REASON is NULL or TARGET_INVALID_REASON = '') AND TARGET_STANDARD_CONCEPT = 'S'
)

select distinct Standard.*
from Standard