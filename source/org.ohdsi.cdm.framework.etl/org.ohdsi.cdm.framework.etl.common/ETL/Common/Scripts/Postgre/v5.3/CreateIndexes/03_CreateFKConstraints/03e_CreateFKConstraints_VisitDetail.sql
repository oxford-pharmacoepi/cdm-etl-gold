ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_person FOREIGN KEY (person_id)  REFERENCES person (person_id); 
--AD: Did not run
--ERROR:  insert or update on table "visit_detail" violates foreign key constraint "fpk_v_detail_person"
--DETAIL:  Key (person_id)=(120018) is not present in table "person".
--SQL state: 23503
--AD: There were records for 7,842,465 patients not acceptable and NOT present in table "Person" - DELETED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_concept FOREIGN KEY (visit_detail_concept_id) REFERENCES vocabulary.concept (concept_id); 
--AD: LOST CONNECTION SEVERAL TIMES WHEN RUNNING THE FOLLOWING ROW
--However, I have checked the FOREIGN KEY is there
ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_type_concept FOREIGN KEY (visit_detail_type_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);
ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_care_site FOREIGN KEY (care_site_id)  REFERENCES care_site (care_site_id);
ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_discharge FOREIGN KEY (discharge_to_concept_id) REFERENCES vocabulary.concept (concept_id);
ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_concept_s FOREIGN KEY (visit_detail_source_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_preceding FOREIGN KEY (preceding_visit_detail_id) REFERENCES visit_detail (visit_detail_id);
ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_parent FOREIGN KEY (visit_detail_parent_id) REFERENCES visit_detail (visit_detail_id);
--AD: DID NOT RUN
--ERROR:  insert or update on table "visit_detail" violates foreign key constraint "fpk_v_detail_parent"
--DETAIL:  Key (visit_detail_parent_id)=(9058863325) is not present in table "visit_detail".
--SQL state: 23503
ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_visit FOREIGN KEY (visit_occurrence_id) REFERENCES visit_occurrence (visit_occurrence_id);