ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_person FOREIGN KEY (person_id)  REFERENCES person (person_id);
--AD: Did not run
--ERROR:  insert or update on table "visit_occurrence" violates foreign key constraint "fpk_visit_person"
--DETAIL:  Key (person_id)=(120018) is not present in table "person".
--AD: There were records for 7,842,465 patients not acceptable and NOT present in table "Person" - DELETED

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_concept FOREIGN KEY (visit_concept_id) REFERENCES vocabulary.concept (concept_id); 
ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_type_concept FOREIGN KEY (visit_type_concept_id)  REFERENCES vocabulary.concept (concept_id); 
ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id); 
ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_care_site FOREIGN KEY (care_site_id)  REFERENCES care_site (care_site_id); 
ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_concept_s FOREIGN KEY (visit_source_concept_id)  REFERENCES vocabulary.concept (concept_id); 
ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_discharge FOREIGN KEY (discharge_to_concept_id) REFERENCES vocabulary.concept (concept_id);
ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_preceding FOREIGN KEY (preceding_visit_occurrence_id) REFERENCES visit_occurrence (visit_occurrence_id); 

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

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_person FOREIGN KEY (person_id)  REFERENCES person (person_id);
ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_concept FOREIGN KEY (procedure_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_type_concept FOREIGN KEY (procedure_type_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_modifier FOREIGN KEY (modifier_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);
ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);
ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);
ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_concept_s FOREIGN KEY (procedure_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_person FOREIGN KEY (person_id)  REFERENCES person (person_id);
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_concept FOREIGN KEY (drug_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_type_concept FOREIGN KEY (drug_type_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_route_concept FOREIGN KEY (route_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);
--The following was SUSPENDED FOR VACCINE DATA ^^
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id); 
--The following was SUSPENDED FOR VACCINE DATA ^^
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_concept_s FOREIGN KEY (drug_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_person FOREIGN KEY (person_id)  REFERENCES person (person_id);
ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_concept FOREIGN KEY (device_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_type_concept FOREIGN KEY (device_type_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);
ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);
ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);
ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_concept_s FOREIGN KEY (device_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_person FOREIGN KEY (person_id)  REFERENCES person (person_id);
ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_concept FOREIGN KEY (condition_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_type_concept FOREIGN KEY (condition_type_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_status_concept FOREIGN KEY (condition_status_concept_id) REFERENCES vocabulary.concept (concept_id);
ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);
ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);
ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);
ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_concept_s FOREIGN KEY (condition_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_person FOREIGN KEY (person_id)  REFERENCES person (person_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_concept FOREIGN KEY (measurement_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_type_concept FOREIGN KEY (measurement_type_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_operator FOREIGN KEY (operator_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_value FOREIGN KEY (value_as_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_unit FOREIGN KEY (unit_concept_id)  REFERENCES vocabulary.concept (concept_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);
ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_concept_s FOREIGN KEY (measurement_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE note ADD CONSTRAINT fpk_note_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_type_concept FOREIGN KEY (note_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_class_concept FOREIGN KEY (note_class_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_encoding_concept FOREIGN KEY (encoding_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note ADD CONSTRAINT fpk_language_concept FOREIGN KEY (language_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);


ALTER TABLE note_nlp ADD CONSTRAINT fpk_note_nlp_note FOREIGN KEY (note_id) REFERENCES note (note_id);

ALTER TABLE note_nlp ADD CONSTRAINT fpk_note_nlp_section_concept FOREIGN KEY (section_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note_nlp ADD CONSTRAINT fpk_note_nlp_concept FOREIGN KEY (note_nlp_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note_nlp ADD CONSTRAINT fpk_note_nlp_concept_s FOREIGN KEY (note_nlp_source_concept_id) REFERENCES vocabulary.concept (concept_id);


ALTER TABLE observation ADD CONSTRAINT fpk_observation_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE observation ADD CONSTRAINT fpk_observation_concept FOREIGN KEY (observation_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE observation ADD CONSTRAINT fpk_observation_type_concept FOREIGN KEY (observation_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE observation ADD CONSTRAINT fpk_observation_value FOREIGN KEY (value_as_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE observation ADD CONSTRAINT fpk_observation_qualifier FOREIGN KEY (qualifier_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE observation ADD CONSTRAINT fpk_observation_unit FOREIGN KEY (unit_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE observation ADD CONSTRAINT fpk_observation_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

ALTER TABLE observation ADD CONSTRAINT fpk_observation_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);

ALTER TABLE observation ADD CONSTRAint fpk_observation_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);

ALTER TABLE observation ADD CONSTRAINT fpk_observation_concept_s FOREIGN KEY (observation_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


/************************

Standardized health system data

************************/


ALTER TABLE care_site ADD CONSTRAINT fpk_care_site_place FOREIGN KEY (place_of_service_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE care_site ADD CONSTRAINT fpk_care_site_location FOREIGN KEY (location_id)  REFERENCES location (location_id);


ALTER TABLE provider ADD CONSTRAINT fpk_provider_specialty FOREIGN KEY (specialty_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE provider ADD CONSTRAINT fpk_provider_care_site FOREIGN KEY (care_site_id)  REFERENCES care_site (care_site_id);

ALTER TABLE provider ADD CONSTRAINT fpk_provider_gender FOREIGN KEY (gender_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE provider ADD CONSTRAINT fpk_provider_specialty_s FOREIGN KEY (specialty_source_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE provider ADD CONSTRAINT fpk_provider_gender_s FOREIGN KEY (gender_source_concept_id)  REFERENCES vocabulary.concept (concept_id);

