using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public async Task<List<ListZone>> GetZones(string search = null, int page = 1, int rowsPrPage = 10)
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
    }
}
