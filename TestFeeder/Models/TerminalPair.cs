using FTN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFeeder.Models
{
    public class TerminalPair : IdentifiedObject
    {
        public Terminal terminalA { get; set; }
        public Terminal terminalB { get; set; }
    }
}
