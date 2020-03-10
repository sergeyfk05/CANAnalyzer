using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using DynamicResource;

namespace CAN_Analyzer.Resources.DynamicResources
{
    [ContentProperty(nameof(ArgumentBindings))]
    internal class ThemesExtension : BaseExtension<ThemeCultureInfo>
    {
        public ThemesExtension() { }
        public ThemesExtension(string key) : base(key) { }
    }
}
