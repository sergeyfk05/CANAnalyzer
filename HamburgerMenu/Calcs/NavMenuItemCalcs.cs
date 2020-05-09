/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System.Windows;
using System.Windows.Controls;

namespace HamburgerMenu.Calcs
{
    internal static class NavMenuItemCalcs
    {
        internal static double CalcMinNavMenuCorrectWidth(this StackPanel root)
        {
            double result = 0;
            foreach (var el in root.Children)
            {
                if (el is DockPanel dockPanel)
                {
                    double buf = dockPanel.CalcMinItemCorrectWidth();
                    if (buf > result)
                        result = buf;
                }
                else if (el is StackPanel stackPanel)
                {
                    double buf = stackPanel.CalcMinNavMenuCorrectWidth();
                    if (buf > result)
                        result = buf;
                }
            }

            return result;
        }

        internal static double CalcMinItemCorrectWidth(this DockPanel panel)
        {
            double result = 0;
            foreach (var el in panel.Children)
            {
                if (el is FrameworkElement element)
                {
                    result += element.ActualWidth;
                    result += element.Margin.Left;
                    result += element.Margin.Right;
                }
            }

            return result;
        }
    }
}
