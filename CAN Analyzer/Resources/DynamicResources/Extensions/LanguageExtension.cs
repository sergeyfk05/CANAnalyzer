/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
