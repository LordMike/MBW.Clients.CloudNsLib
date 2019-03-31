using Newtonsoft.Json;

namespace MBW.Clients.CloudNsLib.Objects
{
    public class AvailableNameserver
    {
        public string Ip4 { get; set; }

        public string Ip6 { get; set; }

        public string Location { get; set; }

        [JsonProperty("location_cc")]
        public string LocationCc { get; set; }

        public string Name { get; set; }

        public NameserverType Type { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}