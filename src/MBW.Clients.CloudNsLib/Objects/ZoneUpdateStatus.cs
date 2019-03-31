namespace MBW.Clients.CloudNsLib.Objects
{
    public class ZoneUpdateStatus
    {
        public string Ip4 { get; set; }

        public string Ip6 { get; set; }
        
        public string Server { get; set; }

        public bool Updated { get; set; }

        public override string ToString()
        {
            return Server;
        }
    }
}