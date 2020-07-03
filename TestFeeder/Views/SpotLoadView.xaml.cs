using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for CreateSpotLoad.xaml
    /// </summary>
    /// 



    // Class that only execute glue code, creating objects and passing them to GraphViewModel, that holds all logic
    // have 2 use scenarions: Create and Update
    public partial class SpotLoadView : Window
    {

        private MainWindow _mn;
        private Dictionary<string, DataVertex> _gv;
        private SpotLoad current_spot_load;
        Dictionary<string, SpotLoad> _gsl;
        private SpotLoad sl;

        public SpotLoadView(SpotLoad sl, bool v)
        {
            InitializeComponent();
            _mn = App.Current.MainWindow as MainWindow;
            _gv = _mn.GlobalVertices;
            current_spot_load = new SpotLoad();
            loadModelEnum.ItemsSource = Enum.GetValues(typeof(SpotLoad.LoadModelEnum)).Cast<SpotLoad.LoadModelEnum>();
            loadModelEnum.SelectedIndex = 0;
            _gsl = _mn.GlobalSpotLoads;

            //Initial population of data
            populateViewData(sl, v);
        }

        public SpotLoadView(SpotLoad sl)
        {
            InitializeComponent();
            this.sl = sl;
            _mn = App.Current.MainWindow as MainWindow;
            _gv = _mn.GlobalVertices;
            current_spot_load = new SpotLoad();

            //Initial population of data
            populateViewData(sl,true);
            loadModelEnum.ItemsSource = Enum.GetValues(typeof(SpotLoad.LoadModelEnum)).Cast<SpotLoad.LoadModelEnum>();
            loadModelEnum.SelectedIndex = 0;
            this.dynamicButton.Content = "Update";
        }

        private void populateViewData(SpotLoad sl, bool ischecked)
        {
            node_id.Text = "Spot Load number: " + sl.Element_id;
            loadModelEnum.SelectedIndex = 0;
            if (ischecked)
            {
                ph1.Text = sl.Ph_1.ToString();
                ph1_1.Text = sl.Ph_1_2.ToString();
                ph2.Text = sl.Ph_2.ToString();
                ph2_1.Text = sl.Ph_2_2.ToString();
                ph3.Text = sl.Ph_3.ToString();
                ph3_1.Text = sl.Ph_3_2.ToString();
            }




        }

        private void dynamicButton_Click(object sender, RoutedEventArgs e)
        {


            if (!AllFilled())
            {
                MessageBox.Show("Fill all data");
                return;
            }

            if (dynamicButton.Content.ToString() == "Update")
            {
                (this.DataContext as SpotLoadViewModel).Update(current_spot_load);

            }

            createLoadSpotObject();


            (this.DataContext as SpotLoadViewModel).Create(current_spot_load);

        }

        private void createLoadSpotObject()
        {

            SpotLoad.LoadModelEnum sme = (SpotLoad.LoadModelEnum)loadModelEnum.SelectedItem;

            current_spot_load.Text = node_id.Text;
            current_spot_load.Element_id = node_id.Text.Split(' ').Last();
            current_spot_load.Node = _gv[node_id.Text.Split(' ').Last()];
            current_spot_load.LoadModel = sme;
            current_spot_load.Ph_1 = Double.Parse(ph1.Text);
            current_spot_load.Ph_1_2 = Double.Parse(ph1_1.Text);
            current_spot_load.Ph_2 = Double.Parse(ph2.Text);
            current_spot_load.Ph_2_2 = Double.Parse(ph2_1.Text);
            current_spot_load.Ph_3 = Double.Parse(ph3.Text);
            current_spot_load.Ph_3_2 = Double.Parse(ph3_1.Text);
        }

        private bool AllFilled()
        {
            if (node_id.Text != "" && ph1.Text != "" && ph1_1.Text != "" && ph2.Text != "" && ph2_1.Text != "" && ph3.Text != "" && ph3_1.Text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        //Check if entered value is number!
        private void bandwidthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            // Use SelectionStart property to find the caret position.
            // Insert the previewed text into the existing text in the textbox.
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            double val;
            // If parsing is successful, set Handled to false
            e.Handled = !double.TryParse(fullText, out val);
        }
    }
}
