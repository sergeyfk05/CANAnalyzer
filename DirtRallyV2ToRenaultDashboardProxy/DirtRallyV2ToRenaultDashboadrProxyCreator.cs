using CANAnalyzerChannelProxyInterfaces;
using System;
using System.IO;
using System.Linq;

namespace DirtRallyV2ToRenaultDashboardProxy
{
    public class DirtRallyV2ToRenaultDashboadrProxyCreator : IChannelProxyCreator
    {
        public string SupportedFiles => "*.txt";

        public IChannelProxy CreateInstance(string path)
        {
            return IsCanWorkWith(path) ? new DirtRallyV2ToRenaultDashboardProxy(path) : throw new ArgumentException("this device cannot work with this hardware device.");
        }

        public IChannelProxy CreateInstanceDefault(string path)
        {
            return IsCanWorkWith(path) ? new DirtRallyV2ToRenaultDashboardProxy(path) : null;
        }

        public bool IsCanWorkWith(string path)
        {
            if (!(File.Exists(path) && (path.Split('.').Last() == "txt")))
                return false;


            return true;
        }
    }
}
