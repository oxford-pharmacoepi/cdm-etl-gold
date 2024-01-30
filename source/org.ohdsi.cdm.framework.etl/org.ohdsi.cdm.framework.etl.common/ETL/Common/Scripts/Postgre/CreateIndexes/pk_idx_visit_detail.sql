ALTER TABLE {sc}.visit_detail ADD CONSTRAINT xpk_visit_detail PRIMARY KEY ( visit_detail_id ) USING INDEX TABLESPACE {tablespace};
CREATE INDEX idx_visit_detail_person_id  ON {sc}.visit_detail  (person_id ASC) TABLESPACE {tablespace};
CLUSTER {sc}.visit_detail USING idx_visit_detail_person_id ;
CREATE INDEX idx_visit_detail_concept_id ON {sc}.visit_detail (visit_detail_concept_id ASC) TABLESPACE {tablespace};
CREATE INDEX idx_visit_det_occ_id ON {sc}.visit_detail (visit_occurrence_id ASC) TABLESPACE {tablespace};