/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
namespace InfluxDBChannelProxy.XML.InfluxDB
{
    public class Insert
    {
        public string Measurement;
        public InfluxDBTag Tag { get; set; }
        public InfluxDBField Field { get; set; }
    }
}
