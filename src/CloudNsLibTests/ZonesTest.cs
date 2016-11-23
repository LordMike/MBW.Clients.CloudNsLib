using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CloudNsLib.Client;
using CloudNsLib.Objects;
using Xunit;

namespace CloudNsLibTests
{
    public class ZonesTest : IDisposable
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
        public async Task TestMasterZoneCreateMultipleNs()
        {
            List<string> nameservers = new List<string> { "google.dk", "test.dk" };
            const string name = "multiplens" + TestZoneNameSuffix;

            // Create
            Assert.True(await Client.CreateMasterZone(name, nameservers));

            // Ensure we have all the NS
            List<DnsRecord> recs = await Client.RecordsList(name);
            List<DnsRecord> ns = recs.Where(s => s.Type == RecordType.NS).ToList();

            Assert.Equal(nameservers.Count, ns.Count);
            Assert.True(nameservers.All(s => recs.Any(x => x.Record == s.ToString())));

            // Delete zone
            Assert.True(await Client.DeleteZone(name));
        }

        [Fact]
        public async Task TestMasterZoneCreate()
        {
            string[] names = Enumerable.Range(1, 10).Select(s => "test-master" + s + TestZoneNameSuffix).ToArray();

            // Create 10 zones
            foreach (string name in names)
                Assert.True(await Client.CreateMasterZone(name));

            // Fetch list
            List<ListZone> created = Client.ListZones().ToList();

            Assert.True(created.Count >= names.Length);

            // Verify all are present
            Assert.True(names.All(s => created.Any(x => x.Type == ZoneType.Master && x.Name.Equals(s, StringComparison.OrdinalIgnoreCase))));

            // Delete all
            foreach (string name in names)
                Assert.True(await Client.DeleteZone(name));

            // Fetch list
            List<ListZone> deleted = Client.ListZones().ToList();

            // Verify all are gone
            Assert.True(!names.Any(s => deleted.Any(x => x.Name.Equals(s, StringComparison.OrdinalIgnoreCase))));
        }

        [Fact]
        public async Task TestSlaveZoneCreate()
        {
            IPAddress testServer = IPAddress.Parse("127.0.0.1");
            string[] names = Enumerable.Range(1, 10).Select(s => "testslave" + s + TestZoneNameSuffix).ToArray();

            // Create 10 zones
            foreach (string name in names)
                Assert.True(await Client.CreateSlaveZone(name, testServer));

            // Fetch list
            List<ListZone> created = Client.ListZones().ToList();

            Assert.True(created.Count >= names.Length);

            // Verify all are present
            Assert.True(names.All(s => created.Any(x => x.Type == ZoneType.Slave && x.Name.Equals(s, StringComparison.OrdinalIgnoreCase))));

            // Delete all
            foreach (string name in names)
                Assert.True(await Client.DeleteZone(name));

            // Fetch list
            List<ListZone> deleted = Client.ListZones().ToList();

            // Verify all are gone
            Assert.True(!names.Any(s => deleted.Any(x => x.Name.Equals(s, StringComparison.OrdinalIgnoreCase))));
        }
    }
}