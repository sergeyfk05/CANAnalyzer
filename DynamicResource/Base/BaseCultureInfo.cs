using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicResource
{
    public abstract class BaseCultureInfo
    {
        public BaseCultureInfo(string name)
        {
            if (CulturesInfo.FirstOrDefault(x => x.Name == name) != null)
                throw new ArgumentException($"{this.ToString()} instance with {name} already created.");

            Name = name;
            CulturesInfo.Add(this);
        }

        public string Name { get; private set; }


        public static List<BaseCultureInfo> CulturesInfo => _culturesInfo ?? (_culturesInfo = new List<BaseCultureInfo>());
        private static List<BaseCultureInfo> _culturesInfo;
    }
}
