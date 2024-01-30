ALTER TABLE {sc}.observation_period ADD CONSTRAINT xpk_observation_period PRIMARY KEY (observation_period_id) USING INDEX TABLESPACE {tablespace};
CREATE INDEX IF NOT EXISTS idx_observation_period_id ON {sc}.observation_period (person_id ASC) TABLESPACE {tablespace};
CLUSTER {sc}.observation_period USING idx_observation_period_id;