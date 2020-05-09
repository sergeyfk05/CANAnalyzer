/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Windows.Controls;
using System.Windows.Media;

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
                var border = VisualTreeHelper.GetChild(grid, 0) as Decorator;
                if (border != null)
                {
                    //find ScrollViewer UI control
                    var scroll = border.Child as ScrollViewer;

                    //if the scroll was bottom
                    if (Math.Abs(scroll.VerticalOffset - (scroll.ScrollableHeight - e.ExtentHeightChange)) < 0.1)
                    {
                        //scroll to bottom
                        scroll.ScrollToVerticalOffset(scroll.ExtentHeight);
                    }

                }
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
