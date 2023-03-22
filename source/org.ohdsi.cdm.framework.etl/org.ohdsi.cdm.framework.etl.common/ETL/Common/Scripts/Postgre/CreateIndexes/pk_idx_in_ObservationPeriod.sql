ALTER TABLE {sc}.observation_period ADD PRIMARY KEY (observation_period_id);
CREATE INDEX IF NOT EXISTS idx_observation_period_pid ON {sc}.observation_period(person_id ASC);
CLUSTER {sc}.observation_period USING idx_observation_period_pid;