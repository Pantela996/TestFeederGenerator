using FTN;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TestFeeder.FileSerialization;
using TestFeeder.Models;
using TestFeeder.UndoRedo;
using TestFeeder.ViewModels;
using TestFeeder.Views;
using TestFeederGenerator.Models;

namespace TestFeeder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    //Passes value to Logic Core, only have defined methods for user actions, mediator between view and view model logic,
    // holds instances for ViewModels and Views

    [Serializable]
    public partial class MainWindow : Window
    {

        private Dictionary<string, DataVertex> globalVertices;

        //TO BE: Dictionary<string, List<DataEdge>> globalEdges;
        private Dictionary<string, List<DataEdge>> globalEdges;
        private Dictionary<string, CableConfiguration> globalCableConfiguration;
        private Dictionary<Type, Dictionary<string, IdentifiedObject>> globalComponentDictionary;
        private Dictionary<string, SpotLoad> globalSpotLoads;

        private CIMObjectModelLoader cIMObjectModelLoader;
        private Dictionary<Type, Dictionary<string, IdentifiedObject>> modelEntities;

        public CreateConfigurationView ccView;
        public EditConfigurationView editCView;

        public GraphViewModel graphViewModel;
        public CreateConfigurationViewModel ccViewModel;

        Window w;
        

        public Dictionary<string, SpotLoad> GlobalSpotLoads
        {
            get { return globalSpotLoads; }
            set { globalSpotLoads = value; }
        }


        public Dictionary<string, DataVertex> GlobalVertices
        {
            get { return globalVertices; }
            set { globalVertices = value; }
        }

        public Dictionary<string, List<DataEdge>> GlobalEdges
        {
            get { return globalEdges; }
            set { globalEdges = value; }
        }


        public Dictionary<string, CableConfiguration> GlobalCableConfiguration
        {
            get { return globalCableConfiguration; }
            set { globalCableConfiguration = value; }
        }


        public MainWindow()
        {
            InitializeComponent();

            //setup data
            globalEdges = new Dictionary<string, List<DataEdge>>();
            globalVertices = new Dictionary<string, DataVertex>();
            globalCableConfiguration = new Dictionary<string, CableConfiguration>();
            globalComponentDictionary = new Dictionary<Type, Dictionary<string, IdentifiedObject>>();
            modelEntities = new Dictionary<Type, Dictionary<string, IdentifiedObject>>();
            globalSpotLoads = new Dictionary<string, SpotLoad>();

            //setup window properties
            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Maximized;

            graphView.DataContext = graphViewModel;
            graphViewModel.Area = graphView.Area;

            AddDefaultConfig();

            ccView = new CreateConfigurationView(this,globalCableConfiguration);

            editCView = new EditConfigurationView(globalCableConfiguration,this);

            enableButtonIfEdgesExists();



        }

        
        //Initital value for xfm-1 configuration
        private void AddDefaultConfig()
        {
            CableConfiguration cc = new CableConfiguration()
            {
                Name = "xfm-1",
                Phasing = CableConfiguration.PhasingEnum.ABC,
                Primar_value = new ComplexValue()
                {
                    Real_part = 1,
                    Imaginary_part = 1
                },
                Secondary_value = new List<ComplexValue>()
                {
                    new ComplexValue()
                    {
                        Real_part = 1,
                        Imaginary_part = 1
                    },
                    new ComplexValue()
                    {
                        Real_part = 1,
                        Imaginary_part = 1
                    }
                },
                Terciar_value = new List<ComplexValue>()
                {
                    new ComplexValue()
                    {
                        Real_part = 1,
                        Imaginary_part = 1
                    },
                    new ComplexValue()
                    {
                        Real_part = 1,
                        Imaginary_part = 1
                    },
                    new ComplexValue()
                    {
                        Real_part = 1,
                        Imaginary_part = 1
                    }
                },

                Cable = new Models.Cable()
                {
                    MaterialKind = Models.Cable.MaterialEnum.aluminum,
                    SizeDescription = "111",
                    PhaseWireSpacing = 1,
                    Radius = 1,
                    RatedCurrent = 1
                },
                Susceptance = 1
            };
            globalCableConfiguration.Add(cc.Name, cc);
        }



        public void enableButtonIfEdgesExists()
        {
            bool isEnabled = true;
            //tabItem.IsEnabled = false;
            foreach (string x in globalEdges.Keys)
            {
                if (globalEdges[x].Count != 0)
                {
                    isEnabled = true;
                }
            }

            tabItem.IsEnabled = isEnabled;

        }

        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            List<DataEdge> helper_de = new List<DataEdge>();
            //MessageBox.Show("here");
            foreach (string key in globalEdges.Keys)
            {
                foreach (DataEdge de in globalEdges[key])
                {
                    helper_de.Add(de);
                }
            }

            segmentData.ItemsSource = helper_de;
        }

        public void CloseAllWindows(MainWindow mn = null)
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
            {
                if (App.Current.Windows[intCounter] == this)
                {
                    return;
                }
                App.Current.Windows[intCounter].Close();
            }
            
        }

        private void CreateNewConfig(object sender, RoutedEventArgs e)
        {
            var w = new Window();
            w = new CreateConfigurationView(this, globalCableConfiguration);
            w.Show();
        }

        private void GetObjectModel(object sender, MouseButtonEventArgs e)
        {

            cIMObjectModelLoader = new CIMObjectModelLoader();
            modelEntities = cIMObjectModelLoader.CreateObjectModel(globalVertices, globalEdges, globalCableConfiguration, globalSpotLoads);
            string map = createEntityMap(modelEntities);
            entity_map.Text = map;

        }

        private string createEntityMap(Dictionary<Type, Dictionary<string, IdentifiedObject>> modelEntities)
        {
            StringBuilder sb = new StringBuilder();
            int counter = 0;
            string a = "Terminal on Connectivity Node: ";
            string b = " Connected to Terminal on Connectivity Node: ";
            string xtemp = "";
            foreach (Type type in modelEntities.Keys)
            {
                sb.Append(type.Name);
                sb.Append("\n");
                sb.Append("\t");


                foreach (string temp in modelEntities[type].Keys)
                {

                    if (type.Name == "ConnectivityNode")
                    {
                        sb.Append("Connectivity Node on Spot Load: ");
                        sb.Append(modelEntities[type][temp].Name);
                        sb.Append("\n");
                        sb.Append("\t");
                        continue;
                    }

                    if (type.Name == "Terminal")
                    {
                        sb.Append(a);
                        sb.Append(modelEntities[type][temp].Name);

                        
                        
                        if (counter % 2 == 1)
                        {
                            sb.Append("\n");
                            sb.Append("\t");
                        }

                        xtemp = a;
                        a = b;
                        b = xtemp;


                        counter++;

                        continue;
                    }

                    sb.Append(modelEntities[type][temp].Name);
                    sb.Append("\n");
                    sb.Append("\t");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private void EditConfig(object sender, RoutedEventArgs e)
        {
            editCView = new EditConfigurationView(globalCableConfiguration, this);
            w = editCView;
            w.Show();
        }



        private void CreateNewSpotLoad(object sender, RoutedEventArgs e)
        {

            var w = new Window();
            //w.Content = new CreateSpotLoad(this, globalVertices,globalSpotLoads);
            w.Show();

        }

        internal void CreateSpotLoad(SpotLoad current_spot_load)
        {
            

            spotLoadTab._globalSpotLoads = globalSpotLoads;
            spotLoadTab.populateListView();

   
        }

        internal void UpdateSpotLoad(SpotLoad sl)
        {
            //w.Content = new SpotLoadView(this, globalVertices, globalSpotLoads, sl
            w = new SpotLoadView(sl);
            w.Show();

        }

        internal void DeleteSpotLoad(String key)
        {
            globalSpotLoads.Remove(key);
            spotLoadTab._globalSpotLoads = globalSpotLoads;
            spotLoadTab.populateListView();
        }

        private void mockup_data_Click(object sender, RoutedEventArgs e)
        {
            //graphView.mockData();
        }

        private void graph_area_Click(object sender, RoutedEventArgs e)
        {
            graphView.ClearArea();
            entity_map_tab.IsEnabled = true;
        }

        private void LoadSerializedData(object sender, RoutedEventArgs e)
        {
            string vertex_file;
            string edge_file;
            string cable_file;


            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string fileName = openFileDialog.FileName;
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                string[] base_path = openFileDialog.FileName.Split('_');

                if (base_path.Length < 2)
                {
                    System.Windows.MessageBox.Show("Invalid file");
                    return;
                }   
                vertex_file = base_path[0] + "_vertex_data";
                edge_file = base_path[0] + "_edge_data";
                cable_file = base_path[0] + "_cable_config";
            }
            else
            {
                System.Windows.MessageBox.Show("Invalid file");
                return;
            }

            


            graphView.DeserializeVertexData(vertex_file);
            graphView.DeserializeEdgeData(edge_file);
            graphView.DeserializeConfigurationData(cable_file);
            (this.graphView.DataContext as GraphViewModel).undoRedoCommander.Undo_stack = new Stack<Command>();
            (this.graphView.DataContext as GraphViewModel).undoRedoCommander.Redo_stack = new Stack<Command>();
            graphView.redo.IsEnabled = false;
            graphView.undo.IsEnabled = false;
        }

        private void SaveLoadout(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.FileName.Contains("_"))
            {
                System.Windows.MessageBox.Show("Invalid name, can't containt \"_\" ");
                return;
            }

            graphView.SerializeVertexData(saveFileDialog);
            graphView.SerializeEdgeData(saveFileDialog);
            graphView.SerializeConfigurationData(saveFileDialog);

        }

        private void HideShowToolbar(object sender, RoutedEventArgs e)
        {
            graphView.ToolbarVisibility();
        }

        private void ExportCIMXML(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            XMLWriter xmlWriter = new XMLWriter(modelEntities);
            xmlWriter.WriteToFile(saveFileDialog.FileName);
        }
    }
}

