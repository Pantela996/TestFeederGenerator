using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFeeder.Models;

namespace TestFeeder.ViewModels
{
    public class CreateConfigurationViewModel
    {
        internal void AddCableConfiguration(CableConfiguration cableConfiguration, Dictionary<string, CableConfiguration> _globalCableConfiguration = null)
        {
            if (_globalCableConfiguration != null)
            {
                if (_globalCableConfiguration.Count != 0)
                {
                    (App.Current.MainWindow as MainWindow).GlobalCableConfiguration = _globalCableConfiguration;
                }
            }

            (App.Current.MainWindow as MainWindow).GlobalCableConfiguration.Add(cableConfiguration.Name, cableConfiguration);

           

            (App.Current.MainWindow as MainWindow).CloseAllWindows();
        }


        public void RemoveExistingConfiguration(CableConfiguration selectedConfiguration)
        {
            (App.Current.MainWindow as MainWindow).GlobalCableConfiguration.Remove(selectedConfiguration.Name);
        }


    }
}
