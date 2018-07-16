namespace Identity.Server.Options {
    public class ConsulOptions {
        public string HttpEndPoint { get; set; }
        public string Datacenter { get; set; }
        public string ServiceName { get; set; }
    }
}