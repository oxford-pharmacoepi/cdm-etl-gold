CREATE UNIQUE INDEX IF NOT EXISTS idx_person_id  ON person  (person_id ASC);
CLUSTER person  USING idx_person_id ;