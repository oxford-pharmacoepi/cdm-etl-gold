CREATE INDEX idx_note_person_id  ON note  (person_id ASC);
CLUSTER note  USING idx_note_person_id ;
CREATE INDEX idx_note_concept_id ON note (note_type_concept_id ASC);
CREATE INDEX idx_note_visit_id ON note (visit_occurrence_id ASC);