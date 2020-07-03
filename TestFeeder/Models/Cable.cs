using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFeeder.Models
{
    public class Cable
    {
        public enum MaterialEnum
        {
            aaac,
            acsr,
            aluminum,
            aluminumAlloySteel,
            aluminumSteel,
            copper,
            other,
            steel
        }



        public MaterialEnum MaterialKind { get; set; }
        public string SizeDescription { get; set; }
        public Double PhaseWireSpacing { get; set; }

        public float Radius { get; set; }
        
        public float RatedCurrent { get; set; }

    }
}
