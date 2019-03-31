namespace MBW.Clients.CloudNsLib.Objects
{
    public class DnsRecord
    {
        public string Host { get; set; }

        public long Id { get; set; }

        public string Record { get; set; }

        public int Ttl { get; set; }

        public RecordType Type { get; set; }

        public override string ToString()
        {
            return Type + " " + Host;
        }
    }
}