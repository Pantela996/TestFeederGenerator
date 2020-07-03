using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFeederGenerator.Models;

namespace TestFeeder.Models
{
    public class DataVertexTransformerPartial : DataVertex
    {

        public string Name { get; set; }

        public double _kVA { get; set; }

        public double _kV_High { get; set; }

        public double _kV_Low { get; set; }

        public double RPercentage { get; set; }

        public double XPercentage { get; set; }
    }
}
