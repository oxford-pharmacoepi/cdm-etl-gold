ALTER TABLE {sc}.chunk_person ADD CONSTRAINT pk_chunk_person PRIMARY KEY (chunk_id, person_id) USING INDEX TABLESPACE {tablespace};
CREATE UNIQUE INDEX idx_chunk_person_person_id ON {sc}.chunk_person (person_id ASC) TABLESPACE {tablespace};
ALTER TABLE {sc}.chunk ADD CONSTRAINT pk_chunk PRIMARY KEY (chunk_id) USING INDEX TABLESPACE {tablespace};
CREATE INDEX idx_chunk_completed ON {sc}.chunk (completed) TABLESPACE {tablespace};
CLUSTER {sc}.chunk_person USING pk_chunk_person;
CLUSTER {sc}.chunk USING pk_chunk;