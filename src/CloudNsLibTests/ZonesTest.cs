using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CloudNsLib.Client;
using CloudNsLib.Objects;
using Xunit;

namespace CloudNsLibTests
{
    public class ZonesTest :IDisposable
    {
        private const string TestZoneNameSuffix = "-cloudnslib-automated-test.org";
        private static readonly CloudNsClient Client = Configuration.GetClient();

        public void Dispose()
        {
            List<ListZone> list = Client.ListZones(TestZoneNameSuffix).ToList();

            foreach (ListZone zone in list)
            {
                if (!zone.Name.EndsWith(TestZoneNameSuffix, StringComparison.OrdinalIgnoreCase))
                    continue;

                Client.DeleteZone(zone).Wait();
            }
        }

        [Fact]
        public void TestMasterZoneCreateMultipleNs()
        {
            List<string> nameservers = new List<string> { "google.dk", "test.dk" };
            const string name = "multiplens" + TestZoneNameSuffix;

            // Create
            Assert.True(Client.CreateMasterZone(name, nameservers).Result);

            // Ensure we have all the NS
            List<DnsRecord> recs = Client.RecordsList(name).Result;
            List<DnsRecord> ns = recs.Where(s => s.Type == RecordType.NS).ToList();

            Assert.Equal(nameservers.Count, ns.Count);
            Assert.True(nameservers.All(s => recs.Any(x => x.Record == s.ToString())));

            // Delete zone
            Assert.True(Client.DeleteZone(name).Result);
        }

        [Fact]
        public void TestMasterZoneCreate()
        {
            string[] names = Enumerable.Range(1, 20).Select(s => "test-master" + s + TestZoneNameSuffix).ToArray();

            // Create 20 zones
            foreach (string name in names)
                Assert.True(Client.CreateMasterZone(name).Result);

            // Fetch list
            List<ListZone> created = Client.ListZones().ToList();

            Assert.True(created.Count >= names.Length);

            // Verify all are present
            Assert.True(names.All(s => created.Any(x => x.Type == ZoneType.Master && x.Name.Equals(s, StringComparison.OrdinalIgnoreCase))));

            // Delete all
            foreach (string name in names)
                Assert.True(Client.DeleteZone(name).Result);

            // Fetch list
            List<ListZone> deleted = Client.ListZones().ToList();

            // Verify all are gone
            Assert.True(!names.Any(s => deleted.Any(x => x.Name.Equals(s, StringComparison.OrdinalIgnoreCase))));
        }

        [Fact]
        public void TestSlaveZoneCreate()
        {
            IPAddress testServer = IPAddress.Parse("127.0.0.1");
            string[] names = Enumerable.Range(1, 20).Select(s => "testslave" + s + TestZoneNameSuffix).ToArray();

            // Create 20 zones
            foreach (string name in names)
                Assert.True(Client.CreateSlaveZone(name, testServer).Result);

            // Fetch list
            List<ListZone> created = Client.ListZones().ToList();

            Assert.True(created.Count >= names.Length);

            // Verify all are present
            Assert.True(names.All(s => created.Any(x => x.Type == ZoneType.Slave && x.Name.Equals(s, StringComparison.OrdinalIgnoreCase))));

            // Delete all
            foreach (string name in names)
                Assert.True(Client.DeleteZone(name).Result);

            // Fetch list
            List<ListZone> deleted = Client.ListZones().ToList();

            // Verify all are gone
            Assert.True(!names.Any(s => deleted.Any(x => x.Name.Equals(s, StringComparison.OrdinalIgnoreCase))));
        }
    }
}