using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using CloudNsLib.Objects;
using Newtonsoft.Json;

namespace CloudNsLib.Client
{
    public partial class CloudNsClient
    {
        public async Task<List<DnsRecord>> RecordsList(string domainName)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;

            Uri uri = BuildUri("/dns/records.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                if (content == "[]")
                    return new List<DnsRecord>();

                return JsonConvert.DeserializeObject<Dictionary<int, DnsRecord>>(content).Values.ToList();
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        public async Task<bool> RecordsDelete(string domainName, int recordId)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();

            Uri uri = BuildUri("/dns/delete-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterA(string domainName, int recordId, string host, int ttl, IPAddress address)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = address.ToString();

            if (address.AddressFamily == AddressFamily.InterNetwork)
                nvc["record-type"] = RecordType.A.ToString();
            else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                nvc["record-type"] = RecordType.AAAA.ToString();
            else
                throw new ArgumentException("Address must be IPv4 or IPv6", nameof(address));

            Uri uri = BuildUri("/dns/mod-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterTxt(string domainName, int recordId, string host, int ttl, string text)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = text;
            nvc["record-type"] = RecordType.TXT.ToString();

            Uri uri = BuildUri("/dns/mod-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<int?> RecordsAddA(string domainName, string host, int ttl, IPAddress address)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = address.ToString();

            if (address.AddressFamily == AddressFamily.InterNetwork)
                nvc["record-type"] = RecordType.A.ToString();
            else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                nvc["record-type"] = RecordType.AAAA.ToString();
            else
                throw new ArgumentException("Address must be IPv4 or IPv6", nameof(address));

            Uri uri = BuildUri("/dns/add-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? (int?)status.Data["id"] : null;
        }

        public async Task<int?> RecordsAddMx(string domainName, string host, int ttl, string target, int priority)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = target;
            nvc["priority"] = priority.ToString();
            nvc["record-type"] = RecordType.MX.ToString();

            Uri uri = BuildUri("/dns/add-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? (int?)status.Data["id"] : null;
        }

        public async Task<int?> RecordsAddCname(string domainName, string host, int ttl, string target)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = target;
            nvc["record-type"] = RecordType.CNAME.ToString();

            Uri uri = BuildUri("/dns/add-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? (int?)status.Data["id"] : null;
        }

        public async Task<int?> RecordsAddTxt(string domainName, string host, int ttl, string text)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = text;
            nvc["record-type"] = RecordType.TXT.ToString();

            Uri uri = BuildUri("/dns/add-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? (int?)Convert.ToInt32(status.Data["id"]) : null;
        }

        public async Task<int?> RecordsAddNs(string domainName, string host, int ttl, string nameserver)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = nameserver;
            nvc["record-type"] = RecordType.NS.ToString();

            Uri uri = BuildUri("/dns/add-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? (int?)status.Data["id"] : null;
        }

        public async Task<int?> RecordsAddSrv(string domainName, string host, int ttl, string value, int priority, int weight, int port)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = value;
            nvc["priority"] = priority.ToString();
            nvc["weight"] = weight.ToString();
            nvc["port"] = port.ToString();
            nvc["record-type"] = RecordType.NS.ToString();

            Uri uri = BuildUri("/dns/add-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? (int?)status.Data["id"] : null;
        }

        public async Task<int?> RecordsAddSshFp(string domainName, string host, int ttl, string value, int algorithm, int fptype)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = value;
            nvc["algorithm"] = algorithm.ToString();
            nvc["fptype"] = fptype.ToString();
            nvc["record-type"] = RecordType.NS.ToString();

            Uri uri = BuildUri("/dns/add-record.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? (int?)status.Data["id"] : null;
        }
    }
}
