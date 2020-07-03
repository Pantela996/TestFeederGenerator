using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFeederGenerator.Models;

namespace TestFeeder.Models
{
    public class SpotLoad : DataVertex,ICircuitElement
    {

        public enum LoadModelEnum
        {
            D_PQ,
            D_I,
            D_Z
        }

        public string Name { get; set; }
        public DataVertex Node { get; set; }

        public LoadModelEnum LoadModel { get; set; }

        public double Ph_1 { get; set; }

        public double Ph_1_2 { get; set; }

        public double Ph_2 { get; set; }

        public double Ph_2_2 { get; set; }

        public double Ph_3 { get; set; }

        public double Ph_3_2 { get; set; }


    }
}
