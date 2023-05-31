ALTER TABLE {sc}.observation_period ADD CONSTRAINT pk_observation_period PRIMARY KEY (observation_period_id);
CREATE INDEX IF NOT EXISTS idx_op_person_id ON {sc}.observation_period(person_id ASC);
CLUSTER {sc}.observation_period USING idx_op_person_id;