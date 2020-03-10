using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using DynamicResource;

namespace CANAnalyzer.Resources.DynamicResources
{
    [ContentProperty(nameof(ArgumentBindings))]
    internal class ThemeExtension : BaseExtension<ThemeCultureInfo>
    {
        public ThemeExtension() { }
        public ThemeExtension(string key) : base(key) { }
    }
}
