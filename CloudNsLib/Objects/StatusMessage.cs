namespace CloudNsLib.Objects
{
    public class StatusMessage
    {
        public string Status { get; set; }

        public string StatusDescription { get; set; }

        public override string ToString()
        {
            return StatusDescription;
        }
    }
}