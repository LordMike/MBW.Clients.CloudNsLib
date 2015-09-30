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
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;

            string content = await ExecuteGetAsString("/dns/records.json", nvc);

            if (content == "[]")
                return new List<DnsRecord>();

            return JsonConvert.DeserializeObject<Dictionary<int, DnsRecord>>(content).Values.ToList();
        }

        public async Task<bool> RecordsDelete(string domainName, long recordId)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/delete-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterA(string domainName, long recordId, string host, int ttl, IPAddress address)
        {
            NameValueCollection nvc = new NameValueCollection();

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
            
            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/mod-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterTxt(string domainName, long recordId, string host, int ttl, string text)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = text;
            nvc["record-type"] = RecordType.TXT.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/mod-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterSpf(string domainName, long recordId, string host, int ttl, string spfRecord)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = spfRecord;
            nvc["record-type"] = RecordType.TXT.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/mod-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterCname(string domainName, long recordId, string host, int ttl, string target)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = target;
            nvc["record-type"] = RecordType.CNAME.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/mod-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterMx(string domainName, long recordId, string host, int ttl, string target, int priority)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = target;
            nvc["priority"] = priority.ToString();
            nvc["record-type"] = RecordType.MX.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/mod-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterNs(string domainName, long recordId, string host, int ttl, string nameserver)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = nameserver;
            nvc["record-type"] = RecordType.NS.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/mod-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterSrv(string domainName, long recordId, string host, int ttl, string value, int priority, int weight, int port)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = value;
            nvc["priority"] = priority.ToString();
            nvc["weight"] = weight.ToString();
            nvc["port"] = port.ToString();
            nvc["record-type"] = RecordType.SRV.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/mod-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<bool> RecordsAlterSshfp(string domainName, long recordId, string host, int ttl, string value, SshfpAlgorithm algorithm, SshfpFingerprintType fptype)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["record-id"] = recordId.ToString();
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = value;
            nvc["algorithm"] = ((int)algorithm).ToString();
            nvc["fptype"] = ((int)fptype).ToString();
            nvc["record-type"] = RecordType.SSHFP.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/mod-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success";
        }

        public async Task<long?> RecordsAddA(string domainName, string host, int ttl, IPAddress address)
        {
            NameValueCollection nvc = new NameValueCollection();

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
            
            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/add-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);
            
            return status.Status == "Success" ? status.Data["id"] as long? : null;
        }

        public async Task<long?> RecordsAddMx(string domainName, string host, int ttl, string target, int priority)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = target;
            nvc["priority"] = priority.ToString();
            nvc["record-type"] = RecordType.MX.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/add-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? status.Data["id"] as long? : null;
        }

        public async Task<long?> RecordsAddCname(string domainName, string host, int ttl, string target)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = target;
            nvc["record-type"] = RecordType.CNAME.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/add-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? status.Data["id"] as long? : null;
        }

        public async Task<long?> RecordsAddTxt(string domainName, string host, int ttl, string text)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = text;
            nvc["record-type"] = RecordType.TXT.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/add-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? status.Data["id"] as long? : null;
        }

        public async Task<long?> RecordsAddSpf(string domainName, string host, int ttl, string spfRecord)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = spfRecord;
            nvc["record-type"] = RecordType.SPF.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/add-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? status.Data["id"] as long? : null;
        }

        public async Task<long?> RecordsAddNs(string domainName, string host, int ttl, string nameserver)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = nameserver;
            nvc["record-type"] = RecordType.NS.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/add-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? status.Data["id"] as long? : null;
        }

        public async Task<long?> RecordsAddSrv(string domainName, string host, int ttl, string value, int priority, int weight, int port)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = value;
            nvc["priority"] = priority.ToString();
            nvc["weight"] = weight.ToString();
            nvc["port"] = port.ToString();
            nvc["record-type"] = RecordType.SRV.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/add-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? status.Data["id"] as long? : null;
        }

        public async Task<long?> RecordsAddSshfp(string domainName, string host, int ttl, string value, SshfpAlgorithm algorithm, SshfpFingerprintType fptype)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;
            nvc["host"] = host;
            nvc["ttl"] = ttl.ToString();
            nvc["record"] = value;
            nvc["algorithm"] = ((int)algorithm).ToString();
            nvc["fptype"] = ((int)fptype).ToString();
            nvc["record-type"] = RecordType.SSHFP.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/add-record.json", nvc);

            if (status.Status == "Failed")
                throw new Exception(status.StatusDescription);

            return status.Status == "Success" ? status.Data["id"] as long? : null;
        }
    }
}
