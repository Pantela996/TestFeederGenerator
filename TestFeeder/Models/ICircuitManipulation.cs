using FTN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFeeder.Models
{
    interface ICircuitManipulation
    {

        //void Show();
        void Create(ICircuitElement element);
        bool Delete();
        void Update(ICircuitElement element);


    }
}
