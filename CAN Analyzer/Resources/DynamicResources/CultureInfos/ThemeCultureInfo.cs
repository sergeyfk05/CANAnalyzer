using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicResource;

namespace CANAnalyzer.Resources.DynamicResources
{
    internal class ThemeCultureInfo : BaseCultureInfo
    {
        public ThemeCultureInfo(string name) : base(name)
        { }

        public override string ToString()
        {
            return (string)Manager<ThemeCultureInfo>.StaticInstance.GetResource(Name);
        }
    }
}
