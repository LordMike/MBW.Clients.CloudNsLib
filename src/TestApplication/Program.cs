using CloudNsLib.Client;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudNsClient cl = new CloudNsClient(904, "password");

            var ab = cl.ListAvailableNameservers().Result;
            
        }
    }
}
