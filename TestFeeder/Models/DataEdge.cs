using GraphX.Common.Models;
using System.Windows.Media.Animation;
using TestFeeder.Models;

namespace TestFeederGenerator.Models
{
    

    public class DataEdge : EdgeBase<DataVertex>
    {



        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }


        public DataEdge()
            : base(null, null, 1)
        {
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public CableConfiguration Configuration { get; set; }

        public double Length { get; set; }

        #region GET members
        public override string ToString()
        {
            return Text;
        }

        #endregion
    }
}
