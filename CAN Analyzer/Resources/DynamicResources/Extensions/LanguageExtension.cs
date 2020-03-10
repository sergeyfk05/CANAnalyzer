using DynamicResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CANAnalyzer.Resources.DynamicResources
{
    [ContentProperty(nameof(ArgumentBindings))]
    internal class LanguageExtension : BaseExtension<LanguageCultureInfo>
    {
        public LanguageExtension() { }
        public LanguageExtension(string key) : base(key) { }
    }
}
