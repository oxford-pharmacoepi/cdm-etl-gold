CREATE INDEX IF NOT EXISTS idx_note_nlp_note_id  ON note_nlp  (note_id ASC);
CLUSTER note_nlp  USING idx_note_nlp_note_id ;
CREATE INDEX IF NOT EXISTS idx_note_nlp_concept_id ON note_nlp (note_nlp_concept_id ASC);

CREATE INDEX IF NOT EXISTS idx_observation_person_id  ON observation  (person_id ASC);
--AD: LOST CONNECTION SEVERAL TIMES WHEN RUNNING THE FOLLOWING ROW
--However, I have checked the reported clusters and it is there
CLUSTER observation  USING idx_observation_person_id ;
CREATE INDEX IF NOT EXISTS idx_observation_concept_id ON observation (observation_concept_id ASC);
CREATE INDEX IF NOT EXISTS idx_observation_visit_id ON observation (visit_occurrence_id ASC);