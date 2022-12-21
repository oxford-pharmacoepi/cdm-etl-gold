ALTER TABLE {sc}.chunk_person ADD CONSTRAINT pk_chunk_person PRIMARY KEY (chunk_id, person_id, temp_person_id);
-- CREATE UNIQUE INDEX idx_chunk_person_id ON {sc}.chunk_person (chunk_id, person_id ASC);
CREATE UNIQUE INDEX idx_chunk_person_person_id ON {sc}.chunk_person (person_id ASC);
CREATE UNIQUE INDEX idx_chunk_person_temp_person_id ON {sc}.chunk_person (temp_person_id ASC);
ALTER TABLE {sc}.chunk ADD CONSTRAINT pk_chunk PRIMARY KEY (chunk_id);
CREATE INDEX idx_chunk_completed ON {sc}.chunk (completed);
CLUSTER {sc}.chunk_person USING pk_chunk_person;
CLUSTER {sc}.chunk USING pk_chunk;



