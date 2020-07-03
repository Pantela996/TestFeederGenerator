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
using TestFeeder.ViewModels;
using TestFeederGenerator.Models;

namespace TestFeeder.Views
{
    /// <summary>
    /// Interaction logic for NodeID.xaml
    /// </summary>
    public partial class NodeID : Window
    {

        private MainWindow _mn;
        private DataVertex.TypeOfVertex _vt;
        public NodeID(MainWindow mainWindow, TestFeederGenerator.Models.DataVertex.TypeOfVertex vertexType)
        {
            InitializeComponent();
            _mn = mainWindow;
            _vt = vertexType;
        }

        private void node_id_GotFocus(object sender, RoutedEventArgs e)
        {
            node_id.Text = "";
        }


        private void bandwidthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            // Use SelectionStart property to find the caret position.
            // Insert the previewed text into the existing text in the textbox.
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            int val;
            // If parsing is successful, set Handled to false
            e.Handled = !Int32.TryParse(fullText, out val);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (node_id.Text == "")
            {
                MessageBox.Show("Enter value!");
                return;
            }
            if ((_mn.graphView.DataContext as GraphViewModel).CreateDataVertexBase(_vt, node_id.Text))
            {
                 _mn.CloseAllWindows();
            }
           
        }
    }
}
