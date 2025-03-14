The .Net CDM Builder was initially developed by Janssen Research & Development as a tool to transform its observational databases into the OMOP Common Data Model.
You can find the original code in https://github.com/OHDSI/ETL-CDMBuilder

The program is used to convert CPRD GOLD into OMOP CDM. 

Current Software version: Microsoft Visual Studio Community 2022 (64-bit) Version 17.13.1

Set up .NET project in local
=============
1. Download [Visual Studio Setup](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false) i.e., VisualStudioSetup.exe
2. Download and install ***Visual Studio Installer*** by executing VisualStudioSetup.exe
3. Lauch ***Visual Studio Installer*** using admin role
4. Install ***Visual Studio Community 2022*** and ***.NET Desktop Development*** over Visual Studio Installer *Since v17.13.1 is not available, please install v.17.13.3

![image](https://github.com/user-attachments/assets/fd811003-a8ac-4e8f-81ce-c484e30a8054)

![image](https://github.com/user-attachments/assets/91b677af-4d7c-43b4-85ab-77c603aaedeb)

5. Download ***cdm_etl_gold*** repository to your local device
6. Lauch ***Visual Studio Community 2022*** and load the ***cdm_etl_gold*** project

![image](https://github.com/user-attachments/assets/c33d70f9-80ae-46f3-b2c1-4ef57a0774d6)

7. Configurate startup projects as org.ohdsi.cdm.presentation.builder

![image](https://github.com/user-attachments/assets/7d8bb2e0-f981-43e1-ad0d-bc92e92c17ef)
![image](https://github.com/user-attachments/assets/372adb85-1a75-4d1d-808c-842ba642b817)

How to run
=============
After setting up an .Net project
1. Update and save ***org.ohdsi.cdm.presentation.builder/App.config*** to configure the chunk and CDM settings.
Given that the ***SourceReleaseDate*** parameter is for protocol and it is not used in utilized in standard GOLD mapping.
![image](https://github.com/user-attachments/assets/553512ef-362a-4e0e-90c9-96e3cb2e8115)

3. Build Solution

![image](https://github.com/user-attachments/assets/906d098f-e00e-488b-9399-f0c1b10b03ba)

3. Start Debugging

![image](https://github.com/user-attachments/assets/fa027f04-af05-46b7-998e-c3e3c6323a26)

4. input database information under the setting tab in the pop-up Building Manager Window
*If you want to re-load the previous mapping profile(DB connection infomation), make sure there is ONLY ONE Settings.xml under source\org.ohdsi.cdm.presentation.builder\bin\Debug\net8.0-windows ***including any sub-folders under source\org.ohdsi.cdm.presentation.builder\bin\Debug\net8.0-windows***
5. click building tag and the start button to start the mapping. And corresponding log will be shown.

![image](https://github.com/user-attachments/assets/cd0b5fd0-2832-4c51-b15e-c446a2d1a815)

Example:
![image](https://github.com/user-attachments/assets/8714720d-35f8-4707-b901-030c741d0345)


Tag log
=============

v.5.1.0
=============
* Upgraded Npgsql from v8.0.3 to v9.0.2
* Bug fixed incorrect measurement_source_concept_id representing Read Code which is case sensitive
* Bug fixed incorrectly loading the cdm schema as the vocab schema
* Added mapping Specialty (+)
* Applied to **GOLD 202501 release**

v.5.0.1
=============
* Align CDM DDL v5.3 and 5.4 with 4b_OMOPCDM_postgresql_5_3_ddl.sql and 4b_OMOPCDM_postgresql_5_4_ddl.sql
* Upgraded from net6.0 to net8.0

v.5.0
=============
* Populate Specimen
* Support protocol, _p in database name (+)
* Added source release date in configuration for protocol, _p (also used in observation_period) (+)
* Upgraded from netcoreapp3.1 to net6.0
* Applied to **GOLD 202407 release, cdm_gold_p22_001867**


v.4.0
=============
* Enhancing read code list for mapping COVID-19 brand name (+)
    - [Readcode and brand name about COVID-19 vaccination](https://help.cegedim-healthcare.co.uk/Coronavirus_guidance/Content/Coronavirus_Guidance/Vaccinations.htm)
    - P.S. COVID Medicago (Previously COVID-19 – Medicago) not found in GOLD
* update procedure_type_concept_id (+)
* left empty the non-required concept_id fields empty if there is no source value to map to a concept id (+)
* Bug fixed non-condition concepts in condition_occurrence
* not map read code = ZZZZZ00 (+)
* build PK, indexes and FKs in Era Tables (+)
* Applied to **GOLD 202401 release** and **cdm_gold_p22_001867**

v.3.0
=============
* Support CDM v.5.4 (+)
    - (backward compatible to CDM v.5.3)
* Map route in drugs (+)
* Bug fixed the incorrect observation_end_date
* [Tentative] No vaccinations in drug_era (sue to the CVX issues) (-)
* Applied to **GOLD 202307 release**
  
v.2.0
=============
* [Expand COVID-19 Vaccination brand infomation](https://cprd.com/sites/default/files/2022-03/SARS-CoV-2%20counts%20Feb2022.pdf) (+)
* Only map events within observation period (+)
    - For Death, the observation period = observation_start_date to observation_end_date + 3 months)
* Data Cleaning function (-)
* Applied to **GOLD 202301 release**

v.1.0
=============
* SQL tunning
* Autogenerated ids in OMOP CDM tables (+)
* Data Cleaning function (+)
* Applied to **GOLD 202207 release**

v.0.0
=============
Clone from https://github.com/OHDSI/ETL-CDMBuilder

Backlogs
=============
- [x] Support CDM v.5.4
- [x] Populate Specimen 
- [x] Upgrade to net8.0
- [ ] Map Ethnicities 
- [ ] Map Townsend deprivation index to [715996](https://athena.ohdsi.org/search-terms/terms/715996) 
