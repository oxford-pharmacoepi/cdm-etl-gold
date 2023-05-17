ALTER TABLE {sc}.person ADD CONSTRAINT pk_person PRIMARY KEY (person_id);
CLUSTER {sc}.person USING pk_person;