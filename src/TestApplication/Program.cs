using CloudNsLib.Client;

namespace TestApplication
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
