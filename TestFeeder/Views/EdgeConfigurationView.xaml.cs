using GraphX.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestFeeder.Models;
using TestFeeder.ViewModels;
using TestFeederGenerator.Models;

namespace TestFeeder.Views
{
    /// <summary>
    /// Interaction logic for EdgeConfiguration.xaml
    /// </summary>
    /// 


    // Class that only execute glue code (connecting view to main logic), creating objects and passing them to GraphViewModel, that holds all logic
    // Have 2 use scenarions: Create and Update
    public partial class EdgeConfigurationView : Window
    {

        private DataVertex _dv;
        private Dictionary<string, DataVertex> _globalVertices;
        private TestFeederGenerator.Models.GraphX _graph;
        private string _key;
        private Dictionary<string, List<DataEdge>> _globalEdges;
        private bool _isUpdate;
        private VertexControl _vc;
        private GraphArea _graphArea;
        private MainWindow _mn;
        private Dictionary<string, CableConfiguration> _globalCableConfiguration;
        private EdgeControl _ec;
        private DataEdge data;
        private MainWindow mainWindow;
        private VertexControl _ecFrom;


        public EdgeConfigurationView(DataVertex dv, Dictionary<string, DataVertex> globalVertices,
            TestFeederGenerator.Models.GraphX graph, string key, Dictionary<string, List<DataEdge>>  globalEdges, 
            bool isUpdate, VertexControl vc, GraphArea area, MainWindow mn, 
            Dictionary<string, Models.CableConfiguration> globalCableConfiguration,
            EdgeControl ec = null, DataEdge de = null, VertexControl _ecFrom=null)
        {
            InitializeComponent();
            _ec = ec;
            data = de;
            this._ecFrom = _ecFrom;
            initializeLocalVariablesNewEdge(dv,graph,globalVertices, key, globalEdges, isUpdate, area, vc, mn, globalCableConfiguration);
            mainWindow = App.Current.MainWindow as MainWindow;


        }



        public EdgeConfigurationView(DataVertex dv, Dictionary<string, DataVertex> globalVertices, TestFeederGenerator.Models.GraphX graph, string key, Dictionary<string, List<DataEdge>> globalEdges, DataEdge de,bool isUpdate, MainWindow mn, VertexControl vc, Dictionary<string, Models.CableConfiguration> globalCableConfiguration)
        {
            //for update
            InitializeComponent();

            initializeLocalVariablesUpdateEdge(dv, graph, globalVertices, key, globalEdges, isUpdate, de, vc, mn, globalCableConfiguration);


        }



        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                txtBox.Text = txtBox.Text.Remove(txtBox.Text.Length - 1);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(txtBox.Text == ""))
            {
                if (cbo.SelectedItem == null)
                {
                    MessageBox.Show("You have to enter configuration, if you dont have select option, create configuration first.");
                    return;
                }
                _mn.CloseAllWindows();
                Application.Current.MainWindow = _mn;
                //Application.Current.Windows[0].Close();
                string cableConfiguration_key = (string)cbo.SelectedItem;
                (_mn.graphView.DataContext as GraphViewModel).length = Double.Parse(txtBox.Text);
                if (data!=null)
                {
                    data.Configuration = mainWindow.GlobalCableConfiguration[cableConfiguration_key];
                    (_mn.graphView.DataContext as GraphViewModel).AddEdge(data, _ec, _ecFrom);
                }

            }  
        }

        private void initializeLocalVariablesNewEdge(DataVertex dv, TestFeederGenerator.Models.GraphX graph, Dictionary<string, DataVertex> globalVertices, string key, Dictionary<string, List<DataEdge>> globalEdges, bool isUpdate, GraphArea area, VertexControl vc, MainWindow mn, Dictionary<string, CableConfiguration> globalCableConfiguration)
        {
            if (dv == null)
            {
                return;
            }
            _dv = dv;
            _graph = graph;
            _globalVertices = globalVertices;
            _key = key;
            _globalEdges = globalEdges;
            idLabel.Content = "Nodes to connect: " + dv.Text + " -> " + key;
            _isUpdate = isUpdate;
            _vc = vc;
            _graphArea = area;
            _mn = mn;
            _globalCableConfiguration = globalCableConfiguration;
            cbo.ItemsSource = _globalCableConfiguration.Keys;
            cbo.SelectedIndex = 0;
        }

        private void initializeLocalVariablesUpdateEdge(DataVertex dv, TestFeederGenerator.Models.GraphX graph, Dictionary<string, DataVertex> globalVertices, string key, Dictionary<string, List<DataEdge>> globalEdges, bool isUpdate, DataEdge de, VertexControl vc, MainWindow mn, Dictionary<string, CableConfiguration> globalCableConfiguration)
        {
            _dv = dv;
            _graph = graph;
            _globalVertices = globalVertices;
            _key = key;
            _globalEdges = globalEdges;
            idLabel.Content = "Nodes to connect: " + dv.Text + " -> " + key;
            txtBox.Text = de.Length.ToString();
            _isUpdate = isUpdate;
            _mn = mn;
            _vc = vc;
            _globalCableConfiguration = globalCableConfiguration;
            cbo.ItemsSource = _globalCableConfiguration.Keys;
            cbo.SelectedIndex = 0;
        }

    }
}
