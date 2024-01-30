ALTER TABLE {sc}.visit_occurrence ADD CONSTRAINT xpk_visit_occurrence PRIMARY KEY ( visit_occurrence_id ) USING INDEX TABLESPACE {tablespace};
CREATE INDEX idx_visit_person_id  ON {sc}.visit_occurrence  (person_id ASC) TABLESPACE {tablespace};
CLUSTER {sc}.visit_occurrence  USING idx_visit_person_id ;
CREATE INDEX idx_visit_concept_id ON {sc}.visit_occurrence (visit_concept_id ASC) TABLESPACE {tablespace};