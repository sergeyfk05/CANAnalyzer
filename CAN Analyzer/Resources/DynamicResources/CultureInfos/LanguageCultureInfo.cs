using DynamicResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicResource;

namespace CANAnalyzer.Resources.DynamicResources
{
    public class LanguageCultureInfo : BaseCultureInfo
    {
        public LanguageCultureInfo(string name) : base(name)
        { }

        public override string ToString()
        {
            return (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource(Name);
        }
    }
}
