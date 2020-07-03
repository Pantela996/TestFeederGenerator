using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using TestFeederGenerator.Models;

namespace TestFeeder.Models
{
    public class DataVertexRegulator : DataVertex
    {

        public enum enumPhases
        {
            ABC,
            A,
            B,
            C
        }

        public enum enumConnection
        {
            ABCB,
            ACBC,
            ABCC
        }


        public enum enumMonitoring
        {
            ABCB,
            ACBC,
            ABCC
        }


        public enum enumCompensatorSettings
        {
            PhAB,
            PhCB
        }



        public string RegulatorID { get; set; }

        public string Line_Segment_From { get; set; }

        public string Line_Segment_To { get; set; }

        public string Location { get; set; }
        public enumPhases Phases{ get; set; }

        public enumConnection Connection { get; set; }

        public enumMonitoring MonitoringPhase { get; set; }

        public double Bandwidth { get; set; }

        public double Ratio { get; set; }

        public double PrimaryCTRating { get; set; }

        public enumCompensatorSettings CompensatorSettingsA { get; set; }

        public enumCompensatorSettings CompensatorSettingsB { get; set; }

        public double R_SettingA { get; set; }

        public double R_SettingB { get; set; }

        public double X_SettingA { get; set; }

        public double X_SettingB { get; set; }


        public double VoltageLevelA { get; set; }

        public double VoltageLevelB { get; set; }

    }
}
