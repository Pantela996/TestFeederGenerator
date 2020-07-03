using GraphX.Controls;
using System.ComponentModel;
using TestFeederGenerator.Models;

namespace TestFeeder.ViewModels
{
    public interface IGraphViewModel
    {
        GraphArea Area { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        bool CreateDataVertexBase(DataVertex.TypeOfVertex typeOfVertex, string node_id);

    }
}