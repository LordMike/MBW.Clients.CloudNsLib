namespace CloudNsLib.Objects
{
    public class DnsRecord
    {
        public string Host { get; set; }

        public int Id { get; set; }

        public string Record { get; set; }

        public int Ttl { get; set; }

        public string Type { get; set; }

        public override string ToString()
        {
            return Type + " " + Host;
        }
    }
}