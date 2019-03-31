using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MBW.Clients.CloudNsLib.Objects;
using MBW.Clients.CloudNsLib.Utilities;
using Newtonsoft.Json;

namespace MBW.Clients.CloudNsLib.Client
{
    public partial class CloudNsClient
    {
        public static readonly Uri DefaultUri = new Uri("https://api.cloudns.net/");

        public Uri EndPoint => _client.BaseAddress;

        public int AuthId { get; private set; }
        public string AuthPassword { get; private set; }
        public AuthIdType IdType { get; private set; }

        private readonly HttpClient _client;

        public CloudNsClient(int authId, string authPassword, AuthIdType authType = AuthIdType.MainId)
            : this(DefaultUri, authId, authPassword, authType)
        {

        }

        public CloudNsClient(Uri uri, int authId, string authPassword, AuthIdType authType = AuthIdType.MainId)
        {
            _client = new HttpClient();
            _client.BaseAddress = uri;

            SetAuth(authId, authPassword, authType);
        }

        public void SetAuth(int authId, string authPassword, AuthIdType authType)
        {
            AuthId = authId;
            AuthPassword = authPassword;
            IdType = authType;
        }

        internal void AddAuthentication(QueryStringParameters nvc)
        {
            if (IdType == AuthIdType.MainId)
                nvc["auth-id"] = AuthId.ToString();
            if (IdType == AuthIdType.SubId)
                nvc["sub-auth-id"] = AuthId.ToString();

            nvc["auth-password"] = AuthPassword;
        }

        private Uri BuildUri(string path, QueryStringParameters nvc)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (KeyValuePair<string, string[]> pair in nvc)
            {
                if (pair.Value == null)
                    continue;

                foreach (string val in pair.Value)
                {
                    if (sb.Length > 0)
                        sb.Append("&");

                    sb.Append(pair.Key);
                    sb.Append("=");
                    sb.Append(Uri.EscapeUriString(val));
                }
            }

            UriBuilder builder = new UriBuilder(EndPoint);
            builder.Path = path;
            builder.Query = sb.ToString();

            return builder.Uri;
        }

        private async Task<T> ExecuteGet<T>(string url, QueryStringParameters nvc)
        {
            AddAuthentication(nvc);

            Uri uri = BuildUri(url, nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri).ConfigureAwait(false);

            string content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        private async Task<string> ExecuteGetAsString(string url, QueryStringParameters nvc)
        {
            AddAuthentication(nvc);

            Uri uri = BuildUri(url, nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri).ConfigureAwait(false);

            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
