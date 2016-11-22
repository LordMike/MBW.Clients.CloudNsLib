using System;
using System.IO;
using System.Linq;
using CloudNsLib.Client;

namespace CloudNsLibTests
{
    public static class Configuration
    {
        public static CloudNsClient GetClient()
        {
            // Try from ENV
            string input = Environment.GetEnvironmentVariable("CLOUDNS_API");

            // Try from file
            if (string.IsNullOrEmpty(input))
            {
                FileInfo file = new FileInfo(@"..\..\Auth.txt");

                if (file.Exists)
                    input = File.ReadLines(file.FullName).First();
            }

            if (string.IsNullOrEmpty(input))
                throw new Exception("Unable to get client configuration. Setup Auth.txt or Environment variables. See CloudNsLibTests.Configuration");

            string[] splits = input.Split(':');
            int authId = int.Parse(splits[0]);

            return new CloudNsClient(authId, splits[1]);
        }
    }
}