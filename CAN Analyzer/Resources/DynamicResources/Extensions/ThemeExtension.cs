/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using DynamicResource;
using System.Windows.Markup;

namespace CANAnalyzer.Resources.DynamicResources
{
    [ContentProperty(nameof(ArgumentBindings))]
    internal class ThemeExtension : BaseExtension<ThemeCultureInfo>
    {
        public ThemeExtension() { }
        public ThemeExtension(string key) : base(key) { }
    }
}
