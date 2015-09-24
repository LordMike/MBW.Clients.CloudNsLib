using CloudNsLib.Client;

namespace CloudNsLibTests
{
    public static class Configuration
    {
        private const int AuthId = 513;
        private const string AuthPass = "password";
        private const string TestZone = "cloudnslib-automated-tests.com";

        public static CloudNsClient GetClient()
        {
            return new CloudNsClient(AuthId, AuthPass);
        }

        public static string GetTestZoneName()
        {
            return TestZone;
        }
    }
}