DROP SEQUENCE IF EXISTS {sc}.temp_person_id_seq;

CREATE SEQUENCE {sc}.temp_person_id_seq
  START WITH 1
  INCREMENT BY 1
  MINVALUE 1
  NO MAXVALUE
  CACHE 1;

DROP TABLE IF EXISTS {sc}.chunk_person;

CREATE TABLE {sc}.chunk_person AS 
               select (floor((row_number() over (order by patid)-1)/{CHUNK_SIZE}) + 1)::int as chunk_id,
               patid as person_id,
			   nextval('{sc}.temp_person_id_seq'::regclass) as temp_person_id
               FROM {sc}.Patient
               order by chunk_id, person_id;

DROP TABLE IF EXISTS {sc}.chunk;

CREATE TABLE {sc}.chunk AS 
SELECT distinct chunk_id,
0::smallint as completed 
FROM {sc}.chunk_person;



