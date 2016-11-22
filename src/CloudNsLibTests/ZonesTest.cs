using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CloudNsLib.Client;
using CloudNsLib.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudNsLibTests
{
    [TestClass]
    public class ZonesTest
    {
        private const string TestZoneNameSuffix = "-cloudnslib-automated-test.org";
        private static CloudNsClient _client;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _client = Configuration.GetClient();
        }

        [ClassCleanup]
        public static void ClassClean()
        {
            List<ListZone> list = _client.ListZones(TestZoneNameSuffix).ToList();

            foreach (ListZone zone in list)
            {
                if (!zone.Name.EndsWith(TestZoneNameSuffix, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                _client.DeleteZone(zone).Wait();
            }
        }

        [TestMethod]
        public void TestMasterZoneCreateMultipleNs()
        {
            List<string> nameservers = new List<string> { "google.dk", "test.dk" };
            const string name = "multiplens" + TestZoneNameSuffix;

            // Create
            Assert.IsTrue(_client.CreateMasterZone(name, nameservers).Result);

            // Ensure we have all the NS
            List<DnsRecord> recs = _client.RecordsList(name).Result;
            List<DnsRecord> ns = recs.Where(s => s.Type == RecordType.NS).ToList();

            Assert.AreEqual(nameservers.Count, ns.Count);
            Assert.IsTrue(nameservers.All(s => recs.Any(x => x.Record == s.ToString())));

            // Delete zone
            Assert.IsTrue(_client.DeleteZone(name).Result);
        }

        [TestMethod]
        public void TestMasterZoneCreate()
        {
            string[] names = Enumerable.Range(1, 20).Select(s => "test-master" + s + TestZoneNameSuffix).ToArray();

            // Create 20 zones
            foreach (string name in names)
                Assert.IsTrue(_client.CreateMasterZone(name).Result);

            // Fetch list
            List<ListZone> created = _client.ListZones().ToList();

            Assert.IsTrue(created.Count >= names.Length);

            // Verify all are present
            Assert.IsTrue(names.All(s => created.Any(x => x.Type == ZoneType.Master && x.Name.Equals(s, StringComparison.InvariantCultureIgnoreCase))));

            // Delete all
            foreach (string name in names)
                Assert.IsTrue(_client.DeleteZone(name).Result);

            // Fetch list
            List<ListZone> deleted = _client.ListZones().ToList();

            // Verify all are gone
            Assert.IsTrue(!names.Any(s => deleted.Any(x => x.Name.Equals(s, StringComparison.InvariantCultureIgnoreCase))));
        }

        [TestMethod]
        public void TestSlaveZoneCreate()
        {
            IPAddress testServer = IPAddress.Parse("127.0.0.1");
            string[] names = Enumerable.Range(1, 20).Select(s => "testslave" + s + TestZoneNameSuffix).ToArray();

            // Create 20 zones
            foreach (string name in names)
                Assert.IsTrue(_client.CreateSlaveZone(name, testServer).Result);

            // Fetch list
            List<ListZone> created = _client.ListZones().ToList();

            Assert.IsTrue(created.Count >= names.Length);

            // Verify all are present
            Assert.IsTrue(names.All(s => created.Any(x => x.Type == ZoneType.Slave && x.Name.Equals(s, StringComparison.InvariantCultureIgnoreCase))));

            // Delete all
            foreach (string name in names)
                Assert.IsTrue(_client.DeleteZone(name).Result);

            // Fetch list
            List<ListZone> deleted = _client.ListZones().ToList();

            // Verify all are gone
            Assert.IsTrue(!names.Any(s => deleted.Any(x => x.Name.Equals(s, StringComparison.InvariantCultureIgnoreCase))));
        }
    }
}