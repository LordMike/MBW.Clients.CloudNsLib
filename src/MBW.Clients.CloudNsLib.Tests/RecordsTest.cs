using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MBW.Clients.CloudNsLib.Client;
using MBW.Clients.CloudNsLib.Objects;
using Xunit;

namespace MBW.Clients.CloudNsLib.Tests
{
    public class RecordsTest : IDisposable
    {
        private const string TestZoneName = "cloudnslib-automated-tests.com";
        private const string TestRecordName = "test";
        private static CloudNsClient _client = Configuration.GetClient();

        private static async Task InitializeZone()
        {
            _client = Configuration.GetClient();

            bool created = await _client.CreateMasterZone(TestZoneName);
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
        public async Task TestTxtRecord()
        {
            await InitializeZone();

            string txtA = "google.dk";
            string txtB = "dr.dk";

            // Create
            long? id = await _client.RecordsAddTxt(TestZoneName, TestRecordName, 300, txtA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(TestRecordName, RecordType.TXT, txtA));

            // Alter
            bool altered = await _client.RecordsAlterTxt(TestZoneName, id.Value, TestRecordName, 300, txtB);

            Assert.True(altered);
            Assert.True(await TestExistence(TestRecordName, RecordType.TXT, txtB));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(TestRecordName, RecordType.TXT));
        }

        [Fact]
        public async Task TestSpfRecord()
        {
            await InitializeZone();

            string txtA = "v=spf1 -all";
            string txtB = "v=spf1 include:google.dk -all";

            // Create
            long? id = await _client.RecordsAddSpf(TestZoneName, TestRecordName, 300, txtA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(TestRecordName, RecordType.SPF, txtA));

            // Alter
            bool altered = await _client.RecordsAlterSpf(TestZoneName, id.Value, TestRecordName, 300, txtB);

            Assert.True(altered);
            Assert.True(await TestExistence(TestRecordName, RecordType.SPF, txtB));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(TestRecordName, RecordType.SPF));
        }

        [Fact]
        public async Task TestNsRecord()
        {
            await InitializeZone();

            string nameA = "ns1.testdomain.dk";
            string nameB = "ns2.testdomain.dk";

            // Create
            long? id = await _client.RecordsAddNs(TestZoneName, TestRecordName, 300, nameA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(TestRecordName, RecordType.NS, nameA));

            // Alter
            bool altered = await _client.RecordsAlterNs(TestZoneName, id.Value, TestRecordName, 300, nameB);

            Assert.True(altered);
            Assert.True(await TestExistence(TestRecordName, RecordType.NS, nameB));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(TestRecordName, RecordType.NS));
        }

        [Fact]
        public async Task TestSshFpRecord()
        {
            await InitializeZone();

            SshfpFingerprintType fptypeA = SshfpFingerprintType.SHA1;
            SshfpAlgorithm algorithmA = SshfpAlgorithm.RSA;
            string valueA = "7E7A55CEA3B8E15528665A6781CA7C35190CF0EB";

            SshfpFingerprintType fptypeB = SshfpFingerprintType.SHA1;
            SshfpAlgorithm algorithmB = SshfpAlgorithm.DSA;
            string valueB = "CC17F14DA60CF38E809FE58B10D0F22680D59D08";

            // Create
            long? id = await _client.RecordsAddSshfp(TestZoneName, TestRecordName, 300, valueA, algorithmA, fptypeA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(TestRecordName, RecordType.SSHFP, valueA));

            // Alter
            bool altered = await _client.RecordsAlterSshfp(TestZoneName, id.Value, TestRecordName, 300, valueB, algorithmB, fptypeB);

            Assert.True(altered);
            Assert.True(await TestExistence(TestRecordName, RecordType.SSHFP, valueB));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(TestRecordName, RecordType.SSHFP));
        }

        [Fact]
        public async Task TestSrvRecord()
        {
            await InitializeZone();

            string host = "_jabber._tcp";

            string nameA = "jabber.google.com";
            string nameB = "jabber.google.dk";

            int prioA = 10, weightA = 10, portA = 10;
            int prioB = 20, weightB = 20, portB = 20;

            // Create
            long? id = await _client.RecordsAddSrv(TestZoneName, host, 300, nameA, prioA, weightA, portA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(host, RecordType.SRV, nameA));

            // Alter
            bool altered = await _client.RecordsAlterSrv(TestZoneName, id.Value, host, 300, nameB, prioB, weightB, portB);

            Assert.True(altered);
            Assert.True(await TestExistence(host, RecordType.SRV, nameB));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(host, RecordType.SRV));
        }

        [Fact]
        public async Task TestMxRecord()
        {
            await InitializeZone();

            string nameA = "google.dk";
            string nameB = "dr.dk";

            int prioA = 10;
            int prioB = 20;

            // Create
            long? id = await _client.RecordsAddMx(TestZoneName, TestRecordName, 300, nameA, prioA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(TestRecordName, RecordType.MX, nameA));

            // Alter
            bool altered = await _client.RecordsAlterMx(TestZoneName, id.Value, TestRecordName, 300, nameB, prioB);

            Assert.True(altered);
            Assert.True(await TestExistence(TestRecordName, RecordType.MX, nameB));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(TestRecordName, RecordType.MX));
        }

        [Fact]
        public async Task TestCnameRecord()
        {
            await InitializeZone();

            string cnameA = "google.dk";
            string cnameB = "dr.dk";

            // Create
            long? id = await _client.RecordsAddCname(TestZoneName, TestRecordName, 300, cnameA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(TestRecordName, RecordType.CNAME, cnameA));

            // Alter
            bool altered = await _client.RecordsAlterCname(TestZoneName, id.Value, TestRecordName, 300, cnameB);

            Assert.True(altered);
            Assert.True(await TestExistence(TestRecordName, RecordType.CNAME, cnameB));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(TestRecordName, RecordType.CNAME));
        }

        [Fact]
        public async Task TestARecord()
        {
            await InitializeZone();

            IPAddress ipA = IPAddress.Parse("80.90.100.110");
            IPAddress ipB = IPAddress.Parse("10.20.30.40");

            // Create
            long? id = await _client.RecordsAddA(TestZoneName, TestRecordName, 300, ipA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(TestRecordName, RecordType.A, ipA.ToString()));

            // Alter
            bool altered = await _client.RecordsAlterA(TestZoneName, id.Value, TestRecordName, 300, ipB);

            Assert.True(altered);
            Assert.True(await TestExistence(TestRecordName, RecordType.A, ipB.ToString()));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(TestRecordName, RecordType.A));
        }

        [Fact]
        public async Task TestAAAARecord()
        {
            await InitializeZone();

            IPAddress ipA = IPAddress.Parse("fe80::1");
            IPAddress ipB = IPAddress.Parse("fe80::2");

            // Create
            long? id = await _client.RecordsAddA(TestZoneName, TestRecordName, 300, ipA);

            Assert.NotNull(id);
            Assert.True(await TestExistence(TestRecordName, RecordType.AAAA, ipA.ToString()));

            // Alter
            bool altered = await _client.RecordsAlterA(TestZoneName, id.Value, TestRecordName, 300, ipB);

            Assert.True(altered);
            Assert.True(await TestExistence(TestRecordName, RecordType.AAAA, ipB.ToString()));

            // Delete
            bool deleted = await _client.RecordsDelete(TestZoneName, id.Value);

            Assert.True(deleted);
            Assert.False(await TestExistence(TestRecordName, RecordType.AAAA));
        }

        private async Task<bool> TestExistence(string host, RecordType type, string content = null)
        {
            List<DnsRecord> allRecords = await _client.RecordsList(TestZoneName);

            if (content == null)
                return allRecords.Any(s => s.Host == host && s.Type == type);
            return allRecords.Any(s => s.Host == host && s.Type == type && s.Record == content);
        }
    }
}
