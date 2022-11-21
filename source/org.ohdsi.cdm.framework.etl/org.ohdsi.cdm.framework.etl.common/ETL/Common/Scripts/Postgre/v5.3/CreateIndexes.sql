/************************

Standardized clinical data

************************/


/**PRIMARY KEY NONCLUSTERED constraints**/

ALTER TABLE person ADD CONSTRAINT xpk_person PRIMARY KEY ( person_id ) ;

ALTER TABLE observation_period ADD CONSTRAINT xpk_observation_period PRIMARY KEY ( observation_period_id ) ;

ALTER TABLE specimen ADD CONSTRAINT xpk_specimen PRIMARY KEY ( specimen_id ) ;

ALTER TABLE visit_occurrence ADD CONSTRAINT xpk_visit_occurrence PRIMARY KEY ( visit_occurrence_id ) ;
--AD: It seems a PK is already present

ALTER TABLE visit_detail ADD CONSTRAINT xpk_visit_detail PRIMARY KEY ( visit_detail_id ) ;
--AD: It seems a PK is already present

ALTER TABLE procedure_occurrence ADD CONSTRAINT xpk_procedure_occurrence PRIMARY KEY ( procedure_occurrence_id ) ;

ALTER TABLE drug_exposure ADD CONSTRAINT xpk_drug_exposure PRIMARY KEY ( drug_exposure_id ) ;

ALTER TABLE device_exposure ADD CONSTRAINT xpk_device_exposure PRIMARY KEY ( device_exposure_id ) ;

ALTER TABLE condition_occurrence ADD CONSTRAINT xpk_condition_occurrence PRIMARY KEY ( condition_occurrence_id ) ;

ALTER TABLE measurement ADD CONSTRAINT xpk_measurement PRIMARY KEY ( measurement_id ) ;

ALTER TABLE note ADD CONSTRAINT xpk_note PRIMARY KEY ( note_id ) ;

ALTER TABLE note_nlp ADD CONSTRAINT xpk_note_nlp PRIMARY KEY ( note_nlp_id ) ;

ALTER TABLE observation  ADD CONSTRAINT xpk_observation PRIMARY KEY ( observation_id ) ;


/************************

Standardized health system data

************************/


ALTER TABLE location ADD CONSTRAINT xpk_location PRIMARY KEY ( location_id ) ;

ALTER TABLE care_site ADD CONSTRAINT xpk_care_site PRIMARY KEY ( care_site_id ) ;

ALTER TABLE provider ADD CONSTRAINT xpk_provider PRIMARY KEY ( provider_id ) ;


/************************

Standardized clinical data

************************/

CREATE UNIQUE INDEX idx_person_id  ON person  (person_id ASC);
CLUSTER person  USING idx_person_id ;

CREATE INDEX idx_observation_period_id  ON observation_period  (person_id ASC);
CLUSTER observation_period  USING idx_observation_period_id ;

CREATE INDEX idx_specimen_person_id  ON specimen  (person_id ASC);
CLUSTER specimen  USING idx_specimen_person_id ;
CREATE INDEX idx_specimen_concept_id ON specimen (specimen_concept_id ASC);

CREATE INDEX idx_visit_person_id  ON visit_occurrence  (person_id ASC);
CLUSTER visit_occurrence  USING idx_visit_person_id ;
CREATE INDEX idx_visit_concept_id ON visit_occurrence (visit_concept_id ASC); ADDED

-- Check from here on

CREATE INDEX idx_visit_detail_person_id  ON visit_detail  (person_id ASC);
--AD: It seems this idx is already present
--AD: LOST CONNECTION SEVERAL TIMES WHEN RUNNING THE FOLLOWING ROW
--However, I have checked the reported clusters and it is there
--CLUSTER visit_detail USING idx_visit_detail_person_id ;

CREATE INDEX idx_visit_detail_concept_id ON visit_detail (visit_detail_concept_id ASC);#ADDED

CREATE INDEX idx_procedure_person_id  ON procedure_occurrence  (person_id ASC);
CLUSTER procedure_occurrence  USING idx_procedure_person_id ;
CREATE INDEX idx_procedure_concept_id ON procedure_occurrence (procedure_concept_id ASC);
CREATE INDEX idx_procedure_visit_id ON procedure_occurrence (visit_occurrence_id ASC);

CREATE INDEX idx_drug_person_id  ON drug_exposure  (person_id ASC);
CLUSTER drug_exposure  USING idx_drug_person_id ;
CREATE INDEX idx_drug_concept_id ON drug_exposure (drug_concept_id ASC);
CREATE INDEX idx_drug_visit_id ON drug_exposure (visit_occurrence_id ASC);

CREATE INDEX idx_device_person_id  ON device_exposure  (person_id ASC);
CLUSTER device_exposure  USING idx_device_person_id ;
CREATE INDEX idx_device_concept_id ON device_exposure (device_concept_id ASC);
CREATE INDEX idx_device_visit_id ON device_exposure (visit_occurrence_id ASC);

CREATE INDEX idx_condition_person_id  ON condition_occurrence  (person_id ASC);
CLUSTER condition_occurrence  USING idx_condition_person_id ;
CREATE INDEX idx_condition_concept_id ON condition_occurrence (condition_concept_id ASC);
CREATE INDEX idx_condition_visit_id ON condition_occurrence (visit_occurrence_id ASC);

CREATE INDEX idx_measurement_person_id  ON measurement  (person_id ASC);
CLUSTER measurement  USING idx_measurement_person_id ;
CREATE INDEX idx_measurement_concept_id ON measurement (measurement_concept_id ASC);
CREATE INDEX idx_measurement_visit_id ON measurement (visit_occurrence_id ASC);

CREATE INDEX idx_note_person_id  ON note  (person_id ASC);
CLUSTER note  USING idx_note_person_id ;
CREATE INDEX idx_note_concept_id ON note (note_type_concept_id ASC);
CREATE INDEX idx_note_visit_id ON note (visit_occurrence_id ASC);

CREATE INDEX idx_note_nlp_note_id  ON note_nlp  (note_id ASC);
CLUSTER note_nlp  USING idx_note_nlp_note_id ;
CREATE INDEX idx_note_nlp_concept_id ON note_nlp (note_nlp_concept_id ASC);

CREATE INDEX idx_observation_person_id  ON observation  (person_id ASC);
--AD: LOST CONNECTION SEVERAL TIMES WHEN RUNNING THE FOLLOWING ROW
--However, I have checked the reported clusters and it is there
CLUSTER observation  USING idx_observation_person_id ;
CREATE INDEX idx_observation_concept_id ON observation (observation_concept_id ASC);
CREATE INDEX idx_observation_visit_id ON observation (visit_occurrence_id ASC);



/************************

Foreign key constraints

************************/

/************************

Standardized clinical data

************************/

ALTER TABLE person ADD CONSTRAINT fpk_person_gender_concept FOREIGN KEY (gender_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE person ADD CONSTRAINT fpk_person_race_concept FOREIGN KEY (race_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE person ADD CONSTRAINT fpk_person_ethnicity_concept FOREIGN KEY (ethnicity_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE person ADD CONSTRAINT fpk_person_gender_concept_s FOREIGN KEY (gender_source_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE person ADD CONSTRAINT fpk_person_race_concept_s FOREIGN KEY (race_source_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE person ADD CONSTRAINT fpk_person_ethnicity_concept_s FOREIGN KEY (ethnicity_source_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE person ADD CONSTRAINT fpk_person_location FOREIGN KEY (location_id)  REFERENCES location (location_id);

ALTER TABLE person ADD CONSTRAINT fpk_person_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

ALTER TABLE person ADD CONSTRAINT fpk_person_care_site FOREIGN KEY (care_site_id)  REFERENCES care_site (care_site_id);


ALTER TABLE observation_period ADD CONSTRAINT fpk_observation_period_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE observation_period ADD CONSTRAINT fpk_observation_period_concept FOREIGN KEY (period_type_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE specimen ADD CONSTRAINT fpk_specimen_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE specimen ADD CONSTRAINT fpk_specimen_concept FOREIGN KEY (specimen_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE specimen ADD CONSTRAINT fpk_specimen_type_concept FOREIGN KEY (specimen_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE specimen ADD CONSTRAINT fpk_specimen_unit_concept FOREIGN KEY (unit_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE specimen ADD CONSTRAINT fpk_specimen_site_concept FOREIGN KEY (anatomic_site_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE specimen ADD CONSTRAINT fpk_specimen_status_concept FOREIGN KEY (disease_status_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_person FOREIGN KEY (person_id)  REFERENCES person (person_id); #ADDED
--AD: Did not run
--ERROR:  insert or update on table "visit_occurrence" violates foreign key constraint "fpk_visit_person"
--DETAIL:  Key (person_id)=(120018) is not present in table "person".
--AD: There were records for 7,842,465 patients not acceptable and NOT present in table "Person" - DELETED

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_concept FOREIGN KEY (visit_concept_id) REFERENCES vocabulary.concept (concept_id); # ADDED

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_type_concept FOREIGN KEY (visit_type_concept_id)  REFERENCES vocabulary.concept (concept_id); ADDED

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id); ADDED

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_care_site FOREIGN KEY (care_site_id)  REFERENCES care_site (care_site_id); ADDED

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_concept_s FOREIGN KEY (visit_source_concept_id)  REFERENCES vocabulary.concept (concept_id); ADDED

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_discharge FOREIGN KEY (discharge_to_concept_id) REFERENCES vocabulary.concept (concept_id);ADDED

ALTER TABLE visit_occurrence ADD CONSTRAINT fpk_visit_preceding FOREIGN KEY (preceding_visit_occurrence_id) REFERENCES visit_occurrence (visit_occurrence_id); ADDED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_person FOREIGN KEY (person_id)  REFERENCES person (person_id); ADDED
--AD: Did not run
--ERROR:  insert or update on table "visit_detail" violates foreign key constraint "fpk_v_detail_person"
--DETAIL:  Key (person_id)=(120018) is not present in table "person".
--SQL state: 23503
--AD: There were records for 7,842,465 patients not acceptable and NOT present in table "Person" - DELETED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_concept FOREIGN KEY (visit_detail_concept_id) REFERENCES vocabulary.concept (concept_id); ADDED
--AD: LOST CONNECTION SEVERAL TIMES WHEN RUNNING THE FOLLOWING ROW
--However, I have checked the FOREIGN KEY is there

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_type_concept FOREIGN KEY (visit_detail_type_concept_id)  REFERENCES vocabulary.concept (concept_id);ADDED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);ADDED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_care_site FOREIGN KEY (care_site_id)  REFERENCES care_site (care_site_id);ADDED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_discharge FOREIGN KEY (discharge_to_concept_id) REFERENCES vocabulary.concept (concept_id);ADDED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_concept_s FOREIGN KEY (visit_detail_source_concept_id)  REFERENCES vocabulary.concept (concept_id);ADDED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_preceding FOREIGN KEY (preceding_visit_detail_id) REFERENCES visit_detail (visit_detail_id);ADDED

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_parent FOREIGN KEY (visit_detail_parent_id) REFERENCES visit_detail (visit_detail_id);ADDED
--AD: DID NOT RUN
--ERROR:  insert or update on table "visit_detail" violates foreign key constraint "fpk_v_detail_parent"
--DETAIL:  Key (visit_detail_parent_id)=(9058863325) is not present in table "visit_detail".
--SQL state: 23503

ALTER TABLE visit_detail ADD CONSTRAINT fpk_v_detail_visit FOREIGN KEY (visit_occurrence_id) REFERENCES visit_occurrence (visit_occurrence_id);ADDED

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_concept FOREIGN KEY (procedure_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_type_concept FOREIGN KEY (procedure_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_modifier FOREIGN KEY (modifier_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);ADDED

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);ADDED

ALTER TABLE procedure_occurrence ADD CONSTRAINT fpk_procedure_concept_s FOREIGN KEY (procedure_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_concept FOREIGN KEY (drug_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_type_concept FOREIGN KEY (drug_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_route_concept FOREIGN KEY (route_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

--The following was SUSPENDED FOR VACCINE DATA ^^
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id); ADDED

--The following was SUSPENDED FOR VACCINE DATA ^^
ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);ADDED

ALTER TABLE drug_exposure ADD CONSTRAINT fpk_drug_concept_s FOREIGN KEY (drug_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_concept FOREIGN KEY (device_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_type_concept FOREIGN KEY (device_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);ADDED

ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);ADDED

ALTER TABLE device_exposure ADD CONSTRAINT fpk_device_concept_s FOREIGN KEY (device_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_concept FOREIGN KEY (condition_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_type_concept FOREIGN KEY (condition_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_status_concept FOREIGN KEY (condition_status_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);ADDED

ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);ADDED

ALTER TABLE condition_occurrence ADD CONSTRAINT fpk_condition_concept_s FOREIGN KEY (condition_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_concept FOREIGN KEY (measurement_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_type_concept FOREIGN KEY (measurement_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_operator FOREIGN KEY (operator_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_value FOREIGN KEY (value_as_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_unit FOREIGN KEY (unit_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);ADDED

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);ADDED

ALTER TABLE measurement ADD CONSTRAINT fpk_measurement_concept_s FOREIGN KEY (measurement_source_concept_id)  REFERENCES vocabulary.concept (concept_id);


ALTER TABLE note ADD CONSTRAINT fpk_note_person FOREIGN KEY (person_id)  REFERENCES person (person_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_type_concept FOREIGN KEY (note_type_concept_id)  REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_class_concept FOREIGN KEY (note_class_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_encoding_concept FOREIGN KEY (encoding_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note ADD CONSTRAINT fpk_language_concept FOREIGN KEY (language_concept_id) REFERENCES vocabulary.concept (concept_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_provider FOREIGN KEY (provider_id)  REFERENCES provider (provider_id);

ALTER TABLE note ADD CONSTRAINT fpk_note_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);ADDED

ALTER TABLE note ADD CONSTRAINT fpk_note_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);ADDED


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

ALTER TABLE observation ADD CONSTRAINT fpk_observation_visit FOREIGN KEY (visit_occurrence_id)  REFERENCES visit_occurrence (visit_occurrence_id);ADDED

ALTER TABLE observation ADD CONSTRAint fpk_observation_v_detail FOREIGN KEY (visit_detail_id) REFERENCES visit_detail (visit_detail_id);ADDED

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

