CREATE INDEX IF NOT EXISTS idx_condition_person_id  ON condition_occurrence  (person_id ASC);
CLUSTER condition_occurrence  USING idx_condition_person_id ;
CREATE INDEX IF NOT EXISTS idx_condition_concept_id ON condition_occurrence (condition_concept_id ASC);
CREATE INDEX IF NOT EXISTS idx_condition_visit_id ON condition_occurrence (visit_occurrence_id ASC);