using System;
using System.IO;
using System.Linq;
using CloudNsLib.Client;

namespace TestApplication
{
    public static class Configuration
    {
        public static CloudNsClient GetClient()
        {
            string inputAuthId = Environment.GetEnvironmentVariable("CLOUDNS_AUTHID");
            string inputAuthPass = Environment.GetEnvironmentVariable("CLOUDNS_AUTHPASS");

            if (!string.IsNullOrEmpty(inputAuthId) && !string.IsNullOrEmpty(inputAuthPass))
            {
                int authId = int.Parse(inputAuthId);

                return new CloudNsClient(authId, inputAuthPass);
            }

            FileInfo file = new FileInfo(@"..\..\Auth.txt");

            if (file.Exists)
            {
                string line = File.ReadLines(file.FullName).First();
                string[] splits = line.Split(':');

                inputAuthId = splits[0];
                inputAuthPass = splits[1];

                int authId = int.Parse(inputAuthId);

                return new CloudNsClient(authId, inputAuthPass);
            }

            throw new Exception("Unable to get client configuration. Setup Auth.txt or Environment variables. See TestApplication.Configuration");
        }
    }
}