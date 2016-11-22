using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CloudNsLib.Objects;
using CloudNsLib.Utilities;
using Newtonsoft.Json;

namespace CloudNsLib.Client
{
    public partial class CloudNsClient
    {
        public async Task<List<AvailableNameserver>> ListAvailableNameservers()
        {
            return await ExecuteGet<List<AvailableNameserver>>("/dns/available-name-servers.json", new NameValueCollection());
        }

        internal async Task<List<ListZone>> GetZoneList(string search, int page, int rowsPrPage)
        {
            NameValueCollection nvc = new NameValueCollection();
            
            nvc["page"] = page.ToString();
            nvc["rows-per-page"] = rowsPrPage.ToString();

            if (!string.IsNullOrWhiteSpace(search))
                nvc["search"] = search;

            return await ExecuteGet<List<ListZone>>("/dns/list-zones.json", nvc);
        }

        public ZoneIterator ListZones(string search = null)
        {
            return new ZoneIterator(this, search);
        }

        public async Task<bool> IsZoneUpdated(string domainName)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;

            return await ExecuteGet<bool>("/dns/is-updated.json", nvc);
        }

        public async Task<List<ZoneUpdateStatus>> GetUpdateStatus(string domainName)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = domainName;

            return await ExecuteGet<List<ZoneUpdateStatus>>("/dns/update-status.json", nvc);
        }

        public async Task<bool> CreateMasterZone(string zoneName, List<string> masterServers = null)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = zoneName;
            nvc["zone-type"] = "master";

            if (masterServers != null)
            {
                if (masterServers.Any())
                {
                    foreach (string ipAddress in masterServers)
                        nvc.Add("ns[]", ipAddress);
                }
                else
                    nvc["ns[]"] = string.Empty;
            }

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/register.json", nvc);

            return status.Status == "Success";
        }

        public async Task<bool> CreateSlaveZone(string zoneName, IPAddress masterServer)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = zoneName;
            nvc["zone-type"] = "slave";
            nvc["master-ip"] = masterServer.ToString();

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/register.json", nvc);

            return status.Status == "Success";
        }

        public async Task<bool> DeleteZone(ListZone zone)
        {
            return await DeleteZone(zone.Name);
        }

        public async Task<bool> DeleteZone(string zoneName)
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc["domain-name"] = zoneName;

            StatusMessage status = await ExecuteGet<StatusMessage>("/dns/delete.json", nvc);

            return status.Status == "Success";
        }
    }
}
