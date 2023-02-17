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