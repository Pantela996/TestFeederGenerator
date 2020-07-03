using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TestFeeder.Models;
using TestFeeder.ViewModels;
using TestFeederGenerator.Models;



namespace TestFeeder.Views
{
    /// <summary>
    /// Interaction logic for RegulatorDetails.xaml
    /// </summary>
    /// 


    // Class that only execute glue code (connecting view to main logic), creating objects and passing them to GraphViewModel, that holds all logic
    // have 2 use scenarions: Create and Update

    public partial class RegulatorView : Window
    {
        private DataVertex dv;
        private Dictionary<string, DataVertex> globalVertices;
        private Dictionary<string, List<DataEdge>> globalEdges;
        private MainWindow _mn;
        private string last_value;

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public RegulatorView()
        {
            InitializeComponent();
        }



        public RegulatorView(DataVertex dv)
        {
            InitializeComponent();

            this.dv = dv;
            this._mn = App.Current.MainWindow as MainWindow;
            this.globalVertices = _mn.GlobalVertices;
            this.globalEdges = _mn.GlobalEdges;
           

            DataVertexRegulator dvr = (DataVertexRegulator)dv;

            if (dvr.RegulatorID != null)
            {

                populateViewIfObjectExists(dvr);

            }

            setupViewDeafultValues();

        }

        private void setupViewDeafultValues()
        {

            linefrom.Text = dv.Element_id;
            if ((_mn.graphView.DataContext as GraphViewModel).dvrp != null)
            {
                lineTo.Text = (_mn.graphView.DataContext as GraphViewModel).dvrp.Element_id;
                lineTo.IsReadOnly = true;
            }
            location.Content = "Location: " + dv.Text;
            phaseEnum.ItemsSource = Enum.GetValues(typeof(DataVertexRegulator.enumPhases)).Cast<DataVertexRegulator.enumPhases>();
            phaseEnum.SelectedIndex = 0;
            monitoringPhase.ItemsSource = Enum.GetValues(typeof(DataVertexRegulator.enumMonitoring)).Cast<DataVertexRegulator.enumMonitoring>();
            monitoringPhase.SelectedIndex = 0;
            connection.ItemsSource = Enum.GetValues(typeof(DataVertexRegulator.enumConnection)).Cast<DataVertexRegulator.enumConnection>();
            connection.SelectedIndex = 0;
            compensatorSettingsA.ItemsSource = Enum.GetValues(typeof(DataVertexRegulator.enumCompensatorSettings)).Cast<DataVertexRegulator.enumCompensatorSettings>();
            compensatorSettingsA.SelectedIndex = 0;
            compensatorSettingsB.ItemsSource = Enum.GetValues(typeof(DataVertexRegulator.enumCompensatorSettings)).Cast<DataVertexRegulator.enumCompensatorSettings>();
            compensatorSettingsB.SelectedIndex = 0;

        }

        private void populateViewIfObjectExists(DataVertexRegulator dvr)
        {
            lineTo.Text = dvr.Line_Segment_To.ToString();
            last_value = lineTo.Text;
            dynamicButton.Content = "Update";
            dynamicButton2.IsEnabled = true;
            MyTextBox.Text = dvr.RegulatorID;
            bandwidthTextBox.Text =  dvr.Bandwidth.ToString();
            ptRatioTextBox.Text = dvr.Ratio.ToString();
            primaryCTRatingTextBox.Text = dvr.PrimaryCTRating.ToString();
            rsettingA.Text = dvr.R_SettingA.ToString();
            rsettingB.Text = dvr.R_SettingB.ToString();
            xsettingA.Text = dvr.X_SettingA.ToString();
            xsettingB.Text = dvr.X_SettingB.ToString();
            voltage_level.Text = dvr.VoltageLevelA.ToString();
            voltage_levelB.Text = dvr.VoltageLevelB.ToString();
            foreach (DataEdge de in globalEdges[dvr.Line_Segment_From])
            {
                if (de.Target.Element_id == dvr.Line_Segment_To)
                {
                    length.Text = de.Length.ToString();
                    break;
                }
            }
           
        }

        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MyTextBox.Text = "";
        }

        private void listboxfrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("back");

            ComboBox cb = sender as ComboBox;
            string loc = cb.SelectedItem.ToString();

            location.Content = "Location: " + loc;
        }




   

        private void bandwidthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (!allFilled())
            {
                MessageBox.Show("You have to enter all data");
                return;
            }

            if ((_mn.graphView.DataContext as GraphViewModel).dvrp != null)
            {
                MessageBox.Show("Second part of regulator exists!");
                return;
            }

            if (globalVertices.ContainsKey(lineTo.Text) && lineTo.Text != last_value)
            {
                MessageBox.Show("Node with that ID already exists, change Line segment to input.");
                return;
            }


            DataVertexRegulator dvr = setupDataVertexRegulatorObject();
            DataVertexRegulatorPartial dvrp;

            if ((_mn.graphView.DataContext as GraphViewModel).dvrp != null)
            {
                dvrp = (_mn.graphView.DataContext as GraphViewModel).dvrp;
                dynamicButton.Content = "Update";
            }
            else
            {
                dvrp = new DataVertexRegulatorPartial()
                {
                    typeOfVertex = DataVertex.TypeOfVertex.REGULATOR_PARTIAL_VERTEX,
                    Text = "Synchronous machine second ending: " + dvr.Line_Segment_To,
                    RegulatorID = dvr.Line_Segment_To,
                    Element_id = dvr.Line_Segment_To,
                    CompensatorSettingsB = dvr.CompensatorSettingsB,
                    R_SettingB = dvr.R_SettingB,
                    X_SettingB = dvr.X_SettingB,
                    VoltageLevelB = dvr.VoltageLevelB,

                };
                (_mn.graphView.DataContext as GraphViewModel).dvrp = dvrp;
            }


            

            if (dynamicButton.Content.ToString() == "Update")
            {

                (_mn.graphView.DataContext as GraphViewModel).UpdateVertices(dvr,dvrp, last_value, length.Text);
                last_value = dvr.Line_Segment_To;
            }
            else
            {
                (_mn.graphView.DataContext as GraphViewModel).CreateComplexVertexWithPartialType(dvr,dvrp,dv,length.Text);

            }
               
            _mn.CloseAllWindows();
            


     
        }

        private DataVertexRegulator setupDataVertexRegulatorObject()
        {
            DataVertexRegulator.enumPhases ep = (DataVertexRegulator.enumPhases)phaseEnum.SelectedItem;
            DataVertexRegulator.enumConnection cn = (DataVertexRegulator.enumConnection)connection.SelectedItem;
            DataVertexRegulator.enumMonitoring em = (DataVertexRegulator.enumMonitoring)monitoringPhase.SelectedItem;
            DataVertexRegulator.enumCompensatorSettings ecs = (DataVertexRegulator.enumCompensatorSettings)compensatorSettingsA.SelectedItem;
            DataVertexRegulator.enumCompensatorSettings ecs2 = (DataVertexRegulator.enumCompensatorSettings)compensatorSettingsB.SelectedItem;

            DataVertexRegulator dvr = new DataVertexRegulator()
            {
                Text = dv.Text,
                Element_id = dv.Element_id,
                typeOfVertex = DataVertex.TypeOfVertex.REGULATOR_VERTEX,
                Location = dv.Text,
                Line_Segment_From = dv.Element_id,
                Line_Segment_To = lineTo.Text,
                connected_nodes = dv.connected_nodes,
                RegulatorID = MyTextBox.Text,
                Bandwidth = Double.Parse(bandwidthTextBox.Text),
                Ratio = Double.Parse(ptRatioTextBox.Text),
                PrimaryCTRating = Double.Parse(primaryCTRatingTextBox.Text),
                Phases = ep,
                Connection = cn,
                MonitoringPhase = em,
                CompensatorSettingsA = ecs,
                CompensatorSettingsB = ecs2,
                R_SettingA = Double.Parse(rsettingA.Text),
                R_SettingB = Double.Parse(rsettingB.Text),
                X_SettingA = Double.Parse(xsettingA.Text),
                X_SettingB = Double.Parse(xsettingB.Text),
                VoltageLevelA = Double.Parse(voltage_level.Text),
                VoltageLevelB = Double.Parse(voltage_levelB.Text),
                //Length = Double.Parse(length.Text)


            };
            return dvr;
        }

        private bool allFilled()
        {
            if (MyTextBox.Text != "" && bandwidthTextBox.Text != "" && ptRatioTextBox.Text != "" && primaryCTRatingTextBox.Text != "" && rsettingA.Text != "" && rsettingB.Text != "" && xsettingA.Text != "" && xsettingB.Text!= "" && voltage_level.Text != "" && voltage_levelB.Text != "" && lineTo != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void dynamicButton2_Click(object sender, RoutedEventArgs e)
        {
            (_mn.graphView.DataContext as GraphViewModel).PrepareComplexVertexForDelete(dv);
            
            //_mn.deleteNode(dv);
        }
    }
}
