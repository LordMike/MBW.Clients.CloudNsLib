namespace CloudNsLib.Objects
{
    public class ListZone
    {
        public string Name { get; set; }

        public ZoneType Type { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}