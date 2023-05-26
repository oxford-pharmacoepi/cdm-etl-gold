﻿{base},
Standard as (
SELECT distinct SOURCE_CODE, TARGET_CONCEPT_ID, TARGET_DOMAIN_ID, SOURCE_VALID_START_DATE, SOURCE_VALID_END_DATE
FROM Source_to_Standard
WHERE lower(SOURCE_VOCABULARY_ID) IN ('gold_add_entype_stcm', 'gold_drug_stcm')  AND (TARGET_INVALID_REASON is NULL or TARGET_INVALID_REASON = '') AND lower(TARGET_STANDARD_CONCEPT) = 's'
)

select distinct Standard.*
from Standard