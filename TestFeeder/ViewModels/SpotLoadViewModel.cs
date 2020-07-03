using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestFeeder.Models;

namespace TestFeeder.ViewModels
{
    public class SpotLoadViewModel : ICircuitManipulation, INotifyPropertyChanged
    {

        private string ph1;
        private string ph1_1;
        private string ph2;
        private string ph2_1;
        private string ph3;
        private string ph3_1;
        private string node_id;
        private SpotLoad current_spot_load = new SpotLoad();

        public string Node_id
        {
            get { return node_id; }
            set
            { 
                node_id = value;
                OnPropertyChanged(nameof(node_id));
            }
        }


        public string Ph1
        {
            get { return ph1; }
            set
            {
                ph1 = value;
                OnPropertyChanged(nameof(ph1));
            }
        }

        public string Ph1_1
        {
            get { return ph1_1; }
            set
            {
                ph1_1 = value;
                OnPropertyChanged(nameof(ph1_1));
            }
        }

        public string Ph2
        {
            get { return ph2; }
            set
            {
                ph2 = value;
                OnPropertyChanged(nameof(ph2));
            }
        }


        public string Ph2_1
        {
            get { return ph2_1; }
            set
            {
                ph2_1 = value;
                OnPropertyChanged(nameof(ph2_1));
            }
        }


        public string Ph3
        {
            get { return ph3; }
            set
            {
                ph3 = value;
                OnPropertyChanged(nameof(ph3));
            }
        }

        public string Ph3_1
        {
            get { return ph3_1; }
            set
            {
                ph3_1 = value;
                OnPropertyChanged(nameof(ph3_1));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SpotLoadViewModel()
        {

        }


        public void Create(ICircuitElement element)
        {
            SpotLoad spotLoad = element as SpotLoad;
            MainWindow mainWindow = GetMainWindow();

            if (mainWindow.GlobalSpotLoads.ContainsKey(spotLoad.Element_id) == false)
            {
                mainWindow.GlobalSpotLoads.Add(spotLoad.Element_id, spotLoad);
            }
            else
            {
                mainWindow.GlobalSpotLoads[spotLoad.Element_id] = spotLoad;
            }
            
            mainWindow.CloseAllWindows();

            mainWindow.spotLoadTab._globalSpotLoads = mainWindow.GlobalSpotLoads;
            mainWindow.spotLoadTab.populateListView();
        }

        private MainWindow GetMainWindow()
        {
            return App.Current.MainWindow as MainWindow;
        }

        public bool Delete()
        {
            return false;
        }


        public void Update(ICircuitElement element)
        {

            SpotLoad spotLoad = element as SpotLoad;
            MainWindow mainWindow = GetMainWindow();

            if (mainWindow.GlobalSpotLoads.ContainsKey(spotLoad.Element_id))
            {
                mainWindow.GlobalSpotLoads.Remove(spotLoad.Element_id);
            }

            mainWindow.DeleteSpotLoad(spotLoad.Element_id);
        }


        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
