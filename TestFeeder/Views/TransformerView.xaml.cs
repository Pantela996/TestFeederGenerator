using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using TestFeeder.Models;
using TestFeeder.ViewModels;
using TestFeeder.Views.Controls;
using TestFeederGenerator.Models;

namespace TestFeeder.Views
{
    /// <summary>
    /// Interaction logic for TransformerDetails.xaml
    /// </summary>
    /// 

    // Class that only execute glue code, creating objects and passing them to GraphViewModel, that holds all logic
    // have 2 use scenarions: Create and Update
    public partial class TransformerView : Window
    {

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private DataVertex dv;
        private Dictionary<string, DataVertex> globalVertices;
        private Dictionary<string, List<DataEdge>> globalEdges;
        private MainWindow _mn;
        private string last_value;
        private string second_component;

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public TransformerView()
        {
            InitializeComponent();
        }

        public TransformerView(DataVertex dv)
        {
            InitializeComponent();

            this.dv = dv;
            this._mn = App.Current.MainWindow as MainWindow;
            this.globalVertices = _mn.GlobalVertices;
            this.globalEdges = _mn.GlobalEdges;
            
            

            DataVertexTransformer dataVertexTransformer = (DataVertexTransformer) dv; 
            
            //if node already consist some data
            if (dataVertexTransformer.NameA != null)
            {
                populateViewIfObjectExists(dataVertexTransformer);
            }

            if ((_mn.graphView.DataContext as GraphViewModel).dvtp != null)
            {
                lineTo.Text = (_mn.graphView.DataContext as GraphViewModel).dvtp.Element_id;
                last_value = lineTo.Text;
                lineTo.IsReadOnly = true;
            }
            else
            {
                //lineTo.Text = dataVertexTransformer.Line_to;
                //lineTo.IsReadOnly = true;
            }

            setupViewDeafultValues();
        }

        private void setupViewDeafultValues()
        {
            linefrom.Text = dv.Element_id;
        }

        private void populateViewIfObjectExists(DataVertexTransformer dataVertexTransformer)
        {
            dynamicButton.Content = "Update";
            dynamicButton2.IsEnabled = true;
            first_name.Text = dataVertexTransformer.NameA;
            second_name.Text = dataVertexTransformer.NameB;
            second_component = dataVertexTransformer.Line_to;
            apparent_power_first_part.Text = dataVertexTransformer._kVA_A.ToString();
            apparent_power_second_part.Text = dataVertexTransformer._kVA_B.ToString();
            high_limit_first_part.Text = dataVertexTransformer._kV_HighA.ToString();
            low_limit_first_part.Text = dataVertexTransformer._kV_LowA.ToString();
            high_limit_second_part.Text = dataVertexTransformer._kV_HighB.ToString();
            low_limit_second_part.Text = dataVertexTransformer._kV_LowB.ToString();
            r_percentage__first_part.Text = dataVertexTransformer.RPercentageA.ToString();
            r_percentage__second_part.Text = dataVertexTransformer.RPercentageB.ToString();
            x_percentage__first_part.Text = dataVertexTransformer.XPercentageA.ToString();
            x_percentage__second_part.Text = dataVertexTransformer.XPercentageB.ToString();

            
            last_value = lineTo.Text;
        }

        private void bandwidthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dynamicButton_Click(object sender, RoutedEventArgs e)
        {


            if (!allFilled())
            {
                MessageBox.Show("You have to enter all data");
                return;
            }

            if((_mn.graphView.DataContext as GraphViewModel).dvtp == null)
            {
                if (globalVertices.ContainsKey(lineTo.Text) && second_component != lineTo.Text)
                {
                    MessageBox.Show("Node with that ID already exists, change Line segment to input.");
                    return;
                }
            }

          

            DataVertexTransformer dataVertexTransformer = setupDataVertexTransformerProperties();

            DataVertexTransformerPartial dvtp;


            if ((_mn.graphView.DataContext as GraphViewModel).dvtp != null)
            {
                dvtp = (_mn.graphView.DataContext as GraphViewModel).dvtp;
                dvtp.Text = "Second transformer ending: " + lineTo.Text;
                dynamicButton.Content = "Update";
            }
            else
            {
                DataVertexTransformerPartial dataVertexTransformerPartial = new DataVertexTransformerPartial()
                {
                    Text = "Second transformer ending: " + lineTo.Text,
                    typeOfVertex = DataVertex.TypeOfVertex.TRANSFORMER_PARTIAL_VERTEX,
                    Element_id = dataVertexTransformer.Line_to,
                    Name = dataVertexTransformer.NameB,
                    _kVA = dataVertexTransformer._kVA_B,
                    _kV_High = dataVertexTransformer._kV_HighB,
                    _kV_Low = dataVertexTransformer._kV_LowB,
                    RPercentage = dataVertexTransformer.RPercentageB,
                    XPercentage = dataVertexTransformer.XPercentageB
                };
                (_mn.graphView.DataContext as GraphViewModel).dvtp = dataVertexTransformerPartial;
                dvtp = dataVertexTransformerPartial;
            }

            dvtp.Element_id = lineTo.Text;

            if (dynamicButton.Content.ToString() == "Update")
            {
                (_mn.graphView.DataContext as GraphViewModel).graphState = GraphViewModel.GraphState.UPDATE;
                (_mn.graphView.DataContext as GraphViewModel).UpdateVertices(dataVertexTransformer, dvtp, last_value);
                (_mn.graphView.DataContext as GraphViewModel).graphState = GraphViewModel.GraphState.NORMAL;
                last_value = dataVertexTransformer.Line_to;
            }
            else
            {
                (_mn.graphView.DataContext as GraphViewModel).CreateComplexVertexWithPartialType(dataVertexTransformer, dvtp, dv);
            }

            _mn.CloseAllWindows();

        }

        private DataVertexTransformer setupDataVertexTransformerProperties()
        {
            DataVertexTransformer dataVertexTransformer = new DataVertexTransformer()
            {
                //populate data
                Text = dv.Text,
                Element_id = linefrom.Text,
                typeOfVertex = dv.typeOfVertex,
                NameA = first_name.Text,
                NameB = second_name.Text,
                _kVA_A = Double.Parse(apparent_power_first_part.Text),
                _kVA_B = Double.Parse(apparent_power_second_part.Text),
                _kV_HighA = Double.Parse(high_limit_first_part.Text),
                _kV_HighB = Double.Parse(high_limit_second_part.Text),
                _kV_LowA = Double.Parse(low_limit_first_part.Text),
                _kV_LowB = Double.Parse(low_limit_second_part.Text),
                RPercentageA = Double.Parse(r_percentage__first_part.Text),
                RPercentageB = Double.Parse(r_percentage__second_part.Text),
                XPercentageA = Double.Parse(x_percentage__first_part.Text),
                XPercentageB = Double.Parse(x_percentage__second_part.Text),
                Line_from = linefrom.Text,
                Line_to = lineTo.Text
            };
            return dataVertexTransformer;
        }

        private void dynamicButton2_Click(object sender, RoutedEventArgs e)
        {
            (_mn.graphView.DataContext as GraphViewModel).PrepareComplexVertexForDelete(dv);
        }

        private bool allFilled()
        {
            if (lineTo.Text != "" && first_name.Text != "" && second_name.Text != "" && apparent_power_first_part.Text != "" && apparent_power_second_part.Text != "" && high_limit_first_part.Text != "" && high_limit_second_part.Text != "" && low_limit_first_part.Text != "" && low_limit_second_part.Text != "" && r_percentage__first_part.Text != "" && r_percentage__second_part.Text != "" && x_percentage__first_part.Text != "" && x_percentage__second_part.Text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
