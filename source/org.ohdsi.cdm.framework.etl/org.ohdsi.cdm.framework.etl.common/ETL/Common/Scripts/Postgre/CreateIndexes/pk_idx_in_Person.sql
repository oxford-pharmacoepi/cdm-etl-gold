ALTER TABLE {sc}.person ADD CONSTRAINT xpk_person PRIMARY KEY (person_id);
CREATE UNIQUE INDEX IF NOT EXISTS idx_person_id ON {sc}.person (person_id ASC);
CLUSTER {sc}.person USING idx_person_id;
CREATE INDEX idx_gender ON {TARGET_SCHEMA}.person (gender_concept_id ASC);