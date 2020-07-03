using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace TestFeeder.Views
{
    /// <summary>
    /// Interaction logic for EditConfiguration.xaml
    /// </summary>
    /// 


    //Glue code that interacts with MainWindow that holds global dictionaries of Compounding components
    public partial class EditConfigurationView : Window
    {
        private MainWindow mainWindow;
        private Dictionary<string, CableConfiguration> _configurationContainer;
        private CableConfiguration selectedConfiguration;


        public EditConfigurationView(Dictionary<string, CableConfiguration> ccs, MainWindow _mn)
        {
            InitializeComponent();
            cableConfigurations.ItemsSource = ccs.Values.ToList();
            mainWindow = _mn;
            _configurationContainer = ccs;
        }

        private void EditConfiguration(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            selectedConfiguration = button.DataContext as CableConfiguration;

            var w = new Window();
            w = new CreateConfigurationView(mainWindow, _configurationContainer,selectedConfiguration);
            w.Show();


        }

        private void DeleteConfiguration(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            selectedConfiguration = button.DataContext as CableConfiguration;


            (mainWindow.ccView.DataContext as CreateConfigurationViewModel).RemoveExistingConfiguration(selectedConfiguration);

            _configurationContainer.Remove(selectedConfiguration.Name);

            cableConfigurations.ItemsSource = mainWindow.GlobalCableConfiguration.Values;

            //mainWindow.CloseAllWindows();

        }
    }
}
