ALTER TABLE {sc}.person ADD CONSTRAINT xpk_person PRIMARY KEY (person_id);
CLUSTER {sc}.person USING xpk_person;