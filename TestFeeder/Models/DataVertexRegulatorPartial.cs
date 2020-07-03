using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFeederGenerator.Models;

namespace TestFeeder.Models
{
    public class DataVertexRegulatorPartial : DataVertex
    {
        public string RegulatorID { get; set; }

        public DataVertexRegulator.enumCompensatorSettings CompensatorSettingsB { get; set; }

        public double R_SettingB { get; set; }

        public double X_SettingB { get; set; }

        public double VoltageLevelB { get; set; }
    }
}
