using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CreateConfiguration.xaml
    /// </summary>
    public partial class CreateConfigurationView : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private MainWindow _mn;
        private Dictionary<string, CableConfiguration> globalCableConfiguration;
        private CableConfiguration selectedConfiguration;
        private CreateConfigurationViewModel ccViewModel;

        public CreateConfigurationView(MainWindow mainWindow, Dictionary<string, CableConfiguration> _globalCableConfiguration)
        {
            _mn = mainWindow;
            _mn.ccView = this;
            globalCableConfiguration = _globalCableConfiguration;
            InitializeComponent();
            setupViewDeafultValues();
        }

        public CreateConfigurationView(MainWindow mainWindow, Dictionary<string, CableConfiguration> _globalCableConfiguration, CableConfiguration selectedConfiguration) : this(mainWindow, _globalCableConfiguration)
        {
            _mn = mainWindow;
            _mn.ccView = this;
            this.selectedConfiguration = selectedConfiguration;
            globalCableConfiguration = _globalCableConfiguration;
            dynamicButton.Content = "Update";
            InitializeComponent();
            populateData(selectedConfiguration);

        }

        private void populateData(CableConfiguration selectedConfiguration)
        {
            conf_id.Text = selectedConfiguration.Name;
            comboboxPhasing.SelectedIndex = 0;
            sizeDescription.Text = selectedConfiguration.Cable.SizeDescription;
            materialKind.SelectedIndex = 0;
            wireSpacing.Text = selectedConfiguration.Cable.PhaseWireSpacing.ToString();


            real_part1_1.Text = selectedConfiguration.Primar_value.Real_part.ToString();
            imaginary_part1_1.Text = selectedConfiguration.Primar_value.Imaginary_part.ToString();

            real_part1_2.Text = selectedConfiguration.Secondary_value[0].Real_part.ToString();
            imaginary_part1_2.Text = selectedConfiguration.Secondary_value[0].Imaginary_part.ToString();

            real_part1_3.Text = selectedConfiguration.Terciar_value[0].Real_part.ToString();
            imaginary_part1_3.Text = selectedConfiguration.Terciar_value[0].Imaginary_part.ToString();

            real_part2_2.Text = selectedConfiguration.Secondary_value[1].Real_part.ToString();
            imaginary_part2_2.Text = selectedConfiguration.Secondary_value[1].Imaginary_part.ToString();

            real_part2_3.Text = selectedConfiguration.Terciar_value[1].Real_part.ToString();
            imaginary_part2_3.Text = selectedConfiguration.Terciar_value[1].Imaginary_part.ToString();

            real_part3_3.Text = selectedConfiguration.Terciar_value[2].Real_part.ToString();
            imaginary_part3_3.Text = selectedConfiguration.Terciar_value[2].Imaginary_part.ToString();

            suscenptance_per_length.Text = selectedConfiguration.Susceptance.ToString();





        }

        private void setupViewDeafultValues()
        {
            comboboxPhasing.ItemsSource = Enum.GetValues(typeof(CableConfiguration.PhasingEnum)).Cast<CableConfiguration.PhasingEnum>();
            comboboxPhasing.SelectedIndex = 0;
            materialKind.ItemsSource = Enum.GetValues(typeof(Cable.MaterialEnum)).Cast<Cable.MaterialEnum>();
            materialKind.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Button button = (Button)sender;
            string content = button.Content as string;




            if (!allFilled())
            {
                MessageBox.Show("You have to fill all data.");
                return;
            }

            CableConfiguration cableConfiguration = setupCableConfiguration();

            if (selectedConfiguration != null)
            {
                if (globalCableConfiguration.ContainsKey(selectedConfiguration.Name) && content == "Update")
                {
                    globalCableConfiguration.Remove(selectedConfiguration.Name);
                }

            }




            if (globalCableConfiguration.ContainsKey(conf_id.Text) && content == "Save")
            {
                MessageBox.Show("Configuration with that name already exists");
                return;
            }





            (this.DataContext as CreateConfigurationViewModel).AddCableConfiguration(cableConfiguration, globalCableConfiguration);

        }

        private CableConfiguration setupCableConfiguration()
        {
            CableConfiguration.PhasingEnum pe = (CableConfiguration.PhasingEnum)comboboxPhasing.SelectedItem;
            Cable.MaterialEnum me = (Cable.MaterialEnum)materialKind.SelectedItem;
            ComplexValue primarValue = new ComplexValue()
            {
                Real_part = Double.Parse(real_part1_1.Text),
                Imaginary_part = Double.Parse(imaginary_part1_1.Text)

            };

            List<ComplexValue> secondaryValues = new List<ComplexValue>();
            ComplexValue secondaryValue1 = new ComplexValue()
            {
                Real_part = Double.Parse(real_part1_2.Text),
                Imaginary_part = Double.Parse(imaginary_part1_2.Text)

            };

            ComplexValue secondaryValue2 = new ComplexValue()
            {
                Real_part = Double.Parse(real_part2_2.Text),
                Imaginary_part = Double.Parse(imaginary_part2_2.Text)
            };

            secondaryValues.Add(secondaryValue1);
            secondaryValues.Add(secondaryValue2);


            List<ComplexValue> terciarValues = new List<ComplexValue>();
            ComplexValue terciarValue1 = new ComplexValue()
            {
                Real_part = Double.Parse(real_part1_3.Text),
                Imaginary_part = Double.Parse(imaginary_part1_3.Text)

            };

            ComplexValue terciarValue2 = new ComplexValue()
            {
                Real_part = Double.Parse(real_part2_3.Text),
                Imaginary_part = Double.Parse(imaginary_part2_3.Text)
            };


            ComplexValue terciarValue3 = new ComplexValue()
            {
                Real_part = Double.Parse(real_part3_3.Text),
                Imaginary_part = Double.Parse(imaginary_part3_3.Text)
            };

            terciarValues.Add(terciarValue1);
            terciarValues.Add(terciarValue2);
            terciarValues.Add(terciarValue3);

            Cable cable = new Cable()
            {
                MaterialKind = me,
                SizeDescription = sizeDescription.Text,
                PhaseWireSpacing = Double.Parse(wireSpacing.Text)
            };

            CableConfiguration cableConfiguration = new CableConfiguration()
            {
                Name = conf_id.Text,
                Phasing = pe,
                Primar_value = primarValue,
                Secondary_value = secondaryValues,
                Terciar_value = terciarValues,
                Cable = cable,
                Susceptance = Double.Parse(suscenptance_per_length.Text)
            };

            return cableConfiguration;
        }

        private bool allFilled()
        {
            if (conf_id.Text != "" && sizeDescription.Text != "" 
                && wireSpacing.Text != "" && real_part1_1.Text != ""
                && imaginary_part1_1.Text != "" && real_part1_2.Text != 
                "" && imaginary_part1_2.Text != "" && real_part1_3.Text !=
                "" && imaginary_part1_3.Text != "" && real_part2_2.Text != 
                "" && imaginary_part2_2.Text != "" && real_part2_3.Text != "" && imaginary_part2_3.Text != "" 
                && real_part3_3.Text != "" && imaginary_part3_3.Text != "" && suscenptance_per_length.Text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }



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
