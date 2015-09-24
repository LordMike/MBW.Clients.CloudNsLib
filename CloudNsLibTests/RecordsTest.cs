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
    public class RecordsTest
    {
        private static CloudNsClient _client;

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            _client = Configuration.GetClient();

            bool created = _client.CreateMasterZone(Configuration.GetTestZoneName(), new List<IPAddress>()).Result;
            if (!created)
                throw new Exception("Unable to create " + Configuration.GetTestZoneName());
        }

        [ClassCleanup]
        public static void TestClean()
        {
            bool deleted = _client.DeleteZone(Configuration.GetTestZoneName()).Result;
            if (!deleted)
                throw new Exception("Unable to delete " + Configuration.GetTestZoneName());
        }

        [TestMethod]
        public void TestTxtRecord()
        {
            string txtA = "google.dk";
            string txtB = "dr.dk";

            // Create
            long? id = _client.RecordsAddTxt(Configuration.GetTestZoneName(), "test", 300, txtA).Result;

            Assert.IsNotNull(id);
            Assert.IsTrue(TestExistence("test", RecordType.TXT, txtA));

            // Alter
            bool altered = _client.RecordsAlterTxt(Configuration.GetTestZoneName(), id.Value, "test", 300, txtB).Result;

            Assert.IsTrue(altered);
            Assert.IsTrue(TestExistence("test", RecordType.TXT, txtB));

            // Delete
            bool deleted = _client.RecordsDelete(Configuration.GetTestZoneName(), id.Value).Result;

            Assert.IsTrue(deleted);
            Assert.IsFalse(TestExistence("test", RecordType.TXT));
        }

        //private void TestType(Func<long?> create)
        //{
        //    // Create
        //    long? id = create();

        //    //Assert.IsNotNull(id);
        //    //Assert.IsTrue(TestExistence("test", RecordType.MX, nameA));

        //    //// Alter
        //    //bool altered = _client.RecordsAlterMx(Configuration.GetTestZoneName(), id.Value, "test", 300, nameB, prioB).Result;

        //    //Assert.IsTrue(altered);
        //    //Assert.IsTrue(TestExistence("test", RecordType.MX, nameB));

        //    //// Delete
        //    //bool deleted = _client.RecordsDelete(Configuration.GetTestZoneName(), id.Value).Result;

        //    //Assert.IsTrue(deleted);
        //    //Assert.IsFalse(TestExistence("test", RecordType.MX));
        //}

        [TestMethod]
        public void TestMxRecord()
        {
            string nameA = "google.dk";
            string nameB = "dr.dk";

            int prioA = 10;
            int prioB = 20;

            // Create
            long? id = _client.RecordsAddMx(Configuration.GetTestZoneName(), "test", 300, nameA, prioA).Result;

            Assert.IsNotNull(id);
            Assert.IsTrue(TestExistence("test", RecordType.MX, nameA));

            // Alter
            bool altered = _client.RecordsAlterMx(Configuration.GetTestZoneName(), id.Value, "test", 300, nameB, prioB).Result;

            Assert.IsTrue(altered);
            Assert.IsTrue(TestExistence("test", RecordType.MX, nameB));

            // Delete
            bool deleted = _client.RecordsDelete(Configuration.GetTestZoneName(), id.Value).Result;

            Assert.IsTrue(deleted);
            Assert.IsFalse(TestExistence("test", RecordType.MX));
        }

        [TestMethod]
        public void TestCnameRecord()
        {
            string cnameA = "google.dk";
            string cnameB = "dr.dk";

            // Create
            long? id = _client.RecordsAddCname(Configuration.GetTestZoneName(), "test", 300, cnameA).Result;

            Assert.IsNotNull(id);
            Assert.IsTrue(TestExistence("test", RecordType.CNAME, cnameA));

            // Alter
            bool altered = _client.RecordsAlterCname(Configuration.GetTestZoneName(), id.Value, "test", 300, cnameB).Result;

            Assert.IsTrue(altered);
            Assert.IsTrue(TestExistence("test", RecordType.CNAME, cnameB));

            // Delete
            bool deleted = _client.RecordsDelete(Configuration.GetTestZoneName(), id.Value).Result;

            Assert.IsTrue(deleted);
            Assert.IsFalse(TestExistence("test", RecordType.CNAME));
        }

        [TestMethod]
        public void TestARecord()
        {
            IPAddress ipA = IPAddress.Parse("80.90.100.110");
            IPAddress ipB = IPAddress.Parse("10.20.30.40");

            // Create
            long? id = _client.RecordsAddA(Configuration.GetTestZoneName(), "test", 300, ipA).Result;

            Assert.IsNotNull(id);
            Assert.IsTrue(TestExistence("test", RecordType.A, ipA.ToString()));

            // Alter
            bool altered = _client.RecordsAlterA(Configuration.GetTestZoneName(), id.Value, "test", 300, ipB).Result;

            Assert.IsTrue(altered);
            Assert.IsTrue(TestExistence("test", RecordType.A, ipB.ToString()));

            // Delete
            bool deleted = _client.RecordsDelete(Configuration.GetTestZoneName(), id.Value).Result;

            Assert.IsTrue(deleted);
            Assert.IsFalse(TestExistence("test", RecordType.A));
        }

        [TestMethod]
        public void TestAAAARecord()
        {
            IPAddress ipA = IPAddress.Parse("fe80::1");
            IPAddress ipB = IPAddress.Parse("fe80::2");

            // Create
            long? id = _client.RecordsAddA(Configuration.GetTestZoneName(), "test", 300, ipA).Result;

            Assert.IsNotNull(id);
            Assert.IsTrue(TestExistence("test", RecordType.AAAA, ipA.ToString()));

            // Alter
            bool altered = _client.RecordsAlterA(Configuration.GetTestZoneName(), id.Value, "test", 300, ipB).Result;

            Assert.IsTrue(altered);
            Assert.IsTrue(TestExistence("test", RecordType.AAAA, ipB.ToString()));

            // Delete
            bool deleted = _client.RecordsDelete(Configuration.GetTestZoneName(), id.Value).Result;

            Assert.IsTrue(deleted);
            Assert.IsFalse(TestExistence("test", RecordType.AAAA));
        }

        private List<DnsRecord> GetRecords(string host, RecordType type)
        {
            List<DnsRecord> allRecords = _client.RecordsList(Configuration.GetTestZoneName()).Result;

            return allRecords.Where(s => s.Host == host && s.Type == type).ToList();
        }

        private bool TestExistence(string host, RecordType type, string content = null)
        {
            List<DnsRecord> allRecords = _client.RecordsList(Configuration.GetTestZoneName()).Result;

            if (content == null)
                return allRecords.Any(s => s.Host == host && s.Type == type);
            return allRecords.Any(s => s.Host == host && s.Type == type && s.Record == content);
        }
    }
}
