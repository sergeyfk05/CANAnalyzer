/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
namespace InfluxDBChannelProxy.XML.Server
{
    public struct ServerInfo
    {
        public string Hostname { get; set; }
        public short Port { get; set; }
        public string Token { get; set; }
        public string Organization { get; set; }
        public string Bucket { get; set; }
    }
}
