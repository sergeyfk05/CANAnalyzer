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

namespace CANAnalyzer.Pages
{
    /// <summary>
    /// Interaction logic for RecieveChannelPage.xaml
    /// </summary>
    public partial class RecieveChannelPage : UserControl
    {
        public RecieveChannelPage()
        {
            InitializeComponent();
        }

        private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                grid.SelectedIndex = grid.Items.Count - 1;
                if (grid.SelectedItem != null)
                    grid.ScrollIntoView(grid.SelectedItem);
            }
        }
    }
}
