using System.Collections;
using System.Collections.Generic;
using MBW.Clients.CloudNsLib.Objects;

namespace MBW.Clients.CloudNsLib.Client
{
    public class ZoneIterator : IEnumerable<ListZone>
    {
        private const int PageSize = 10;

        private readonly CloudNsClient _client;
        private readonly string _search;

        internal ZoneIterator(CloudNsClient client, string search)
        {
            _client = client;
            _search = search;
        }

        private List<ListZone> GetPage(int page)
        {
            return _client.GetZoneList(_search, page, PageSize).Result;
        }

        public IEnumerator<ListZone> GetEnumerator()
        {
            for (int page = 1; ; page++)
            {
                List<ListZone> tmp = GetPage(page);

                foreach (ListZone zone in tmp)
                    yield return zone;

                if (tmp.Count < PageSize)
                    break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}