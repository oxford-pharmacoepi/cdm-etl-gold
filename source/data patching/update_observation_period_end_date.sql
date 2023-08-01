-- update observation_period_end_date

do
$$
declare
	r record;
begin
	for r in 	select op.observation_period_id, op.person_id, op.observation_period_end_date,
				LEAST(a.tod, b.lcd, a.deathdate, to_date(CONCAT(RIGHT(current_database(), 6), '01'), 'YYYYMMDD')) as observation_period_end_date_new
				from observation_period op
				join source.patient a on a.patid = op.person_id
				join source.practice b on MOD(a.patid, 100000) = b.pracid
				where op.observation_period_end_date != LEAST(a.tod, b.lcd, a.deathdate, to_date(CONCAT(RIGHT(current_database(), 6), '01'), 'YYYYMMDD'))
				-- limit 10
	
	loop
		-- RAISE NOTICE 'observation_period_id % found %, %', r.observation_period_id, r.observation_period_end_date, r.observation_period_end_date_new;
		UPDATE observation_period
		SET observation_period_end_date = r.observation_period_end_date_new
		where observation_period_id = r.observation_period_id;
		
	end loop;
end;
$$;