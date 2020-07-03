using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFeederGenerator.Models;

namespace TestFeeder.Models
{
    public class DataVertexTransformer : DataVertex
    {
        public string NameA { get; set; }

        public string NameB { get; set; }

        public double _kVA_A { get; set; }

        public double _kVA_B { get; set; }

        public double _kV_HighA { get; set; }

        public double _kV_HighB { get; set; }

        public double _kV_LowA { get; set; }

        public double _kV_LowB { get; set; }

        public double RPercentageA { get; set; }

        public double RPercentageB { get; set; }

        public double XPercentageA { get; set; }

        public double XPercentageB { get; set; }

        public string Line_from { get; set; }

        public string Line_to { get; set; }
    }
}
