CREATE INDEX IF NOT EXISTS idx_device_person_id  ON device_exposure  (person_id ASC);
CLUSTER device_exposure  USING idx_device_person_id ;
CREATE INDEX IF NOT EXISTS idx_device_concept_id ON device_exposure (device_concept_id ASC);
CREATE INDEX IF NOT EXISTS idx_device_visit_id ON device_exposure (visit_occurrence_id ASC);