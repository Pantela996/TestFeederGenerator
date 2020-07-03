using FTN;
using GraphX.Controls;
using QuickGraph.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestFeeder.ViewModels;
using TestFeeder.Views.Controls;
using TestFeederGenerator.Models;

namespace TestFeeder.UndoRedo
{


    //Logic: We are keeping track of 2 stacks empty at start, after first user action (add,remove edge/vertex), undo stack gets inversed value.
    //After clicking undo button we are executing command and adding redo command with inverted, just executed, undo command


    // Stacks are keeping tracks of command values, which contains name (eg. AddVertex), operands which are type of (DataVertex,DataEdge),
    //and optionally point of vertex (if deleted)
    public class UndoRedoCommander
    {
        private Stack<Command> undo_stack = new Stack<Command>();
        private Stack<Command> redo_stack = new Stack<Command>();
        private Object context;
        private DataEdge partialEdge;

        private GraphViewModel graphViewModel;
        private MainWindow _mn;

        public Stack<Command> Undo_stack
        {
            get { return undo_stack; }
            set { undo_stack = value; }
        }

        public Stack<Command> Redo_stack
        {
            get { return redo_stack; }
            set { redo_stack = value; }
        }


        public UndoRedoCommander(Object ctx)
        {
            context = ctx;
            graphViewModel = (App.Current.MainWindow as MainWindow).graphView.DataContext as GraphViewModel;
            _mn = App.Current.MainWindow as MainWindow;
        }


        //we are doing inversion
        public void addUndoCommand(Command command)
        {
            switch(command.Name)
            {
                case "AddVertex":
                    undo_stack.Push(new Command("DeleteVertex",command.Operands,command.Position));
                    break;

                case "DeleteVertex":
                    undo_stack.Push(new Command("AddVertex", command.Operands,command.Position));
                    break;

                case "AddEdge":
                    undo_stack.Push(new Command("DeleteEdge", command.Operands));
                    break;

                case "DeleteEdge":
                    undo_stack.Push(new Command("AddEdge", command.Operands));
                    break;

            }

            (App.Current.MainWindow as MainWindow).graphView.undo.IsEnabled = true;
            

        }

        //inversion for redo stack
        public void addRedoCommand(Command command)
        {
            switch (command.Name)
            {

                case "AddVertex":
                    redo_stack.Push(new Command("DeleteVertex", command.Operands, command.Position));
                    break;

                case "DeleteVertex":
                    redo_stack.Push(new Command("AddVertex", command.Operands, command.Position));
                    break;

                case "AddEdge":
                    redo_stack.Push(new Command("DeleteEdge", command.Operands));
                    break;

                case "DeleteEdge":
                    redo_stack.Push(new Command("AddEdge", command.Operands));
                    break;

            }
            (App.Current.MainWindow as MainWindow).graphView.redo.IsEnabled = true;

        }


        // if its empty cant be clicked (source of bugs)
        public void UndoCommandExecute()
        {
    
            Command c = undo_stack.Pop();

            if (c.IsActive == false) return;

            executeCommand(c, "undo");

            //AddAdditionalCommandIfPartialVertex(c);

            addRedoCommand(c);

            

            if(undo_stack.Count == 0)
            {
                ((App.Current.MainWindow as MainWindow).graphView).undo.IsEnabled = false;
            }

        }

        private void AddAdditionalCommandIfPartialVertex(Command c)
        {
            if ((c.Operands as DataEdge) != null) return;

            if ((c.Operands as DataVertex).typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX || (c.Operands as DataVertex).typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX)
            {
                

                foreach (DataEdge de in graphViewModel.Area.EdgesList.Keys)
                {

                    if(de.Source.Element_id == graphViewModel.dvt.Element_id)
                    {
                        if (de.Target.Element_id == (c.Operands as  DataVertex).Element_id)
                        {
                            addUndoCommand(new Command("AddEdge", de));
                        }
                    }

                }
                
            }
        }

        // if its empty cant be clicked (source of bugs)
        public void RedoCommandExecute()
        {


            Command c = redo_stack.Pop();

            if (c.IsActive == false) return;

            executeCommand(c, "redo");

            addUndoCommand(c);

            if (redo_stack.Count == 0)
            {
                ((App.Current.MainWindow as MainWindow).graphView).redo.IsEnabled = false;
            }
        }


        //read command name and based on that do execute command with special conditions
        private void executeCommand(Command c, string choice)
        {
            DataVertex dv = new DataVertex();
            DataEdge de = new DataEdge();
            VertexControl vc;
            VertexControl vc2;

            if (choice == "undo")
            {
                graphViewModel.graphState = GraphViewModel.GraphState.UNDO;
            }
            else
            {
                graphViewModel.graphState = GraphViewModel.GraphState.REDO;
            }

            switch (c.Name)
            {
                case "AddVertex":
                    dv = c.Operands as DataVertex;

                    int id = int.Parse(dv.Element_id);
                    while (_mn.GlobalVertices.ContainsKey(dv.Element_id))
                    {
                        id++;
                        dv.Element_id = id.ToString();
                    }

                    graphViewModel.CreateDataVertexBase(dv.typeOfVertex, dv.Element_id);

                    vc = graphViewModel.GetVertexControlWithDataVertex(dv.Element_id);
                    vc.SetPosition(c.Position);

                    break;


                case "DeleteVertex":
                    dv = c.Operands as DataVertex;

                    vc = graphViewModel.GetVertexControlWithDataVertex(dv.Element_id);

                    if(vc == null)
                    {
                        break;
                    }

                    Point p = new Point();
                    p.X = vc.GetPosition().X;
                    p.Y = vc.GetPosition().Y;

                    graphViewModel.DeleteVertex(dv);
                    

                    c.Position = p;

                    break;

                case "AddEdge":
                    de = c.Operands as DataEdge;

                    if(de == null)
                    {
                        graphViewModel.graphState = GraphViewModel.GraphState.NORMAL;
                        return;
                    }

                    vc = graphViewModel.GetVertexControlWithDataVertex(de.Source.Element_id);
                    vc2 = graphViewModel.GetVertexControlWithDataVertex(de.Target.Element_id);


                    if (vc ==null)
                    {
                        vc = new VertexControl(de.Source);
                    }
                    EdgeControl ec = new EdgeControl(vc,vc2,de);



                    graphViewModel.AddEdge(de,ec,vc);
                    
                    break;

                case "DeleteEdge":
                    de = c.Operands as DataEdge;

                    if (c.IsActive == false) return;


                    graphViewModel.DeleteConnection(de.Source, de.Target.Element_id);
                    break;
            }

            graphViewModel.graphState = GraphViewModel.GraphState.NORMAL;
        }


    }
}
