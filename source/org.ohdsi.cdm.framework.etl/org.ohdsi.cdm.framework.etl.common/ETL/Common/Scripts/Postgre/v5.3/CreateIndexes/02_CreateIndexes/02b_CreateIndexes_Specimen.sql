CREATE INDEX IF NOT EXISTS idx_specimen_person_id  ON specimen  (person_id ASC);
CLUSTER specimen  USING idx_specimen_person_id ;
CREATE INDEX IF NOT EXISTS idx_specimen_concept_id ON specimen (specimen_concept_id ASC);