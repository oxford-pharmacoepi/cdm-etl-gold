do
$$
declare
	chunk record;
    n record;
	vd record;
begin
	for chunk in select person_id from person order by person_id
	loop
		RAISE NOTICE 'person %', chunk.person_id;
		
		for n in With a AS(
					select a.visit_detail_id, a.person_id, a.visit_detail_start_date, a.preceding_visit_detail_id, 
					row_number() over (ORDER BY a.person_id, a.visit_detail_start_date, a.visit_detail_id
					) as row
					from visit_detail a 
					where a.person_id = chunk.person_id
				), b AS(
					select b.visit_detail_id, b.person_id, b.visit_detail_start_date, b.preceding_visit_detail_id, 
					row_number() over (
						ORDER BY b.person_id, b.visit_detail_start_date, b.visit_detail_id
					)-1 as row
					from visit_detail b 
					where b.person_id = chunk.person_id
				)
				select b.visit_detail_id, b.person_id, b.visit_detail_start_date, b.preceding_visit_detail_id, a.visit_detail_id as new_preceding_visit_detail_id, a.visit_detail_start_date as later_start_date
				from b
				left join a on a.row = b.row
		loop

		select INTO vd * from visit_detail 
		where visit_detail_id = n.visit_detail_id
		and COALESCE (preceding_visit_detail_id, -99) != COALESCE (n.new_preceding_visit_detail_id, -99);

		IF FOUND THEN
			UPDATE visit_detail
			SET preceding_visit_detail_id = n.new_preceding_visit_detail_id
			WHERE visit_detail_id = n.visit_detail_id;
			RAISE NOTICE 'visit_detail % found %, %', n.visit_detail_id, n.preceding_visit_detail_id, n.new_preceding_visit_detail_id;
		END IF;

		end loop;
		
	end loop;
	commit;
end;
$$;