CREATE TABLE IF NOT EXISTS {sc}.care_site
(
   care_site_id                   integer         NOT NULL,
   care_site_name                 varchar(255),
   place_of_service_concept_id    integer,
   location_id                    integer,
   care_site_source_value         varchar(50),
   place_of_service_source_value  varchar(50)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.cdm_source
(
   cdm_source_name                 varchar(255) NOT NULL,
   cdm_source_abbreviation         varchar(25),
   cdm_holder                      varchar(255),
   source_description              text,
   source_documentation_reference  varchar(255),
   cdm_etl_reference               varchar(255),
   source_release_date             date,
   cdm_release_date                date,
   cdm_version                     varchar(10),
   vocabulary_version              varchar(20)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.condition_era
(
   condition_era_id            BIGSERIAL NOT NULL,
   person_id                   bigint    NOT NULL,
   condition_concept_id        integer   NOT NULL,
   condition_era_start_date    date		 NOT NULL,		
   condition_era_end_date      date		 NOT NULL,		
   condition_occurrence_count  integer
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.condition_occurrence
(
   condition_occurrence_id        BIGSERIAL     NOT NULL,
   person_id                      bigint        NOT NULL,
   condition_concept_id           integer       NOT NULL,
   condition_start_date           date          NOT NULL,
   condition_start_datetime       timestamp,
   condition_end_date             date,
   condition_end_datetime         timestamp,
   condition_type_concept_id      integer       NOT NULL,
   condition_status_concept_id    integer,
   stop_reason                    varchar(20),
   provider_id                    bigint,
   visit_occurrence_id            bigint,
   visit_detail_id                bigint, 
   condition_source_value         varchar(250),
   condition_source_concept_id    integer,
   condition_status_source_value  varchar(50)  
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.cost
(
   cost_id                    integer       NOT NULL,
   cost_event_id              integer       NOT NULL,
   cost_domain_id             varchar(20)   NOT NULL,
   cost_type_concept_id       integer       NOT NULL,
   currency_concept_id        integer,
   total_charge               numeric,
   total_cost                 numeric,
   total_paid                 numeric,
   paid_by_payer              numeric,
   paid_by_patient            numeric,
   paid_patient_copay         numeric,
   paid_patient_coinsurance   numeric,
   paid_patient_deductible    numeric,
   paid_by_primary            numeric,
   paid_ingredient_cost       numeric,
   paid_dispensing_fee        numeric,
   payer_plan_period_id       bigint,
   amount_allowed             numeric,
   revenue_code_concept_id    integer,
   revenue_code_source_value  varchar(50),
   drg_concept_id             integer,
   drg_source_value           varchar(3)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.death
(
   person_id                bigint        NOT NULL,
   death_date               date          NOT NULL,
   death_datetime           timestamp,
   death_type_concept_id    integer       NOT NULL,
   cause_concept_id         integer,
   cause_source_value       varchar(50),
   cause_source_concept_id  integer
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.device_exposure
(
   device_exposure_id              BIGSERIAL      NOT NULL,
   person_id                       bigint         NOT NULL,
   device_concept_id               integer        NOT NULL,
   device_exposure_start_date      date           NOT NULL,
   device_exposure_start_datetime  timestamp,
   device_exposure_end_date        date,
   device_exposure_end_datetime    timestamp,
   device_type_concept_id          integer        NOT NULL,
   unique_device_id                varchar(50),
   quantity                        integer,
   provider_id                     bigint,
   visit_occurrence_id             bigint,
   visit_detail_id                 bigint,
   device_source_value             varchar(250),
   device_source_concept_id        integer
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.dose_era
(
   dose_era_id          BIGSERIAL    	NOT NULL,
   person_id            bigint    		NOT NULL,
   drug_concept_id      integer   		NOT NULL,
   unit_concept_id      integer  		NOT NULL,
   dose_value           numeric   		NOT NULL,
   dose_era_start_date  timestamp 		NOT NULL,		--from date to timestamp
   dose_era_end_date    timestamp 		NOT NULL		--from date to timestamp
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.drug_era
(
   drug_era_id          BIGSERIAL    	NOT NULL,
   person_id            bigint    		NOT NULL,
   drug_concept_id      integer   		NOT NULL,
   drug_era_start_date  date	  		NOT NULL,		
   drug_era_end_date    date	  		NOT NULL,		
   drug_exposure_count  integer,
   gap_days             integer
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.drug_exposure
(
   drug_exposure_id              BIGSERIAL     NOT NULL,
   person_id                     bigint        NOT NULL,
   drug_concept_id               integer       NOT NULL,
   drug_exposure_start_date      date          NOT NULL,
   drug_exposure_start_datetime  timestamp,
   drug_exposure_end_date        date          NOT NULL,
   drug_exposure_end_datetime    timestamp,
   verbatim_end_date             date,
   drug_type_concept_id          integer       NOT NULL,
   stop_reason                   varchar(20),
   refills                       integer,
   quantity                      numeric,
   days_supply                   integer,
   sig                           text,
   route_concept_id              integer,
   lot_number                    varchar(50),
   provider_id                   bigint,
   visit_occurrence_id           bigint,
   visit_detail_id               bigint,
   drug_source_value             varchar(250),
   drug_source_concept_id        integer,
   route_source_value            varchar(100),	-- updated since cdm_gold_202307 as the route in GOLD is longer than 50
   dose_unit_source_value        varchar(50)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.fact_relationship
(
   domain_concept_id_1      integer   NOT NULL,
   fact_id_1                integer   NOT NULL,
   domain_concept_id_2      integer   NOT NULL,
   fact_id_2                integer   NOT NULL,
   relationship_concept_id  integer   NOT NULL
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.location
(
   location_id            integer        NOT NULL,
   address_1              varchar(50),
   address_2              varchar(50),
   city                   varchar(50),
   state                  varchar(2),
   zip                    varchar(9),
   county                 varchar(20),
   location_source_value  varchar(50)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.measurement
(
   measurement_id                 BIGSERIAL       NOT NULL,
   person_id                      bigint          NOT NULL,
   measurement_concept_id         integer         NOT NULL,
   measurement_date               date            NOT NULL,
   measurement_datetime           timestamp,
   measurement_time               varchar(10),
   measurement_type_concept_id    integer         NOT NULL,
   operator_concept_id            integer,
   value_as_number                numeric,
   value_as_concept_id            integer,
   unit_concept_id                integer,
   range_low                      numeric,
   range_high                     numeric,
   provider_id                    bigint,
   visit_occurrence_id            bigint,
   visit_detail_id                bigint,
   measurement_source_value       varchar(250),
   measurement_source_concept_id  integer,
   unit_source_value              varchar(50),
   value_source_value             varchar(50)		-- from 2500 to 50
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.metadata
(
   metadata_concept_id       integer        NOT NULL,
   metadata_type_concept_id  integer        NOT NULL,
   name                      varchar(250)   NOT NULL,
   value_as_string           varchar(250),			-- from text to varchar(250)
   value_as_concept_id       integer,
   metadata_date             date,
   metadata_datetime         timestamp
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.metadata_tmp
(
   person_id  bigint         NOT NULL,
   name       varchar(250)   NOT NULL
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.note
(
   note_id                integer        NOT NULL,
   person_id              bigint         NOT NULL,
   note_date              date           NOT NULL,
   note_datetime          timestamp,
   note_type_concept_id   integer        NOT NULL,
   note_class_concept_id  integer        NOT NULL,
   note_title             varchar(250),
   note_text              text,
   encoding_concept_id    integer        NOT NULL,
   language_concept_id    integer        NOT NULL,
   provider_id            bigint,
   visit_occurrence_id    bigint,
   visit_detail_id        bigint,
   note_source_value      varchar(50)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.note_nlp
(
   note_nlp_id                 integer         NOT NULL,
   note_id                     integer         NOT NULL,
   section_concept_id          integer,
   snippet                     varchar(250),
   "offset"                    varchar(50),		-- from 250 to 50
   lexical_variant             varchar(250)    NOT NULL,
   note_nlp_concept_id         integer,
   note_nlp_source_concept_id  integer,
   nlp_system                  varchar(250),
   nlp_date                    date            NOT NULL,
   nlp_datetime                timestamp,
   term_exists                 varchar(1),
   term_temporal               varchar(50),
   term_modifiers              varchar(2000)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.observation
(
   observation_id                 BIGSERIAL       NOT NULL,
   person_id                      bigint          NOT NULL,
   observation_concept_id         integer         NOT NULL,
   observation_date               date            NOT NULL,
   observation_datetime           timestamp,
   observation_type_concept_id    integer         NOT NULL,
   value_as_number                numeric,
   value_as_string                varchar(60),		-- from 2000 to 60
   value_as_concept_id            integer,
   qualifier_concept_id           integer,
   unit_concept_id                integer,
   provider_id                    bigint,
   visit_occurrence_id            bigint,
   visit_detail_id                bigint,
   observation_source_value       varchar(250),
   observation_source_concept_id  integer,
   unit_source_value              varchar(50),		-- from 250 to 50 
   qualifier_source_value         varchar(50)		-- from 250 to 50 
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.observation_period
(
   observation_period_id          BIGSERIAL NOT NULL,
   person_id                      bigint    NOT NULL,
   observation_period_start_date  date      NOT NULL,
   observation_period_end_date    date      NOT NULL,
   period_type_concept_id         integer   NOT NULL
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.payer_plan_period
(
   payer_plan_period_id           bigint        NOT NULL,
   person_id                      bigint        NOT NULL,
   payer_plan_period_start_date   date          NOT NULL,
   payer_plan_period_end_date     date          NOT NULL,
   payer_concept_id               integer,
   payer_source_value             varchar(50),
   payer_source_concept_id        integer,
   plan_concept_id                integer,
   plan_source_value              varchar(50),
   plan_source_concept_id         integer,
   sponsor_concept_id             integer,
   sponsor_source_value           varchar(50),
   sponsor_source_concept_id      integer,
   family_source_value            varchar(50),
   stop_reason_concept_id         integer,
   stop_reason_source_value       varchar(50),
   stop_reason_source_concept_id  integer
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.person
(
   person_id                    bigint        NOT NULL,
   gender_concept_id            integer       NOT NULL,
   year_of_birth                integer       NOT NULL,
   month_of_birth               integer,
   day_of_birth                 integer,
   birth_datetime               timestamp,
   race_concept_id              integer       NOT NULL,
   ethnicity_concept_id         integer       NOT NULL,
   location_id                  integer,
   provider_id                  bigint,
   care_site_id                 integer,
   person_source_value          varchar(50),
   gender_source_value          varchar(50),
   gender_source_concept_id     integer,
   race_source_value            varchar(50),
   race_source_concept_id       integer,
   ethnicity_source_value       varchar(50),
   ethnicity_source_concept_id  integer
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.procedure_occurrence
(
   procedure_occurrence_id      BIGSERIAL     NOT NULL,
   person_id                    bigint        NOT NULL,
   procedure_concept_id         integer       NOT NULL,
   procedure_date               date          NOT NULL,
   procedure_datetime           timestamp,
   procedure_type_concept_id    integer       NOT NULL,
   modifier_concept_id          integer,
   quantity                     integer,
   provider_id                  bigint,
   visit_occurrence_id          bigint,
   visit_detail_id              bigint,
   procedure_source_value       varchar(250),
   procedure_source_concept_id  integer,
   modifier_source_value        varchar(50)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.provider
(
   provider_id                  bigint         NOT NULL,
   provider_name                varchar(255),
   npi                          varchar(20),
   dea                          varchar(20),
   specialty_concept_id         integer,
   care_site_id                 integer,
   year_of_birth                integer,
   gender_concept_id            integer,
   provider_source_value        varchar(50),
   specialty_source_value       varchar(250),
   specialty_source_concept_id  integer,
   gender_source_value          varchar(50),
   gender_source_concept_id     integer
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.specimen
(
   specimen_id                  int     	  NOT NULL,
   person_id                    bigint        NOT NULL,
   specimen_concept_id          integer       NOT NULL,
   specimen_type_concept_id     integer       NOT NULL,
   specimen_date                date          NOT NULL,
   specimen_datetime            timestamp,
   quantity                     numeric,
   unit_concept_id              integer,
   anatomic_site_concept_id     integer,
   disease_status_concept_id    integer,
   specimen_source_id           varchar(50),
   specimen_source_value        varchar(50),
   unit_source_value            varchar(50),
   anatomic_site_source_value   varchar(50),
   disease_status_source_value  varchar(50)
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.visit_detail
(
   visit_detail_id                 bigint     	 NOT NULL,
   person_id                       bigint        NOT NULL,
   visit_detail_concept_id         integer       NOT NULL,
   visit_detail_start_date         date          NOT NULL,
   visit_detail_start_datetime     timestamp,
   visit_detail_end_date           date          NOT NULL,
   visit_detail_end_datetime       timestamp,
   visit_detail_type_concept_id    integer       NOT NULL,
   provider_id                     bigint,
   care_site_id                    integer,
   visit_detail_source_value       varchar(50),
   visit_detail_source_concept_id  integer,
   admitting_source_value          varchar(50),
   admitting_source_concept_id     integer,
   discharge_to_source_value       varchar(50),
   discharge_to_concept_id         integer,
   preceding_visit_detail_id       bigint,
   visit_detail_parent_id          bigint,
   visit_occurrence_id             bigint        NOT NULL
)TABLESPACE {tablespace};

CREATE TABLE IF NOT EXISTS {sc}.visit_occurrence
(
   visit_occurrence_id            bigint     	NOT NULL,
   person_id                      bigint        NOT NULL,
   visit_concept_id               integer		NOT NULL,
   visit_start_date               date          NOT NULL,
   visit_start_datetime           timestamp,
   visit_end_date                 date          NOT NULL,
   visit_end_datetime             timestamp,
   visit_type_concept_id          integer		NOT NULL,
   provider_id                    bigint,
   care_site_id                   integer,
   visit_source_value             varchar(50),
   visit_source_concept_id        integer,
   admitting_source_concept_id    integer,
   admitting_source_value         varchar(50),
   discharge_to_concept_id        integer,
   discharge_to_source_value      varchar(50),
   preceding_visit_occurrence_id  bigint
)TABLESPACE {tablespace};