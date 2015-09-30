using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudNsLib.Objects;
using Newtonsoft.Json;

namespace CloudNsLib.Client
{
    public partial class CloudNsClient
    {
        public static Uri DefaultUri = new Uri("https://api.cloudns.net/");

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
        
        internal void AddAuthentication(NameValueCollection nvc)
        {
            if (IdType == AuthIdType.MainId)
                nvc["auth-id"] = AuthId.ToString();
            if (IdType == AuthIdType.SubId)
                nvc["sub-auth-id"] = AuthId.ToString();

            nvc["auth-password"] = AuthPassword;
        }

        private Uri BuildUri(string path, NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < nvc.Count; i++)
            {
                string key = nvc.GetKey(i);
                string[] vals = nvc.GetValues(i);

                if (vals == null)
                    continue;

                foreach (string val in vals)
                {
                    if (i > 0)
                        sb.Append("&");

                    sb.Append(key);
                    sb.Append("=");
                    sb.Append(Uri.EscapeUriString(val));
                }
            }

            UriBuilder builder = new UriBuilder(EndPoint);
            builder.Path = path;
            builder.Query = sb.ToString();

            return builder.Uri;
        }

        private async Task<T> ExecuteGet<T>(string url, NameValueCollection nvc)
        {
            AddAuthentication(nvc);

            Uri uri = BuildUri(url, nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            string content = await resp.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception)
            {
                throw new Exception(JsonConvert.DeserializeObject<StatusMessage>(content).StatusDescription);
            }
        }

        private async Task<string> ExecuteGetAsString(string url, NameValueCollection nvc)
        {
            AddAuthentication(nvc);

            Uri uri = BuildUri(url, nvc);
            HttpResponseMessage resp = await _client.GetAsync(uri);

            return await resp.Content.ReadAsStringAsync();
        }
    }
}
