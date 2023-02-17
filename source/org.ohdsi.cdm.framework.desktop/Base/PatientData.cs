using org.ohdsi.cdm.framework.common.Base;
using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Definitions;
using org.ohdsi.cdm.framework.common.Enums;
using org.ohdsi.cdm.framework.common.Omop;
using org.ohdsi.cdm.framework.desktop.Databases;
using org.ohdsi.cdm.framework.desktop.DbLayer;
using org.ohdsi.cdm.framework.desktop.Enums;
using org.ohdsi.cdm.framework.desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Text;

namespace org.ohdsi.cdm.framework.desktop.Base
{
    public class PatientData
    {
        public List<Person> personList = new List<Person>();
        public List<ObservationPeriod> ObservationPeriodList = new List<ObservationPeriod>();
        public List<Metadata> metadataList = new List<Metadata>();

        public void Clean() {
            personList.Clear();
            ObservationPeriodList.Clear();
            metadataList.Clear();
        }

    }
}
