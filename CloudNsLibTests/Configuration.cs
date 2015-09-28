using CloudNsLib.Client;

namespace CloudNsLibTests
{
    public static class Configuration
    {
        private const int AuthId = 513;
        private const string AuthPass = "password";

        public static CloudNsClient GetClient()
        {
            return new CloudNsClient(AuthId, AuthPass);
        }
    }
}