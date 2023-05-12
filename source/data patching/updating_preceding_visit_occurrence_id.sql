do
$$
declare
	chunk record;
    n record;
	vo record;
begin
	for chunk in select person_id from person order by person_id
	loop
		RAISE NOTICE 'person_id %', chunk.person_id;
		
		for n in With a AS(
					select a.visit_occurrence_id, a.person_id, a.visit_start_date, a.preceding_visit_occurrence_id, 
					row_number() over (ORDER BY a.person_id, a.visit_start_date
					) as row
					from visit_occurrence a 
					where a.person_id = chunk.person_id
				), b AS(
					select b.visit_occurrence_id, b.person_id, b.visit_start_date, b.preceding_visit_occurrence_id, 
					row_number() over (
						ORDER BY b.person_id, b.visit_start_date
					)-1 as row
					from visit_occurrence b
					where b.person_id = chunk.person_id
				)
				select b.visit_occurrence_id, b.person_id, b.visit_start_date, b.preceding_visit_occurrence_id, a.visit_occurrence_id as new_preceding_visit_occurrence_id, a.visit_start_date as later_start_date
				from b
				left join a on a.row = b.row
		loop

		select INTO vo * from visit_occurrence 
		where visit_occurrence_id = n.visit_occurrence_id
		and COALESCE (preceding_visit_occurrence_id, -99) != COALESCE (n.new_preceding_visit_occurrence_id, -99);

		IF FOUND THEN
			UPDATE visit_occurrence
			SET preceding_visit_occurrence_id = n.new_preceding_visit_occurrence_id
			WHERE visit_occurrence_id = n.visit_occurrence_id;
			-- RAISE NOTICE 'visit_occurrence % found %, %', n.visit_occurrence_id, n.preceding_visit_occurrence_id, n.new_preceding_visit_occurrence_id;
		END IF;

		end loop;
		
	end loop;
end;
$$;