using System.Linq;
using System.Net;
using CloudNsLib.Client;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudNsClient cl = new CloudNsClient(513, "password");

            var ab = cl.ListAvailableNameservers().Result;
            var cc = cl.IsZoneUpdated("spfstore.dk").Result;
            var bb = cl.GetZones().Result;
            var aa = cl.RecordsList("spfstore.dk").Result;
            var dd = cl.GetUpdateStatus("spfstore.dk").Result;

            var aab = cl.RecordsAddCname("spfstore.dk", "test2", 60, "google.dk").Result;
            var aaa = cl.RecordsAddA("spfstore.dk", "test", 60, IPAddress.IPv6None).Result;

            var aac = Enumerable.Range(0, 20000).AsParallel().Select(i =>
            {
                return cl.RecordsAddTxt("spfstore.dk", "sub-" + i, 3600, "test :)").Result;
            }).ToList();
        }
    }
}
