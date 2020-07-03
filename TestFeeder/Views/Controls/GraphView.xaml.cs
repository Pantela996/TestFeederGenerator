using GraphX.Common.Enums;
using GraphX.Common.Models;
using GraphX.Controls;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TestFeeder.FileSerialization;
using TestFeeder.Models;
using TestFeeder.UndoRedo;
using TestFeeder.ViewModels;
using TestFeederGenerator.Models;

namespace TestFeeder.Views.Controls
{
    

    //View: GraphView, ViewModel: GraphViewModel
    //Passing values to logic part, listeners to user actions defined, along with some basic utility actions related to view
    //Most important methods: AreaClicked,EdgeClicked,VertexSelected,VertexClicked,VertexDoubleClicked

    public partial class GraphView : UserControl
    {
        private int last_added_id;

        public DataVertexRegulator dvr;
        public DataVertexTransformer dvt;

        public TestFeederGenerator.Models.GraphX dataGraph;
        private MainWindow mainWindow;

        public VertexControl _ecFrom;
        private EdgeBlueprint _edgeBp;
        public Double length;

        public UndoRedoCommander undoRedoCommander;


        public GraphView()
        {
            InitializeComponent();

            mainWindow = App.Current.MainWindow as MainWindow;
            dataGraph = new TestFeederGenerator.Models.GraphX();

            mainWindow.graphView = this;

            ZoomControl.SetViewFinderVisibility(zoomctrl, Visibility.Visible);
            zoomctrl.ZoomToFill();
            last_added_id = 1;

            //setup Area properties
            GraphAreaExample_Setup();

            //setup graph properties
            startGraphDrawing();


            undo.IsEnabled = false;
            redo.IsEnabled = false;

            (mainWindow.graphView.DataContext as GraphViewModel).undoRedoCommander = new UndoRedoCommander(this.DataContext);

        }

        void startGraphDrawing()
        {

            Area.GenerateGraph(true, false);
            //Area.SetEdgesDashStyle(EdgeDashStyle.DashDot);
            Area.ShowAllEdgesArrows(false);
            //Area.SetEdgesDrag(false);
            Area.UpdateAllEdges();
            Area.SetVerticesDrag(true);
            Console.WriteLine("dbg2");


            //setColorsOfNodes();


            zoomctrl.ZoomToFill();
        }

        private void GraphAreaExample_Setup()
        {
            //Lets create logic core and filled data graph with edges and vertices
            var logicCore = new GXLogicCore() { Graph = dataGraph };

            Console.WriteLine(dataGraph);



            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.FR;

            //Now we can set parameters for selected algorithm using AlgorithmFactory property. This property provides methods for
            //creating all available algorithms and algo parameters.
            logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            ((KKLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            //Default parameters are created automaticaly when new default algorithm is set and previous params were NULL
            logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = false;



            //Finally assign logic core to GraphArea object
            Area.LogicCore = logicCore;


        }

        public void Dispose()
        {
            //If you plan dynamicaly create and destroy GraphArea it is wise to use Dispose() method
            //that ensures that all potential memory-holding objects will be released.
            Area.Dispose();
        }

        public VertexControl getVertexControlWithDataVertex(string node_id)
        {
            foreach (DataVertex dataVertex in Area.VertexList.Keys)
            {
                if (dataVertex.Element_id == node_id)
                {
                    return Area.VertexList[dataVertex];
                }
            }
            return null;
        }

       
        private void FindSelectedVertex(string vertex_id, bool found, VertexControl vc)
        {
            if (!found)
            {
                Console.WriteLine("No vertex found");
            }

            DataVertex dv = mainWindow.GlobalVertices[vertex_id];

            (this.DataContext as GraphViewModel).ShowVertexDetails(dv, vc);

        }

        private bool FindVertexInCollection(string vertex_id)
        {
            foreach (string q in mainWindow.GlobalVertices.Keys)
            {

                if (q == vertex_id)
                {
                    return true;
                }
            }
            return false;

        }

        private void AddElement(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string type = b.DataContext as string;
            int next = 1;

            var w = new Window();


            DataVertex.TypeOfVertex typeOfVertex;

            switch (type)
            {
                case "R":
                    typeOfVertex = DataVertex.TypeOfVertex.REGULATOR_VERTEX;
                    break;
                case "T":
                    typeOfVertex = DataVertex.TypeOfVertex.TRANSFORMER_VERTEX;
                    break;
                case "SL":
                    typeOfVertex = DataVertex.TypeOfVertex.SPOT_LOAD_VERTEX;
                    break;
                default:
                    typeOfVertex = DataVertex.TypeOfVertex.REGULAR;
                    break;
            }


            if (auto_generate_id.IsChecked == true)
            {
                next = last_added_id;
                while (mainWindow.GlobalVertices.ContainsKey((next).ToString()))
                {
                    last_added_id = last_added_id + 1;
                    next++;
                }

                (this.DataContext as GraphViewModel).CreateDataVertexBase(typeOfVertex, next.ToString());
            }
            else
            {
                w = new NodeID(mainWindow, typeOfVertex);
                w.Show();
            }



        }

        private void Area_VertexDoubleClick(object sender, GraphX.Controls.Models.VertexSelectedEventArgs args)
        {
            VertexControl vc = (VertexControl)args.VertexControl;
            DataVertex dv = (DataVertex)vc.Vertex;
            string vertex_id = dv.Element_id;
            bool found = FindVertexInCollection(vertex_id);
            FindSelectedVertex(vertex_id, found, args.VertexControl);
        }


   

        //public void mockData()
        //{

        //    bool is_first = true;

        //    CableConfiguration cc = new CableConfiguration()
        //    {
        //        Name = "721",
        //        Phasing = CableConfiguration.PhasingEnum.ABC,
        //        Primar_value = new ComplexValue()
        //        {
        //            Real_part = 1,
        //            Imaginary_part = 1
        //        },
        //        Cable = new Models.Cable()
        //        {
        //            MaterialKind = Models.Cable.MaterialEnum.aluminum,
        //            SizeDescription = "111",
        //            PhaseWireSpacing = 1,
        //            Radius = 1,
        //            RatedCurrent = 1
        //        },
        //        Susceptance = 1
        //    };


        //    mainWindow.addCableConfiguration(cc);

        //    for (int i = 0; i < 5; i++)
        //    {
        //        SpotLoad dv = new SpotLoad()
        //        {
        //            typeOfVertex = DataVertex.TypeOfVertex.SPOT_LOAD_VERTEX,
        //            Text = "Spot load: " + i.ToString(),
        //            Element_id = i.ToString()
        //        };
        //        CreateVertexWithType(dv.typeOfVertex, dv, is_first);
        //        is_first = false;
        //    }

        //    int length = 0;
        //    for (int i = 0; i < mainWindow.GlobalVertices.Count - 1; i++)
        //    {
        //        length = length + 100;
        //        for (int j = 1; j < mainWindow.GlobalVertices.Count; j++)
        //        {
        //            VertexControl vc = Area.VertexList[mainWindow.GlobalVertices[i.ToString()]];
        //            if (i < j)
        //            {
        //                AreaAddEdge(mainWindow.GlobalVertices.ElementAt(i).Value, mainWindow.GlobalVertices.ElementAt(j).Key, length, false, vc, cc);
        //            }

        //        }
        //    }

        //    DataVertexTransformer dvt = new DataVertexTransformer();
        //    dvt.Text = "211";
        //    dvt._kVA_A = 200;
        //    dvt._kVA_B = 200;
        //    dvt._kV_LowA = 200;
        //    dvt._kV_LowB = 200;
        //    dvt.NameA = "Transformer End A";
        //    dvt.NameB = "Transformer End B";
        //    dvt.Line_to = "111";
        //    dvt.Line_from = "211";
        //    dvt.Element_id = "211";

        //    DataVertexTransformerPartial dvtp = new DataVertexTransformerPartial()
        //    {
        //        Text = "111",
        //        Name = dvt.NameB,
        //        _kVA = dvt._kVA_B,
        //        _kV_Low = dvt._kV_LowB,
        //        Element_id = "111"
        //    };

        //    dvtp.typeOfVertex = DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX;



        //    CreateVertexWithType(DataVertex.TypeOfVertex.TRANSFORMER_VERTEX, dvt, false, true);
        //    addVertexToGraph(dvt, dvtp, dvt, "1", cc);

        //    //tg_highlightEnabled_Checked(null, null);

        //}

        public void ClearArea()
        {

            Area.ClearLayout(true, true, true);
            GraphAreaExample_Setup();
            startGraphDrawing();
            (this.DataContext as GraphViewModel).dvr = null;
            (this.DataContext as GraphViewModel).dvt = null;
            (this.DataContext as GraphViewModel).dvrp = null;
            (this.DataContext as GraphViewModel).dvtp = null;
            (this.DataContext as GraphViewModel).last_added_id = 1;
            (this.DataContext as GraphViewModel).undoRedoCommander.Undo_stack = new Stack<Command>();
            (this.DataContext as GraphViewModel).undoRedoCommander.Redo_stack = new Stack<Command>();
            redo.IsEnabled = false;
            undo.IsEnabled = false;
            mainWindow.GlobalVertices = new Dictionary<string, DataVertex>();
            mainWindow.GlobalEdges = new Dictionary<string, List<DataEdge>>();
            mainWindow.GlobalSpotLoads = new Dictionary<string, SpotLoad>();
            mainWindow.GlobalCableConfiguration = new Dictionary<string, CableConfiguration>();
            //mainWindow.mockup_data.IsEnabled = true;

        }


        private void Area_VertexSelected(object sender, GraphX.Controls.Models.VertexSelectedEventArgs args)
        {
            if (ACLine.IsChecked == false)
            {
                return;
            }

            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                (this.DataContext as GraphViewModel).ConnectEdges(args.VertexControl);
            }

        }


        private void zoomctrl_Click(object sender, RoutedEventArgs e)
        {

            (this.DataContext as GraphViewModel)._ecFrom = null;

        }


        private void EnableACLine(object sender, RoutedEventArgs e)
        {

            if (ACLine.IsChecked == true)
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Arrow;
            }


            if (ACLine.IsChecked == true)
            {
                xbutton.IsChecked = false;
                //xvertex.IsChecked = false;
            }


        }

        public void SerializeVertexData(System.Windows.Forms.SaveFileDialog result)
        {

            List<GraphSerializationData> graphSerializationData = new List<GraphSerializationData>();
            Console.WriteLine(mainWindow.GlobalVertices.Count);
            foreach (VertexControl vc in Area.VertexList.Values)
            {
                Point p = vc.GetPosition();

                GraphSerializationData gsd = new GraphSerializationData()
                {
                    Data = (DataVertex)vc.Vertex,
                    Position = new GraphX.Measure.Point(p.X, p.Y),
                    IsVisible = true,
                    HasLabel = true
                };
                graphSerializationData.Add(gsd);
            }
            
            FileServiceProvider.SerializeDataToFile(result.FileName + "_vertex_data", graphSerializationData);
        }

        public void DeserializeVertexData(string vertex_file)
        {
            List<GraphSerializationData> gds = FileServiceProvider.DeserializeDataFromFile(vertex_file);

            this.ClearArea();


            (this.DataContext as GraphViewModel).RecreateSerializedVertexData(gds);


           

        }

        public void SerializeEdgeData(System.Windows.Forms.SaveFileDialog result)
        {

            List<GraphSerializationData> graphSerializationData = new List<GraphSerializationData>();
            foreach (DataEdge ec in Area.EdgesList.Keys)
            {

                GraphSerializationData gsd = new GraphSerializationData()
                {
                    Data = ec,
                    IsVisible = true,
                    HasLabel = true
                };
                graphSerializationData.Add(gsd);
            }

            FileServiceProvider.SerializeDataToFile(result.FileName + "_edge_data", graphSerializationData);
        }

        public void DeserializeEdgeData(string edge_file)
        {
            List<GraphSerializationData> gds = FileServiceProvider.DeserializeDataFromFile(edge_file);

            foreach (GraphSerializationData x in gds)
            {

                DataVertex source = (x.Data as DataEdge).Source;
                DataVertex target = (x.Data as DataEdge).Target;

                VertexControl vc = getVertexControlWithDataVertex(source.Element_id);

                (this.DataContext as GraphViewModel).RecreateSerializedEdgeData(source,target,vc, ((DataEdge)x.Data).Length);
                //AreaAddEdge(source, target.Element_id, ((DataEdge)x.Data).Length, false, vc);

            }

        }

        public void SerializeConfigurationData(System.Windows.Forms.SaveFileDialog result)
        {

            List<GraphSerializationData> graphSerializationData = new List<GraphSerializationData>();
            foreach (CableConfiguration cc in mainWindow.GlobalCableConfiguration.Values)
            {

                GraphSerializationData gsd = new GraphSerializationData()
                {
                    Data = cc
                };
                graphSerializationData.Add(gsd);
            }

            FileServiceProvider.SerializeDataToFile(result.FileName + "_cable_config", graphSerializationData);
        }

        public void DeserializeConfigurationData(string cable_file)
        {
            List<GraphSerializationData> gds = FileServiceProvider.DeserializeDataFromFile(cable_file);

            foreach (GraphSerializationData x in gds)
            {
                CableConfiguration cc = x.Data as CableConfiguration;

                (mainWindow.ccView.DataContext as CreateConfigurationViewModel).AddCableConfiguration(cc);

            }

        }

        private void undo_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as GraphViewModel).Undo_Click();
        }

        private void redo_Click(object sender, RoutedEventArgs e)
        {

            (this.DataContext as GraphViewModel).Redo_click();
        }

        private void XClick(object sender, RoutedEventArgs e)
        {
            if (xbutton.IsChecked == true)
            {
                Cursor = Cursors.Cross;
            }
            else
            {
                Cursor = Cursors.Arrow;
            }

            if (xbutton.IsChecked == true)
            {
                ACLine.IsChecked = false;
                //xvertex.IsChecked = false;
            }
        }


        //private void XVertexClick(object sender, RoutedEventArgs e)
        //{
        //    if (xvertex.IsChecked == true)
        //    {
        //        Cursor = Cursors.Pen;
        //    }
        //    else
        //    {
        //        Cursor = Cursors.Arrow;
        //    }

        //    if (xvertex.IsChecked == true)
        //    {
        //        ACLine.IsChecked = false;
        //        xbutton.IsChecked = false;
        //    }

        //}

        private void Area_EdgeClicked(object sender, GraphX.Controls.Models.EdgeClickedEventArgs args)
        {
            EdgeControl ec = (EdgeControl)args.Control;


            if (xbutton.IsEnabled == true)
            {
                (this.DataContext as GraphViewModel).DeleteConnection((ec.Source.Vertex as DataVertex), (ec.Target.Vertex as DataVertex).Element_id);
            }

            //DeleteConnection();
        }

        private void Area_VertexClicked(object sender, GraphX.Controls.Models.VertexClickedEventArgs args)
        {
            VertexControl vc = (VertexControl)args.Control;


            //if (xvertex.IsEnabled == true)
            //{
            //    (this.DataContext as GraphViewModel).DeleteVertex(vc.Vertex as DataVertex);
            //}

            //DeleteConnection();
        }


        private void HideShowToolbar(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                ToolbarVisibility();
            }

        }


        public void ToolbarVisibility()
        {
            if (toolbar.Visibility == Visibility.Hidden)
            {
                toolbar.Visibility = Visibility.Visible;
            }
            else if (toolbar.Visibility == Visibility.Visible)
            {
                toolbar.Visibility = Visibility.Hidden;
            }
        }
    }
}
