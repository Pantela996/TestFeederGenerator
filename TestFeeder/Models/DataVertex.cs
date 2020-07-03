using GraphX.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;

namespace TestFeederGenerator.Models
   { 


    public class DataVertex: VertexBase
    {


        public enum TypeOfVertex
        {
            REGULAR,
            REGULATOR_VERTEX,
            TRANSFORMER_VERTEX,
            REGULATOR_PARTIAL_VERTEX,
            TRANSFORMER_PARTIAL_VERTEX,
            SPOT_LOAD_VERTEX
        }


        private string element_id;

        public string Element_id
        {
            get { return element_id; }
            set { element_id = value; }
        }

        public string Text { get; set; }

        public List<string> connected_nodes;

        public TypeOfVertex typeOfVertex { get; set; }

        #region Calculated or static props

        public override string ToString()
        {
            return Text;
        }


        public DataVertex():this(string.Empty)
        {
        }

        public DataVertex(string text = "")
        {
            Text = text;
            connected_nodes = new List<string>();
            Element_id = text;
        }
        #endregion
    }
}
