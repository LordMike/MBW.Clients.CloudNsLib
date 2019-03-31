using MBW.Clients.CloudNsLib.Client;

namespace MBW.Clients.CloudNsLib.TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudNsClient cl = Configuration.GetClient();

            var ab = cl.ListAvailableNameservers().Result;

        }
    }
}
