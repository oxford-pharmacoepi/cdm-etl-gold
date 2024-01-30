ALTER TABLE {sc}.DRUG_ERA ADD CONSTRAINT xpk_DRUG_ERA PRIMARY KEY (drug_era_id) USING INDEX TABLESPACE {tablespace};
CREATE INDEX idx_drug_era_person_id_1  ON {sc}.drug_era  (person_id ASC) TABLESPACE {tablespace};
CLUSTER {sc}.drug_era  USING idx_drug_era_person_id_1;
CREATE INDEX idx_drug_era_concept_id_1 ON {sc}.drug_era (drug_concept_id ASC) TABLESPACE {tablespace};