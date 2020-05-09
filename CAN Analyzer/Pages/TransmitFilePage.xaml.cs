/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System.Windows.Controls;

namespace CANAnalyzer.Pages
{
    /// <summary>
    /// Interaction logic for TransmitPage.xaml
    /// </summary>
    public partial class TransmitFilePage : UserControl
    {
        public TransmitFilePage()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
