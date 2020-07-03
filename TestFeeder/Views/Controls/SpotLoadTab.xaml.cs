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

namespace TestFeeder.Views.Controls
{
    /// <summary>
    /// Interaction logic for SpotLoadTab.xaml
    /// </summary>
    /// 

    //Data Representation of Dict. GlobalSpotLoads
    public partial class SpotLoadTab : UserControl
    {

        public  Dictionary<string, SpotLoad> _globalSpotLoads;
        private MainWindow _mn;
        public SpotLoadTab()
        {
            _globalSpotLoads = new Dictionary<string, SpotLoad>();
            InitializeComponent();
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        internal void populateListView()
        {
            spotLoadData.ItemsSource = _globalSpotLoads.Values.ToArray();
            Console.WriteLine("s");
        }

        private void UpdateSpotLoad(object sender, RoutedEventArgs e)
        {
            _mn =(MainWindow) Application.Current.MainWindow;

            Button button = sender as Button;

            SpotLoad sl = button.DataContext as SpotLoad;

            _mn.UpdateSpotLoad(sl);
        }

        private void DeleteSpotLoad(object sender, RoutedEventArgs e)
        {
            _mn = (MainWindow)Application.Current.MainWindow;

            Button button = sender as Button;

            SpotLoad sl = button.DataContext as SpotLoad;

            _mn.DeleteSpotLoad(sl.Element_id);

            (_mn.graphView.DataContext as GraphViewModel).DeleteVertex(sl);

            //_mn.graphView.deleteNode(_mn.GlobalVertices[sl.Node.Element_id],false,false);
        }
    }
}
