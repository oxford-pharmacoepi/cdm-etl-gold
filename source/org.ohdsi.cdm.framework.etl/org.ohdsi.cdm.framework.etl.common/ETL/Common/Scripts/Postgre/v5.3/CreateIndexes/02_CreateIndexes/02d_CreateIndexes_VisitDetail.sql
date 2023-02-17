CREATE INDEX IF NOT EXISTS idx_visit_detail_person_id  ON visit_detail  (person_id ASC);
--AD: It seems this idx is already present
--AD: LOST CONNECTION SEVERAL TIMES WHEN RUNNING THE FOLLOWING ROW
--However, I have checked the reported clusters and it is there
CLUSTER visit_detail USING idx_visit_detail_person_id ;
CREATE INDEX IF NOT EXISTS idx_visit_detail_concept_id ON visit_detail (visit_detail_concept_id ASC);