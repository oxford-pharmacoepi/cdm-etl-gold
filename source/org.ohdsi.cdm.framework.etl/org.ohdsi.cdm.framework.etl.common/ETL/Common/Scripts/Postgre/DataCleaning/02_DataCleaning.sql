DROP PROCEDURE IF EXISTS pr_DataCleaning(in_TableName varchar) CASCADE;

GO

CREATE PROCEDURE pr_DataCleaning(
	in_TableName varchar
)
LANGUAGE plpgsql AS
$proc$
DECLARE
	dropTableSql varchar;
	createTableSql varchar;
	insertSql varchar;
	deletesql varchar;
	temp varchar;
BEGIN

	in_TableName := LOWER(NULLIF(in_TableName, ''));

	-- Step 1: Create {sc}_clean schema
	CREATE SCHEMA IF NOT EXISTS source_nok;
	
	-- Step 2: 	Drop Table If Exists
	-- 			Create Table and Insert Redudant Data to {SOURCE_NOK_SCHEMA}
	--			Delete Redudant Data in {sc}
	
	dropTableSql := 'DROP TABLE IF EXISTS {SOURCE_NOK_SCHEMA}.' || in_TableName || ' CASCADE';
	createTableSql := 'CREATE TABLE {SOURCE_NOK_SCHEMA}.' || in_TableName || ' (LIKE {sc}.' || in_TableName || ')';

	EXECUTE dropTableSql;
	EXECUTE createTableSql;
	
	IF('patient' = in_TableName) THEN	
		
		With t as (
						SELECT DISTINCT patid 
						FROM {sc}.patient 
						where accept = 0 
						OR (gender is null or gender in (3,4)) 
						OR (case when yob < 1000 then 1800 + yob else yob end) < 1875 
						OR (deathdate is not null AND deathdate <  frd) 
				)
				INSERT INTO {SOURCE_NOK_SCHEMA}.patient 
				SELECT t1.* FROM {sc}.patient as t1 
				INNER JOIN t on t.patid = t1.patid;
					
		alter table {SOURCE_NOK_SCHEMA}.patient add constraint pk_patient_nok primary key (patid);
		
		DELETE FROM {sc}.patient as t1 
		using {SOURCE_NOK_SCHEMA}.patient as t2 
		WHERE t1.patid = t2.patid;
					
	ELSE
	
		-- Eliminate records belong to invalid patients
		insertSql := 	'INSERT INTO {SOURCE_NOK_SCHEMA}.' || in_TableName || 
						' SELECT t1.* FROM {sc}.' || in_TableName || ' as t1' ||
						' INNER JOIN {SOURCE_NOK_SCHEMA}.patient as t2 on t2.patid = t1.patid';
						
		EXECUTE insertSql;
		
		deleteSql := 	'DELETE FROM {sc}.' || in_TableName || ' as t1 '
						'using {SOURCE_NOK_SCHEMA}.' || in_TableName ||' as t2 '
						'WHERE t1.id = t2.id';
		
		EXECUTE deleteSql;
	
	END IF;

	-- For consultation, clinical, referral, immunisation, test, therapy, remove eventdate is NULL
	IF(in_TableName IN ('consultation', 'clinical', 'referral', 'immunisation', 'test', 'therapy')) THEN		
	
		insertSql := 	'INSERT INTO {SOURCE_NOK_SCHEMA}.' || in_TableName || 
						' SELECT * from {sc}.' || in_TableName || ' where eventdate is NULL';		
						
		EXECUTE insertSql;

		deleteSql := 	'DELETE FROM {sc}.' || in_TableName || ' as t1 '
						'using {SOURCE_NOK_SCHEMA}.' || in_TableName ||' as t2 '
						'WHERE t1.id = t2.id';
		
		EXECUTE deleteSql;	
	END IF;

	-- For testing
	-- RAISE NOTICE 'FORCE ROLLBACK'; 		

EXCEPTION WHEN others THEN
	ROLLBACK;
	RAISE NOTICE 'Transaction ROLLBACK'; 
	RAISE EXCEPTION '% %', SQLERRM, SQLSTATE;
	
END;
$proc$;