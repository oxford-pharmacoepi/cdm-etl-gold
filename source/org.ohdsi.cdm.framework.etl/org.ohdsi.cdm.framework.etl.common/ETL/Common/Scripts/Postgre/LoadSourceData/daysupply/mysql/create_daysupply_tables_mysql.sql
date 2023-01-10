# -------------------------------------
# 1. DAYSUPPLY_MODES
# -------------------------------------
# This code works for MySQL < 8.0
# NB: Therapy and commondosages needs to have the right indexes
# Add PK to tables below
 
CREATE TABLE IF NOT EXISTS daysupply_modes
(
	prodcode INT UNSIGNED NOT NULL,
	numdays SMALLINT UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_cs;

SET @row_number = 0; 
SET @prodcode = 0;
INSERT INTO daysupply_modes
SELECT b.prodcode, b.numdays AS dayssuppl
	FROM (SELECT 
		@row_number := CASE WHEN @prodcode = prodcode THEN @row_number + 1 ELSE 1 END AS RowNumber,
		@prodcode := a.prodcode as prodcode, 
        a.numdays, a.daycount
			FROM (SELECT prodcode, numdays, COUNT(patid) AS daycount
				FROM therapy
				WHERE numdays > 0 AND numdays <= 365 AND prodcode > 1
				GROUP BY prodcode, numdays
				) a
				ORDER BY a.prodcode ASC, a.daycount DESC, a.numdays ASC
		) b
	WHERE b.RowNumber = 1;

ALTER TABLE daysupply_modes
ADD PRIMARY KEY (`prodcode`,`numdays`);


# -------------------------------------
# 2. DAYSUPPLY_DECODES
# -------------------------------------
CREATE TABLE IF NOT EXISTS daysupply_decodes
(
	prodcode INT UNSIGNED NOT NULL,
	daily_dose DECIMAL(15,3) UNSIGNED NOT NULL,
	qty DECIMAL(9,2) UNSIGNED NOT NULL,
	numpacks INT UNSIGNED NOT NULL,
	numdays SMALLINT UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_cs;

SET @row_number = 0; 
SET @prodcode = 0;
SET @daily_dose = 0; 
SET @qty = 0;
SET @numpacks = 0;
INSERT INTO daysupply_decodes
SELECT b.prodcode, b.daily_dose, b.qty, b.numpacks, b.numdays
	FROM (SELECT  
		@row_number := CASE WHEN @prodcode = prodcode AND @daily_dose = daily_dose AND @qty = qty AND @numpacks = numpacks
		THEN @row_number + 1 ELSE 1 END AS RowNumber,
		@prodcode := a.prodcode as prodcode, 
		@daily_dose := a.daily_dose as daily_dose,
		@qty := a.qty as qty,
		@numpacks := a.numpacks as numpacks,
		a.numdays
		FROM (SELECT t.prodcode,
					CASE WHEN cd.daily_dose IS NULL THEN 0 ELSE cd.daily_dose END AS daily_dose,
					CASE WHEN t.qty IS NULL THEN 0 ELSE t.qty END AS qty,
					CASE WHEN t.numpacks IS NULL THEN 0 ELSE t.numpacks END AS numpacks,
					t.numdays, COUNT(t.prodcode) AS daycount
				FROM therapy as t
				left join commondosages as cd on t.dosageid = cd.dosageid
				WHERE t.numdays > 0 AND t.numdays <= 365 AND t.prodcode > 1
				GROUP BY t.prodcode,
					CASE WHEN cd.daily_dose IS NULL THEN 0 ELSE cd.daily_dose END,
					CASE WHEN t.qty IS NULL THEN 0 ELSE t.qty END,
					CASE WHEN t.numpacks IS NULL THEN 0 ELSE t.numpacks END,
					t.numdays
			) a
			ORDER BY a.prodcode ASC, a.daily_dose ASC, a.qty ASC, a.numpacks ASC, a.daycount DESC, a.numdays ASC
	) b
	WHERE b.RowNumber = 1;
# -------------------------------------
# Add primary key
# -------------------------------------
ALTER TABLE daysupply_decodes
ADD PRIMARY KEY (`prodcode`,`daily_dose`,`qty`,`numpacks`,`numdays`);

