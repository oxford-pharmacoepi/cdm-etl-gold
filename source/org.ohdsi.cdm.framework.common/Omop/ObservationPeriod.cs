using org.ohdsi.cdm.framework.common.Enums;
using System;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace org.ohdsi.cdm.framework.common.Omop
{
    public class ObservationPeriod : Entity
    {
        public override EntityType GeEntityType()
        {
            return EntityType.ObservationPeriod;
        }

    }
}
