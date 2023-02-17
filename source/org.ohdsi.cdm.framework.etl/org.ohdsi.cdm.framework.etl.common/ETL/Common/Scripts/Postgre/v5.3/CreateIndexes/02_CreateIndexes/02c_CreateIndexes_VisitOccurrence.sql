CREATE INDEX IF NOT EXISTS idx_visit_person_id  ON visit_occurrence  (person_id ASC);
CLUSTER visit_occurrence  USING idx_visit_person_id ;
CREATE INDEX IF NOT EXISTS idx_visit_concept_id ON visit_occurrence (visit_concept_id ASC); 