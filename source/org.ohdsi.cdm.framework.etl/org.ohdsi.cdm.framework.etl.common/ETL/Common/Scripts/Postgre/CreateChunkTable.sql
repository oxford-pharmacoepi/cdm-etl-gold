DROP SEQUENCE IF EXISTS {sc}.visit_detail_id_seq;
CREATE SEQUENCE {sc}.visit_detail_id_seq START WITH 1 INCREMENT BY 1 NO MAXVALUE CACHE 1;
DROP TABLE IF EXISTS {sc}.chunk_person;
CREATE TABLE {sc}.chunk_person TABLESPACE {tablespace} AS 
select (floor((row_number() over (order by person_id)-1)/{CHUNK_SIZE}) + 1)::int as chunk_id, person_id
FROM {des}.Person
order by chunk_id, person_id;
DROP TABLE IF EXISTS {sc}.chunk;
CREATE TABLE {sc}.chunk TABLESPACE {tablespace} AS 
SELECT distinct chunk_id, 0::smallint as completed FROM {sc}.chunk_person;