using CANAnalyzer.Models.ChannelsProxy.InfluxDB.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CANAnalyzer.Models.ChannelsProxy.Creators
{
    public class InfluxdbChannelProxyCreator : IChannelProxyCreator
    {
        public string SupportedFiles => "*.xml";

        public IChannelProxy CreateInstance(string path)
        {
            return IsCanWorkWith(path) ? new InfluxdbChannelProxy(path) : throw new ArgumentException("this device cannot work with this hardware device.");
        }

        public IChannelProxy CreateInstanceDefault(string path)
        {
            return IsCanWorkWith(path) ? new InfluxdbChannelProxy(path) : null;
        }

        public bool IsCanWorkWith(string path)
        {
            if (!(File.Exists(path) && (path.Split('.').Last() == "xml")))
                return false;

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    var _config = (InfuxDBConfig)formatter.Deserialize(fs);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static XmlSerializer formatter = new XmlSerializer(typeof(InfuxDBConfig));

    }
}
