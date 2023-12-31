createProcedureOccurrenceTests <- function () {

  ######################################
  # PROCEDURE
  ######################################
  patient <- createPatient();
  declareTest("Tests procedure record with proc_code_type CPT4", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code = '36415', proc_code_type = 'CPT4', proc_date = '2009-01-01')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 2108115, procedure_source_value = '36415')


  patient <- createPatient();
  declareTest("Tests procedure record with proc_code_type HCPCS", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code = 'G0290', proc_code_type = 'HCPCS', proc_date = '2009-01-01')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 43533247, procedure_source_value = 'G0290')


  patient <- createPatient();
  declareTest("Tests procedure record with proc_code_type ICD9", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code = '33.50', proc_code_type = 'ICD9', proc_date = '2009-01-01')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 2001373, procedure_source_value = '33.50')


  patient <- createPatient();
  declareTest("Tests procedure record with proc_code_type ICD10", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code = '0J8S0ZZ', proc_code_type = 'ICD10', proc_date = '2009-01-01')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 2755024, procedure_source_value = '0J8S0ZZ')


  patient <- createPatient();
  declareTest("Tests procedure type concept", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_date = '2009-01-01', proc_code = '36415', proc_code_type = 'CPT4')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_type_concept_id = 38000275)


  patient <- createPatient();
  declareTest("Tests procedure record with proc_code_type of blank results in unmapped procedure occurrence", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code_type = '', proc_date = '2009-01-01', proc_code = '36415')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 0)


  patient <- createPatient();
  declareTest("Tests procedure record with proc_code_type CUSTOM results in unmapped procedure occurrence", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code_type = 'CUSTOM', proc_date = '2009-01-01', proc_code = '36415')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 0)


  patient <- createPatient();
  declareTest("Tests procedure record with proc_code_type REV results in unmapped procedure occurrence", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code_type = 'REV', proc_date = '2009-01-01', proc_code = '36415')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 0)


  patient <- createPatient();
  declareTest("Tests procedure record with proc_code_type CLM_UNK results in unmapped procedure occurrence", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code_type = 'CLM_UNK', proc_date = '2009-01-01', proc_code = '36415')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 0)

  patient <- createPatient();
  declareTest("Tests procedure record with CPT4 code 38241 to verify that it creates a procedure record", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_date = '2009-01-01', proc_code = '38241')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 2108457)

  patient <- createPatient();
  declareTest("Tests procedure record with nonsense proc_code to verify entry created", source_pid = patient$ptid, cdm_pid = patient$person_id)
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_procedure(ptid=patient$ptid, proc_code = '000123', proc_code_type = 'CPT4', proc_date = '2009-01-01')
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 0, procedure_source_value = '000123')



  ######################################
  # DIAGNOSIS
  ######################################
  patient <- createPatient();
  declareTest("Tests procedure type concept id from diagnosis record", source_pid = patient$ptid, cdm_pid = patient$person_id)
  enc <- createEncounter();
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_encounter(ptid=patient$ptid, encid = enc$encid, interaction_date='2009-01-01')
  add_diagnosis(ptid=patient$ptid, diagnosis_status = 'Diagnosis of', diagnosis_cd = '79382',
                diagnosis_cd_type = 'ICD9', diag_date = '2009-01-01', encid = enc$encid)
  expect_procedure_occurrence(person_id = patient$person_id, procedure_type_concept_id = 42865906)


  patient <- createPatient();
  declareTest("Diagnosis without ''Diagnosis of'' status is not loaded", source_pid = patient$ptid, cdm_pid = patient$person_id)
  enc <- createEncounter();
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_encounter(ptid=patient$ptid, encid = enc$encid, interaction_date='2009-01-01')
  add_diagnosis(ptid=patient$ptid, diagnosis_status = 'Family history of', diagnosis_cd = '634.91',
                diagnosis_cd_type = 'ICD9', diag_date = '2009-01-01', encid = enc$encid)
  expect_no_procedure_occurrence(person_id = patient$person_id)


  patient <- createPatient();
  declareTest("Diagnosis record with diagnosis_cd_type ICD9", source_pid = patient$ptid, cdm_pid = patient$person_id)
  enc <- createEncounter();
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_encounter(ptid=patient$ptid, encid = enc$encid, interaction_date='2009-01-01')
  add_diagnosis(ptid=patient$ptid, diagnosis_status = 'Diagnosis of', diagnosis_cd = '79382',
                diagnosis_cd_type = 'ICD9', diag_date = '2009-01-01', encid = enc$encid)
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 4324693, procedure_source_value = '79382')


  patient <- createPatient();
  declareTest("Diagnosis record with diagnosis_cd_type ICD10", source_pid = patient$ptid, cdm_pid = patient$person_id)
  enc <- createEncounter();
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_encounter(ptid=patient$ptid, encid = enc$encid, interaction_date='2009-01-01')
  add_diagnosis(ptid=patient$ptid, diagnosis_status = 'Diagnosis of', diagnosis_cd = 'R939',
                diagnosis_cd_type = 'ICD10', diag_date = '2009-01-01', encid = enc$encid)
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 4155793, procedure_source_value = 'R939')


  patient <- createPatient();
  declareTest("Diagnosis record with diagnosis_cd_type SNOMED", source_pid = patient$ptid, cdm_pid = patient$person_id)
  enc <- createEncounter();
  add_patient(ptid=patient$ptid, birth_yr = 1950, gender = 'Male',
              first_month_active = '200701', last_month_active = '201001')
  add_encounter(ptid=patient$ptid, encid = enc$encid, interaction_date='2009-01-01')
  add_diagnosis(ptid=patient$ptid, diagnosis_status = 'Diagnosis of', diagnosis_cd = '10019001',
                diagnosis_cd_type = 'SNOMED', diag_date = '2009-01-01', encid = enc$encid)
  expect_procedure_occurrence(person_id = patient$person_id, procedure_concept_id = 4001760, procedure_source_value = '10019001')

}
