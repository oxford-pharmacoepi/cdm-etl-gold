ALTER TABLE {sc}.CONDITION_ERA ADD CONSTRAINT xpk_CONDITION_ERA PRIMARY KEY (condition_era_id) USING INDEX TABLESPACE {tablespace};
CREATE INDEX idx_condition_era_person_id_1  ON {sc}.condition_era  (person_id ASC) TABLESPACE {tablespace};		
CLUSTER {sc}.condition_era  USING idx_condition_era_person_id_1;
CREATE INDEX idx_condition_era_concept_id_1 ON {sc}.condition_era (condition_concept_id ASC) TABLESPACE {tablespace};