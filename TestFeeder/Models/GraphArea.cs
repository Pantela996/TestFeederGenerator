using GraphX.Controls;
using QuickGraph;
using System;

namespace TestFeederGenerator.Models
{

    public class GraphArea : GraphArea<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>
    {


  

       public GraphArea Clone()
        {
            return this.MemberwiseClone() as GraphArea;
        }
    }
}
