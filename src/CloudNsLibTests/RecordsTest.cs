using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CloudNsLib.Client;
using CloudNsLib.Objects;
using Xunit;

namespace CloudNsLibTests
{
    public class RecordsTest :IDisposable
    {
        private const string TestZoneName = "cloudnslib-automated-tests.com";
        private const string TestRecordName = "test";
        private static CloudNsClient _client = Configuration.GetClient();

        private static void InitializeZone()
        {
            _client = Configuration.GetClient();

            bool created = _client.CreateMasterZone(TestZoneName).Result;
            if (!created)
                throw new Exception("Unable to create " + TestZoneName);
        }
        
        public void Dispose()
        {
            List<DnsRecord> recs = _client.RecordsList(TestZoneName).Result;

            foreach (DnsRecord rec in recs)
                _client.RecordsDelete(TestZoneName, rec.Id).Wait();

            bool deleted = _client.DeleteZone(TestZoneName).Result;
            if (!deleted)
                throw new Exception("Unable to delete " + TestZoneName);
        }
        
        [Fact]
        public void TestTxtRecord()
        {
            InitializeZone();

            string txtA = "google.dk";
            string txtB = "dr.dk";

            // Create
            long? id = _client.RecordsAddTxt(TestZoneName, TestRecordName, 300, txtA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(TestRecordName, RecordType.TXT, txtA));

            // Alter
            bool altered = _client.RecordsAlterTxt(TestZoneName, id.Value, TestRecordName, 300, txtB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(TestRecordName, RecordType.TXT, txtB));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(TestRecordName, RecordType.TXT));
        }

        [Fact]
        public void TestSpfRecord()
        {
            InitializeZone();

            string txtA = "v=spf1 -all";
            string txtB = "v=spf1 include:google.dk -all";

            // Create
            long? id = _client.RecordsAddSpf(TestZoneName, TestRecordName, 300, txtA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(TestRecordName, RecordType.SPF, txtA));

            // Alter
            bool altered = _client.RecordsAlterSpf(TestZoneName, id.Value, TestRecordName, 300, txtB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(TestRecordName, RecordType.SPF, txtB));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(TestRecordName, RecordType.SPF));
        }

        [Fact]
        public void TestNsRecord()
        {
            InitializeZone();

            string nameA = "ns1.testdomain.dk";
            string nameB = "ns2.testdomain.dk";

            // Create
            long? id = _client.RecordsAddNs(TestZoneName, TestRecordName, 300, nameA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(TestRecordName, RecordType.NS, nameA));

            // Alter
            bool altered = _client.RecordsAlterNs(TestZoneName, id.Value, TestRecordName, 300, nameB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(TestRecordName, RecordType.NS, nameB));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(TestRecordName, RecordType.NS));
        }

        [Fact]
        public void TestSshFpRecord()
        {
            InitializeZone();

            SshfpFingerprintType fptypeA = SshfpFingerprintType.SHA1;
            SshfpAlgorithm algorithmA = SshfpAlgorithm.RSA;
            string valueA = "7E7A55CEA3B8E15528665A6781CA7C35190CF0EB";

            SshfpFingerprintType fptypeB = SshfpFingerprintType.SHA1;
            SshfpAlgorithm algorithmB = SshfpAlgorithm.DSA;
            string valueB = "CC17F14DA60CF38E809FE58B10D0F22680D59D08";

            // Create
            long? id = _client.RecordsAddSshfp(TestZoneName, TestRecordName, 300, valueA, algorithmA, fptypeA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(TestRecordName, RecordType.SSHFP, valueA));

            // Alter
            bool altered = _client.RecordsAlterSshfp(TestZoneName, id.Value, TestRecordName, 300, valueB, algorithmB, fptypeB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(TestRecordName, RecordType.SSHFP, valueB));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(TestRecordName, RecordType.SSHFP));
        }

        [Fact]
        public void TestSrvRecord()
        {
            InitializeZone();

            string host = "_jabber._tcp";

            string nameA = "jabber.google.com";
            string nameB = "jabber.google.dk";

            int prioA = 10, weightA = 10, portA = 10;
            int prioB = 20, weightB = 20, portB = 20;

            // Create
            long? id = _client.RecordsAddSrv(TestZoneName, host, 300, nameA, prioA, weightA, portA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(host, RecordType.SRV, nameA));

            // Alter
            bool altered = _client.RecordsAlterSrv(TestZoneName, id.Value, host, 300, nameB, prioB, weightB, portB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(host, RecordType.SRV, nameB));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(host, RecordType.SRV));
        }

        [Fact]
        public void TestMxRecord()
        {
            InitializeZone();

            string nameA = "google.dk";
            string nameB = "dr.dk";

            int prioA = 10;
            int prioB = 20;

            // Create
            long? id = _client.RecordsAddMx(TestZoneName, TestRecordName, 300, nameA, prioA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(TestRecordName, RecordType.MX, nameA));

            // Alter
            bool altered = _client.RecordsAlterMx(TestZoneName, id.Value, TestRecordName, 300, nameB, prioB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(TestRecordName, RecordType.MX, nameB));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(TestRecordName, RecordType.MX));
        }

        [Fact]
        public void TestCnameRecord()
        {
            InitializeZone();

            string cnameA = "google.dk";
            string cnameB = "dr.dk";

            // Create
            long? id = _client.RecordsAddCname(TestZoneName, TestRecordName, 300, cnameA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(TestRecordName, RecordType.CNAME, cnameA));

            // Alter
            bool altered = _client.RecordsAlterCname(TestZoneName, id.Value, TestRecordName, 300, cnameB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(TestRecordName, RecordType.CNAME, cnameB));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(TestRecordName, RecordType.CNAME));
        }

        [Fact]
        public void TestARecord()
        {
            InitializeZone();

            IPAddress ipA = IPAddress.Parse("80.90.100.110");
            IPAddress ipB = IPAddress.Parse("10.20.30.40");

            // Create
            long? id = _client.RecordsAddA(TestZoneName, TestRecordName, 300, ipA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(TestRecordName, RecordType.A, ipA.ToString()));

            // Alter
            bool altered = _client.RecordsAlterA(TestZoneName, id.Value, TestRecordName, 300, ipB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(TestRecordName, RecordType.A, ipB.ToString()));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(TestRecordName, RecordType.A));
        }

        [Fact]
        public void TestAAAARecord()
        {
            InitializeZone();

            IPAddress ipA = IPAddress.Parse("fe80::1");
            IPAddress ipB = IPAddress.Parse("fe80::2");

            // Create
            long? id = _client.RecordsAddA(TestZoneName, TestRecordName, 300, ipA).Result;

            Assert.NotNull(id);
            Assert.True(TestExistence(TestRecordName, RecordType.AAAA, ipA.ToString()));

            // Alter
            bool altered = _client.RecordsAlterA(TestZoneName, id.Value, TestRecordName, 300, ipB).Result;

            Assert.True(altered);
            Assert.True(TestExistence(TestRecordName, RecordType.AAAA, ipB.ToString()));

            // Delete
            bool deleted = _client.RecordsDelete(TestZoneName, id.Value).Result;

            Assert.True(deleted);
            Assert.False(TestExistence(TestRecordName, RecordType.AAAA));
        }
        
        private bool TestExistence(string host, RecordType type, string content = null)
        {
            List<DnsRecord> allRecords = _client.RecordsList(TestZoneName).Result;

            if (content == null)
                return allRecords.Any(s => s.Host == host && s.Type == type);
            return allRecords.Any(s => s.Host == host && s.Type == type && s.Record == content);
        }
    }
}
