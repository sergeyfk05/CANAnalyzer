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
