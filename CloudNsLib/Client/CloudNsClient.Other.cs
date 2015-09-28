using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CloudNsLib.Objects;
using Newtonsoft.Json;

namespace CloudNsLib.Client
{
    public partial class CloudNsClient
    {
        public async Task<List<AvailableNameserver>> ListAvailableNameservers()
        {
            NameValueCollection nvc = CreateUri();

            Uri uri = BuildUri("/dns/available-name-servers.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<List<AvailableNameserver>>(content);
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        internal async Task<List<ListZone>> GetZoneList(string search, int page, int rowsPrPage)
        {
            NameValueCollection nvc = CreateUri();

            nvc["page"] = page.ToString();
            nvc["rows-per-page"] = rowsPrPage.ToString();

            if (!string.IsNullOrWhiteSpace(search))
                nvc["search"] = search;

            Uri uri = BuildUri("/dns/list-zones.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<List<ListZone>>(content);
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        public ZoneIterator ListZones(string search = null)
        {
            return new ZoneIterator(this, search);
        }

        public async Task<bool> IsZoneUpdated(string domainName)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;

            Uri uri = BuildUri("/dns/is-updated.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<bool>(content);
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        public async Task<List<ZoneUpdateStatus>> GetUpdateStatus(string domainName)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = domainName;

            Uri uri = BuildUri("/dns/update-status.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<List<ZoneUpdateStatus>>(content);
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        public async Task<bool> CreateMasterZone(string zoneName, List<IPAddress> masterServers = null)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = zoneName;
            nvc["zone-type"] = "master";

            if (masterServers != null)
            {
                if (masterServers.Any())
                {
                    foreach (IPAddress ipAddress in masterServers)
                        nvc["ns[]"] = ipAddress.ToString();
                }
                else
                    nvc["ns[]"] = string.Empty;
            }

            Uri uri = BuildUri("/dns/register.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

                return status.Status == "Success";
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        public async Task<bool> CreateSlaveZone(string zoneName, IPAddress masterServer)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = zoneName;
            nvc["zone-type"] = "slave";
            nvc["master-ip"] = masterServer.ToString();

            Uri uri = BuildUri("/dns/register.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

                return status.Status == "Success";
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        public async Task<bool> DeleteZone(ListZone zone)
        {
            return await DeleteZone(zone.Name);
        }

        public async Task<bool> DeleteZone(string zoneName)
        {
            NameValueCollection nvc = CreateUri();

            nvc["domain-name"] = zoneName;

            Uri uri = BuildUri("/dns/delete.json", nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                StatusMessage status = JsonConvert.DeserializeObject<StatusMessage>(content);

                return status.Status == "Success";
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }
    }
}
