using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CANAnalyzer.Models.ChannelsProxy.Creators
{
    class DirtRallyProxyCreator : IChannelProxyCreator
    {
        public string SupportedFiles => "*.txt";

        public IChannelProxy CreateInstance(string path)
        {
            return IsCanWorkWith(path) ? new DirtRallyProxy(path) : throw new ArgumentException("this device cannot work with this hardware device.");
        }

        public IChannelProxy CreateInstanceDefault(string path)
        {
            return IsCanWorkWith(path) ? new DirtRallyProxy(path) : null;
        }

        public bool IsCanWorkWith(string path)
        {
            if (!(File.Exists(path) && (path.Split('.').Last() == "txt")))
                return false;


            return true;
        }
    }
}
