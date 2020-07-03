using GraphX.Common.Models;
using GraphX.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TestFeeder.Models;
using TestFeeder.UndoRedo;
using TestFeeder.Views;
using TestFeederGenerator.Models;

namespace TestFeeder.ViewModels
{


    //Main logic, CRUD operations for entities, Serialization/Deserialization, UndoRedo commander
    public class GraphViewModel : INotifyPropertyChanged, IGraphViewModel
    {

        private GraphArea area;

        private MainWindow mainWindow;
        public GraphState graphState;

        public DataVertexTransformer dvt;
        public DataVertexRegulator dvr;
        public DataVertexRegulatorPartial dvrp;
        public DataVertexTransformerPartial dvtp;

        public VertexControl _ecFrom;
        private EdgeBlueprint _edgeBp;

        public UndoRedoCommander undoRedoCommander;

        private Window w = new Window();


        int x = 0;
        int y = 0;

        public double length;
        public int last_added_id;

        public enum GraphState
        {
            NORMAL,
            UNDO,
            REDO,
            MOCK,
            PARTIAL_CONNECTING,
            UPDATE
        }

        public GraphViewModel()
        {

            mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow.graphViewModel = this;
            last_added_id = 1;

            graphState = GraphState.NORMAL;



        }

        public GraphArea Area
        {
            get { return area; }
            set
            {
                area = value;
                OnPropertyChanged(nameof(area));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }


        //Creating Empty Vertex that can be SpotLoad, Regulator,Transformer, not filled with data!
        public bool CreateDataVertexBase(DataVertex.TypeOfVertex type_of_vertex, string node_id)
        {

            last_added_id = Int32.Parse(node_id);
            int key = last_added_id;
            int result;

            if (node_id == "")
            {
                return false;
            }

            bool try_parse_int = Int32.TryParse(node_id, out result);

            if (try_parse_int == false)
            {
                MessageBox.Show("Enter number");
                return false;
            }

            if (mainWindow.GlobalVertices.ContainsKey(key.ToString()))
            {
                MessageBox.Show("Node with that id already exists");
                return false;
            }

            DataVertex dv = new DataVertex(key.ToString());

            switch (type_of_vertex)
            {

                case DataVertex.TypeOfVertex.SPOT_LOAD_VERTEX:
                    CreateSpotLoadVertex(dv);
                    break;
                case DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX:
                    CreateTransformerPartialVertex(dv);
                    break;
                case DataVertex.TypeOfVertex.TRANSFORMER_VERTEX:
                    CreateTransformerVertex(dv);
                    break;
                case DataVertex.TypeOfVertex.REGULATOR_VERTEX:
                    CreateRegulatorVertex(dv);
                    break;
                case DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX:
                    CreateRegulatorPartialVertex(dv);
                    break;
                default:
                    break;
            }

            VertexControl vc = GetVertexControlWithDataVertex(dv.Element_id);

            Point point = CreatePointBasedOn(vc);

            vc = GetVertexControlWithDataVertex(dv.Element_id);

            if (vc == null)
            {
                return false;
            }

            point = vc.GetPosition();

            if (graphState == GraphState.NORMAL)
            {
                undoRedoCommander.addUndoCommand(new Command("AddVertex", vc.Vertex, point));
            }


            return true;


        }

        private void CreateSpotLoadVertex(DataVertex dv, double m = 0, double n = 0)
        {
            SpotLoad sp = new SpotLoad();

            VertexControl vc;
            sp.Text = "Spot load: " + dv.Element_id;
            sp.Element_id = dv.Element_id;
            sp.typeOfVertex = DataVertex.TypeOfVertex.SPOT_LOAD_VERTEX;

            vc = new VertexControl(sp);

            mainWindow.GlobalVertices.Add(sp.Element_id, sp);

            if (mainWindow.GlobalSpotLoads.ContainsKey(sp.Element_id))
            {
                mainWindow.GlobalSpotLoads[sp.Element_id] = sp;
            }
            else
            {
                mainWindow.GlobalSpotLoads.Add(sp.Element_id, sp);
            }




            mainWindow.spotLoadTab._globalSpotLoads = mainWindow.GlobalSpotLoads;
            mainWindow.spotLoadTab.populateListView();

            if (m == 0)
            {
                vc.SetPosition(new Point(x, y));
            }
            else
            {
                vc.SetPosition(m, n);
            }
            vc.Background = Brushes.Gray;
            mainWindow.graphView.Area.AddVertexAndData(sp, vc, true);

            if (graphState == GraphState.NORMAL)
            {
                x += 30;
                y += 30;
            }


            return;
        }


        private void CreateTransformerVertex(DataVertex dv, double m = 0, double n = 0)
        {
            VertexControl vc;

            if (dvt != null)
            {
                MessageBox.Show("Transformer already created");
                return;
            }



            dvt = new DataVertexTransformer()
            {
                Text = "First transfromer ending: " + dv.Element_id,
                Element_id = dv.Element_id
            };


            dvt.typeOfVertex = DataVertex.TypeOfVertex.TRANSFORMER_VERTEX;

            vc = new VertexControl(dvt);



            if (m == 0)
            {
                vc.SetPosition(new Point(x, y));
            }
            else
            {
                vc.SetPosition(m, n);
            }

            mainWindow.GlobalVertices.Add(dvt.Element_id, dvt);

            vc.Background = Brushes.GreenYellow;
            mainWindow.graphView.Area.AddVertexAndData(dvt, vc, true);



            Point point = CreatePointBasedOn(vc);

            if (graphState == GraphState.NORMAL)
            {
                x += 30;
                y += 30;
            }

            return;
        }

        private void CreateTransformerPartialVertex(DataVertex dv, double m = 0, double n = 0)
        {
            DataVertexTransformerPartial dvtp = new DataVertexTransformerPartial()
            {
                Text = dv.Text,
                Element_id = dv.Element_id
            };

            VertexControl vc;
            vc = new VertexControl(dvtp);




            dvtp.typeOfVertex = DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX;



            vc = new VertexControl(dvtp);


            if (m == 0)
            {
                dvtp.Text = "Second transfromer ending: " + dvtp.Element_id;
                vc.SetPosition(new Point(x, y));
            }
            else
            {
                vc.SetPosition(m, n);
            }

            mainWindow.GlobalVertices.Add(dvtp.Element_id, dvtp);

            vc.Background = Brushes.GreenYellow;
            mainWindow.graphView.Area.AddVertexAndData(dvtp, vc, true);

            Point point = CreatePointBasedOn(vc);


            if (graphState == GraphState.NORMAL)
            {
                x += 30;
                y += 30;
            }

            this.dvtp = dvtp;
            return;
        }

        private void CreateRegulatorVertex(DataVertex dv, double m = 0, double n = 0)
        {
            VertexControl vc;
            if (dvr != null)
            {
                MessageBox.Show("Regulator already created");
                return;
            }


            dvr = new DataVertexRegulator() { Text = dv.Text, Element_id = dv.Element_id };
            vc = new VertexControl(dvr);


            if (m == 0)
            {
                dvr.Text = "Synchronous machine first ending: " + dv.Element_id;
                vc.SetPosition(new Point(x, y));
            }
            else
            {
                vc.SetPosition(m, n);
            }



            dvr.typeOfVertex = DataVertex.TypeOfVertex.REGULATOR_VERTEX;


            vc.Background = Brushes.Red;

            mainWindow.GlobalVertices.Add(dvr.Element_id, dvr);
            mainWindow.graphView.Area.AddVertexAndData(dvr, vc, true);

            Point point = CreatePointBasedOn(vc);
            if (graphState == GraphState.NORMAL)
            {
                x += 30;
                y += 30;
            }


            return;
        }

        private void CreateRegulatorPartialVertex(DataVertex dv, double m = 0, double n = 0)
        {
            VertexControl vc;
            DataVertexRegulatorPartial dvrp = new DataVertexRegulatorPartial() { Text = dv.Text, Element_id = dv.Element_id };
            dvrp.typeOfVertex = DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX;
            vc = new VertexControl(dvrp);

            if (m == 0)
            {
                dvrp.Text = "Synchronous machine second ending: " + dvrp.Element_id;
                vc.SetPosition(new Point(x, y));
            }
            else
            {
                vc.SetPosition(m, n);
            }

            //dv.typeOfVertex = DataVertex.TypeOfVertex.SEGMENT_VERTEX;
            mainWindow.GlobalVertices.Add(dvrp.Element_id, dvrp);
            vc.SetPosition(new Point(x, y));
            vc.Background = Brushes.IndianRed;
            mainWindow.graphView.Area.AddVertexAndData(dvrp, vc, true);

            Point point = CreatePointBasedOn(vc);

            if (graphState == GraphState.NORMAL)
            {
                x += 30;
                y += 30;
            }
            this.dvrp = dvrp;
            return;

        }

        internal void CreateComplexVertexWithPartialType(DataVertex existing_transformer_vertex,
        DataVertex existing_transformer_vertex_partial, DataVertex clicked_vertex,
        string length = null, CableConfiguration cableConfiguration = null)
        {
            mainWindow.GlobalVertices[existing_transformer_vertex.Element_id] = existing_transformer_vertex;
            string local_length = "0";

            foreach (VertexControl vc in Area.VertexList.Values)
            {


                DataVertex dv = (DataVertex)vc.Vertex;



                if (length != null)
                {
                    local_length = length;
                }

                if (dv.Text == clicked_vertex.Text)
                {
                    //update areas vertex
                    vc.Vertex = existing_transformer_vertex;

                    //remove old and add updated vertex
                    mainWindow.graphView.Area.VertexList.Remove(dv);
                    mainWindow.graphView.Area.VertexList.Add(existing_transformer_vertex, vc);

                    //create
                    CreateDataVertexBase(existing_transformer_vertex_partial.typeOfVertex, existing_transformer_vertex_partial.Element_id);

                    //connect new partial and existing

                    _ecFrom = GetVertexControlWithDataVertex(dv.Element_id);

                    VertexControl target = GetVertexControlWithDataVertex(existing_transformer_vertex_partial.Element_id);

                    graphState = GraphState.PARTIAL_CONNECTING;

                    ConnectEdges(target, int.Parse(local_length));

                    graphState = GraphState.NORMAL;
                    break;
                }



            }
        }

        public VertexControl GetVertexControlWithDataVertex(string node_id)
        {
            foreach (DataVertex dataVertex in mainWindow.graphView.Area.VertexList.Keys)
            {
                if (dataVertex.Element_id == node_id)
                {
                    return mainWindow.graphView.Area.VertexList[dataVertex];
                }
            }
            return null;
        }

        private Point CreatePointBasedOn(VertexControl vc)
        {
            if (vc == null)
            {
                return new Point();
            }

            Point point = new Point();
            point.X = vc.GetPosition().X;
            point.Y = vc.GetPosition().Y;
            return point;
        }

        public void ShowVertexDetails(DataVertex dv, VertexControl vc)
        {
            //open context vertex
            switch (dv.typeOfVertex)
            {
                case DataVertex.TypeOfVertex.REGULATOR_VERTEX:
                    w = new RegulatorView(dv);
                    break;
                case DataVertex.TypeOfVertex.TRANSFORMER_VERTEX:
                    w = new TransformerView(dv);
                    break;
                case DataVertex.TypeOfVertex.SPOT_LOAD_VERTEX:
                    SpotLoad spotLoad = FindSpotLoadVertex(dv);

                    if (spotLoad == null)
                    {
                        w = new SpotLoadView(dv as SpotLoad, false);
                    }
                    else
                    {
                        w = new SpotLoadView(spotLoad);
                    }
                    break;
                default:
                    return;

            }

            w.Show();
        }

        private SpotLoad FindSpotLoadVertex(DataVertex dv)
        {
            SpotLoad sl;
            if (mainWindow.GlobalSpotLoads.ContainsKey(dv.Element_id))
            {
                sl = mainWindow.GlobalSpotLoads[dv.Element_id];
                return sl;
            }
            return null;
        }

        public void PrepareComplexVertexForDelete(DataVertex dv)
        {
            VertexControl vc = GetVertexControlWithDataVertex(dv.Element_id);

            Point p = CreatePointBasedOn(vc);

            DeleteVertex(dv);


            if (dv.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX)
            {
                dvr = (DataVertexRegulator)dv;
                if (dvr.Line_Segment_To != null)
                {
                    if (mainWindow.GlobalVertices.Keys.Contains(dvr.Line_Segment_To))
                    {
                        DeleteVertex(mainWindow.GlobalVertices[dvr.Line_Segment_To]);
                        mainWindow.GlobalVertices.Remove(dvr.Line_Segment_To);
                        dvrp = null;
                    }
                }

                dvr = null;
            }
            else if (dv.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX)
            {
                dvt = (DataVertexTransformer)dv;
                if (dvt.Line_to != null)
                {
                    if (mainWindow.GlobalVertices.Keys.Contains(dvt.Line_to))
                    {
                        DeleteVertex(mainWindow.GlobalVertices[dvt.Line_to]);
                        mainWindow.GlobalVertices.Remove(dvt.Line_to);
                        dvtp = null;
                    }
                }

                dvt = null;
            }

            mainWindow.enableButtonIfEdgesExists();
            mainWindow.CloseAllWindows();
        }

        public void DeleteVertex(DataVertex dv)
        {
            //Area.RemoveVertex(globalVertices[dv.Text]);
            mainWindow.GlobalVertices.Remove(dv.Element_id);
            Console.WriteLine(mainWindow.GlobalVertices);
            bool is_empty = true;
            bool has_edge = false;


            is_empty = CheckIfZeroEdgesExist();


            //if 0 edges exist we can remove it
            RemoveIfZeroEdgesExist(is_empty, dv.Element_id);

            //if more then 0 edges exist we look for them
            has_edge = RemoveIfMoreThenZeroEdgesExist(dv);

            //if its not empty and its has edges, its third case, we delete it
            if (!is_empty && !has_edge)
            {
                VertexControl vc = GetVertexControlWithDataVertex(dv.Element_id);

                if(vc == null)
                {
                    return;
                }

                mainWindow.graphView.Area.RemoveVertex((vc.Vertex as DataVertex));
            }


            if (dv.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX)
            {
                dvr = null;

                if(dvrp != null)
                {
                    DeleteVertex(dvrp);
                    dvrp = null;
                }
             
            }


            if (dv.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX)
            {
                dvt = null;
                if (dvtp!=null)
                {
                    DeleteVertex(dvtp);
                    dvtp = null;
                }
                
                
            }


            //now we look for his edges and delete them
            foreach (string key in mainWindow.GlobalEdges.Keys)
            {
                foreach (DataEdge de in mainWindow.GlobalEdges[key])
                {
                    if (de.Target.Element_id == dv.Element_id)
                    {
                        mainWindow.GlobalEdges[key].Remove(de);

                        if (mainWindow.graphView.Area.EdgesList.ContainsKey(de))
                        {
                            mainWindow.graphView.Area.RemoveEdge(de);
                            break;
                        }
                    }
                }
            }


            if (dv.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX || dv.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX)
            {
                foreach (DataVertex temp in mainWindow.graphView.Area.VertexList.Keys)
                {
                    if (temp.Element_id == dv.Element_id)
                    {
                        mainWindow.graphView.Area.RemoveVertex(temp);
                        break;
                    }
                }
            }

            if (graphState == GraphState.NORMAL)
            {
                undoRedoCommander.addUndoCommand(new Command("DeleteVertex", dv));
            }



        }

        private bool RemoveIfMoreThenZeroEdgesExist(DataVertex dv)
        {
            bool has_edge = false;
            if (mainWindow.GlobalEdges.ContainsKey(dv.Element_id))
            {

                foreach (DataEdge de in mainWindow.GlobalEdges[dv.Element_id])
                {

                    if (mainWindow.graphView.Area.EdgesList.ContainsKey(de))
                    {
                        foreach (DataVertex temp in mainWindow.graphView.Area.VertexList.Keys)
                        {
                            if (temp.Element_id == dv.Element_id)
                            {
                                mainWindow.graphView.Area.RemoveVertex(temp);
                                has_edge = true;
                                mainWindow.GlobalVertices.Remove(dv.Element_id);
                                break;
                            }

                        }
                        mainWindow.graphView.Area.RemoveEdge(de);
                    }
                }

                mainWindow.GlobalEdges.Remove(dv.Element_id);
            }
            return has_edge;
        }

        private void RemoveIfZeroEdgesExist(bool is_empty, string element_id)
        {
            if (is_empty)
            {
                foreach (DataVertex temp in mainWindow.graphView.Area.VertexList.Keys)
                {
                    if (temp.Element_id == element_id)
                    {
                        mainWindow.graphView.Area.RemoveVertex(temp);
                        break;
                    }
                }
                return;
            }
        }

        private bool CheckIfZeroEdgesExist()
        {

            foreach (string key in mainWindow.GlobalEdges.Keys)
            {
                if (mainWindow.GlobalEdges[key].Count != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateVertices(DataVertex dataVertex, DataVertex dataVertexPartial, string old_value, string length = null)
        {
            string local_length = "0";

            if (dataVertex.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX)
            {
                dataVertex = (DataVertexRegulator)dataVertex;
                dataVertexPartial = (DataVertexRegulatorPartial)dataVertexPartial;
                local_length = length;
            }
            else if (dataVertex.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX)
            {
                dataVertex = (DataVertexTransformer)dataVertex;
                dataVertexPartial = (DataVertexTransformerPartial)dataVertexPartial;
            }

            //find vertex control from regulator nodes, so you can update graph
            VertexControl dataVertexRegulatorControl = GetVertexControlWithDataVertex(dataVertex.Element_id);
            VertexControl dataVertexRegulatorPartialControl = GetVertexControlWithDataVertex(old_value);

            //update their vertices
            dataVertexRegulatorControl.Vertex = dataVertex;
            dataVertexRegulatorPartialControl.Vertex = dataVertexPartial;

            //update main regulator
            mainWindow.GlobalVertices[dataVertex.Element_id] = dataVertex;

            //add new and remove old partial regulator

            if (mainWindow.GlobalVertices.ContainsKey(dataVertexPartial.Element_id) == false)
            {
                mainWindow.graphView.Area.VertexList.Add(new KeyValuePair<DataVertex, VertexControl>(dataVertexPartial, dataVertexRegulatorPartialControl));
            }
            else
            {
                mainWindow.graphView.Area.VertexList[dataVertexPartial] = dataVertexRegulatorPartialControl;
            }


            mainWindow.graphView.Area.VertexList.Remove(mainWindow.GlobalVertices[old_value]);
            mainWindow.graphView.Area.RemoveVertex(mainWindow.GlobalVertices[old_value]);

            //update global vertices
            mainWindow.GlobalVertices.Remove(old_value);
            mainWindow.GlobalVertices.Add(dataVertexPartial.Element_id, dataVertexPartial);


            foreach (Command c in undoRedoCommander.Undo_stack)
            {
                if (c.Name.Contains("Vertex"))
                {
                    DataVertex dv = c.Operands as DataVertex;

                    if (dv.Element_id == old_value)
                    {
                        c.Operands = dataVertexPartial;

                        break;
                    }
                }

            }

            //create new edge
            ReplaceEdgeTarget(dataVertex, dataVertexPartial, old_value, dataVertexRegulatorControl, dataVertexRegulatorPartialControl, local_length);
        }

        internal void DeleteConnection(DataVertex dv, string key)
        {

            if (!mainWindow.GlobalEdges.ContainsKey(dv.Element_id))
            {
                return;
            }

            foreach (DataEdge de in mainWindow.GlobalEdges[dv.Element_id])
            {
                if (de.Source.Element_id == dv.Element_id && de.Target.Element_id == key)
                {
                    mainWindow.GlobalEdges[dv.Element_id].Remove(de);

                    if (graphState == GraphState.NORMAL)
                    {
                        undoRedoCommander.addUndoCommand(new Command("DeleteEdge", de));
                    }


                    //dataGraph.RemoveEdge(de);
                    Area.RemoveEdge(de);
                    mainWindow.enableButtonIfEdgesExists();

                    break;
                }
            }



        }

        private void ReplaceEdgeTarget(DataVertex dataVertexRegulator, DataVertex dataVertexRegulatorPartial,
            string old_value, VertexControl dataVertexRegulatorControl,
            VertexControl dataVertexRegulatorPartialControl, string length)
        {
            foreach (DataEdge dataEdge in mainWindow.GlobalEdges[dataVertexRegulator.Element_id])
            {
                if (dataEdge.Target.Element_id == old_value)
                {

                    //setup new data edge
                    DataEdge de = new DataEdge(dataVertexRegulator, dataVertexRegulatorPartial);
                    if (dataEdge.Source.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX || dataEdge.Source.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX)
                    {
                        de.Configuration = new CableConfiguration()
                        {
                            Name = "xfm_1"
                        };
                    }
                    de.Length = Double.Parse(length);
                    de.Text = length;



                    //setup new edge control
                    EdgeControl edgeControl = new EdgeControl(dataVertexRegulatorControl, dataVertexRegulatorPartialControl, de);

                    setEdgesDashStyle(edgeControl, dataVertexRegulator.typeOfVertex);


                    //update area edges
                    mainWindow.graphView.Area.RemoveEdge(dataEdge);
                    mainWindow.graphView.Area.EdgesList.Remove(dataEdge);

                    if (dataVertexRegulator.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX)
                    {
                        mainWindow.graphView.Area.AddEdge(de, edgeControl, false);
                    }
                    else
                    {
                        mainWindow.graphView.Area.AddEdge(de, edgeControl, true);
                    }


                    //update global edges
                    mainWindow.GlobalEdges[dataVertexRegulator.Element_id].Remove(dataEdge);
                    mainWindow.GlobalEdges[dataVertexRegulator.Element_id].Add(de);

                    break;
                }
            }
        }

        private void setEdgesDashStyle(EdgeControl ec, DataVertex.TypeOfVertex typeOfVertex)
        {

            switch (typeOfVertex)
            {
                case DataVertex.TypeOfVertex.REGULATOR_VERTEX:
                    ec.DashStyle = ec.DashStyle = EdgeDashStyle.Solid;
                    break;
                case DataVertex.TypeOfVertex.TRANSFORMER_VERTEX:
                    ec.DashStyle = ec.DashStyle = EdgeDashStyle.Dot;
                    break;
                case DataVertex.TypeOfVertex.SPOT_LOAD_VERTEX:
                    ec.DashStyle = ec.DashStyle = EdgeDashStyle.Dash;
                    break;
                default:
                    return;
            }
        }

        public void ConnectEdges(VertexControl vc, double length = 0)
        {
            if (_ecFrom == null)
            {
                CreateVirtualEdge(vc, vc.GetPosition());
                _ecFrom = vc;
                HighlightBehaviour.SetHighlighted(_ecFrom, true);
                return;
            }

            if (_ecFrom == vc) return;

            if ((DataVertex)_ecFrom.Vertex == null) return;

            var data = new DataEdge((DataVertex)_ecFrom.Vertex, (DataVertex)vc.Vertex);
            data.Length = length;

            //if (graphState == GraphState.PARTIAL_CONNECTING)
            //{

            //    if ((_ecFrom.Vertex as DataVertex).typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX)
            //    {
            //        DataVertexRegulator dvr_temp = (vc.Vertex as DataVertexRegulator);
            //        dvr_temp.CompensatorSettingsB = dvrp.CompensatorSettingsB;
            //        dvr_temp.Line_Segment_To = dvrp.Element_id;
            //        dvr_temp.R_SettingB = dvrp.R_SettingB;
            //        dvr_temp.VoltageLevelB = dvrp.VoltageLevelB;
            //        dvr_temp.X_SettingB = dvrp.X_SettingB;
            //    }
            //    else if ((_ecFrom.Vertex as DataVertex).typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX)
            //    {
            //        DataVertexRegulator dvr_temp = (_ecFrom.Vertex as DataVertexRegulator);
            //        dvr_temp.CompensatorSettingsB = dvrp.CompensatorSettingsB;
            //        dvr_temp.Line_Segment_To = dvrp.Element_id;
            //        dvr_temp.R_SettingB = dvrp.R_SettingB;
            //        dvr_temp.VoltageLevelB = dvrp.VoltageLevelB;
            //        dvr_temp.X_SettingB = dvrp.X_SettingB;
            //    }

            //}

            var ec = new EdgeControl(_ecFrom, vc, data);

            ec.DashStyle = EdgeDashStyle.DashDot;

            if (CheckIfConnectionExists(data) == true)
            {
                MessageBox.Show("Already connected!");
                return;
            }

            if (graphState == GraphState.PARTIAL_CONNECTING)
            {
                (mainWindow.graphView.DataContext as GraphViewModel).AddEdge(data, ec, _ecFrom);
                return;
            }

            w = new Window();
            w = new EdgeConfigurationView((DataVertex)_ecFrom.Vertex, mainWindow.GlobalVertices, null,
                ((DataVertex)vc.Vertex).Element_id, mainWindow.GlobalEdges, false, vc, null,
                mainWindow, mainWindow.GlobalCableConfiguration, ec, data, _ecFrom);
            w.Show();
        }

        private bool CheckIfConnectionExists(DataEdge data)
        {

            foreach (DataEdge de in (mainWindow.graphView.DataContext as GraphViewModel).Area.EdgesList.Keys)
            {
                if (de.Source.Element_id == data.Source.Element_id
                    && de.Target.Element_id == data.Target.Element_id)
                {
                    return true;
                }
                else if (de.Source.Element_id == data.Target.Element_id
                    && de.Target.Element_id == data.Source.Element_id)
                {
                    return true;
                }
            }

            return false;
        }


        public void CreateVirtualEdge(VertexControl source, Point mousePos)
        {
            _edgeBp = new EdgeBlueprint(source, mousePos, Brushes.White);
            Area.InsertCustomChildControl(0, _edgeBp.EdgePath);
        }

        public void DestroyVirtualEdge()
        {
            ClearEdgeBp();
        }

        private void ClearEdgeBp()
        {
            if (_edgeBp == null) return;
            Area.RemoveCustomChildControl(_edgeBp.EdgePath);
            _edgeBp.Dispose();
            _edgeBp = null;
        }

        internal void AddEdge(DataEdge data, EdgeControl ec, VertexControl ecFrom, CableConfiguration cableConfiguration = null)
        {

            if (data == null) return;

            if (_ecFrom == null)
            {
                VertexControl vc = GetVertexControlWithDataVertex(data.Source.Element_id); ;
                _ecFrom = vc;
            }


            if (mainWindow.graphView.Area.EdgesList.ContainsKey(data)) return;

            if (graphState == GraphState.NORMAL)
            {
                data.Length = length;
                data.Text = length.ToString();
            }

            if (data.Source.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX || data.Source.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX)
            {
                cableConfiguration = new CableConfiguration()
                {
                    Name = "xfm_1"
                };
                data.Configuration = cableConfiguration;
            }

            if (mainWindow.GlobalEdges.ContainsKey(data.Source.Element_id))
            {
                mainWindow.GlobalEdges[data.Source.Element_id].Add(data);
            }
            else
            {
                mainWindow.GlobalEdges.Add(data.Source.Element_id, new List<DataEdge>()
                {
                    data
                });
            }


            bool its_needed = CheckIfLabelNeeded(data);

            if (its_needed)
            {
                mainWindow.graphView.Area.AddEdge(data, ec, true);
            }
            else
            {
                mainWindow.graphView.Area.AddEdge(data, ec, false);
            }



            setEdgesDashStyle(ec, data.Source.typeOfVertex);

            HighlightBehaviour.SetHighlighted(_ecFrom, true);
            _ecFrom = null;
            DestroyVirtualEdge();
            mainWindow.segmentData.IsEnabled = true;


            if (graphState == GraphState.NORMAL)
            {
                undoRedoCommander.addUndoCommand(new Command("AddEdge", data));
            }

        }

        private bool CheckIfLabelNeeded(DataEdge data)
        {
            if (data.Source.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX
            && data.Target.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX)
            {
                return false;
            }
            else if (data.Source.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX
            && data.Target.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX)
            {
                return false;
            }


            if (data.Target.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX
                && data.Source.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX)
            {
                return false;
            }
            else if (data.Target.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX
            && data.Source.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX)
            {
                return false;
            }

            return true;
        }

        public void Undo_Click()
        {

            if (graphState == GraphState.NORMAL)
            {
                if (undoRedoCommander.Undo_stack.Count > 0)
                {
                    undoRedoCommander.UndoCommandExecute();
                }
            }

        }


        public void Redo_click()
        {
            if (graphState == GraphState.NORMAL)
            {
                if (undoRedoCommander.Redo_stack.Count > 0)
                {
                    undoRedoCommander.RedoCommandExecute();
                }
            }
        }

        internal void RecreateSerializedEdgeData(DataVertex source, DataVertex target, VertexControl vc, double length)
        {
            DataEdge de = new DataEdge(source, target);

            VertexControl vc1 = GetVertexControlWithDataVertex(source.Element_id);
            VertexControl vc2 = GetVertexControlWithDataVertex(target.Element_id);

            this.length = length;

            EdgeControl ec = new EdgeControl(vc1, vc2, de);


            AddEdge(de, ec, vc1);
        }

        internal void RecreateSerializedVertexData(List<GraphSerializationData> gds)
        {
            foreach (GraphSerializationData x in gds)
            {
                DataVertex dv = x.Data as DataVertex;


                switch (dv.typeOfVertex)
                {
                    case DataVertex.TypeOfVertex.REGULATOR_VERTEX:
                        CreateRegulatorVertex(dv, x.Position.X, x.Position.Y);
                        break;
                    case DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX:
                        CreateRegulatorPartialVertex(dv, x.Position.X, x.Position.Y);
                        break;
                    case DataVertex.TypeOfVertex.TRANSFORMER_VERTEX:
                        CreateTransformerVertex(dv, x.Position.X, x.Position.Y);
                        break;
                    case DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX:
                        CreateTransformerPartialVertex(dv, x.Position.X, x.Position.Y);
                        break;
                    case DataVertex.TypeOfVertex.SPOT_LOAD_VERTEX:
                        CreateSpotLoadVertex(dv, x.Position.X, x.Position.Y);
                        break;
                }

                //CreateVertexWithType(dv.typeOfVertex, dv, first_creation, false, x.Position.X, x.Position.Y);

            }
        }
    }


}





// undo stack commands