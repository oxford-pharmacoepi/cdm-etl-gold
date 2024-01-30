ALTER TABLE {sc}.person ADD CONSTRAINT xpk_person PRIMARY KEY (person_id) USING INDEX TABLESPACE {tablespace};
CREATE UNIQUE INDEX IF NOT EXISTS idx_person_id ON {sc}.person (person_id ASC) TABLESPACE {tablespace};
CLUSTER {sc}.person USING idx_person_id;
CREATE INDEX idx_gender ON {sc}.person (gender_concept_id ASC) TABLESPACE {tablespace};