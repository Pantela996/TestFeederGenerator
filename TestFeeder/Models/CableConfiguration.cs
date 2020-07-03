using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFeeder.ViewModels;

namespace TestFeeder.Models
{
    public class CableConfiguration
    {
        public enum PhasingEnum
        {
            A,
            B,
            C,
            AB,
            AC,
            BC,
            ABC
        }

        public string Name { get; set; }
        public PhasingEnum Phasing { get; set; }
       
        public ComplexValue Primar_value { get; set; }

        public List<ComplexValue> Secondary_value { get; set; }

        public List<ComplexValue> Terciar_value { get; set; }

        public Cable Cable { get; set; }

        public Double Susceptance { get; set; }
    }
}
