using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using CloudNsLib.Objects;

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

        private NameValueCollection CreateUri()
        {
            NameValueCollection nvc = new NameValueCollection();

            if (IdType == AuthIdType.MainId)
                nvc["auth-id"] = AuthId.ToString();
            if (IdType == AuthIdType.SubId)
                nvc["sub-auth-id"] = AuthId.ToString();

            nvc["auth-password"] = AuthPassword;

            return nvc;
        }

        private Uri BuildUri(string path, NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < nvc.Count; i++)
            {
                if (i > 0)
                    sb.Append("&");

                sb.Append(nvc.Keys[i] + "=" + Uri.EscapeUriString(nvc[i]));
            }

            UriBuilder builder = new UriBuilder(EndPoint);
            builder.Path = path;
            builder.Query = sb.ToString();

            return builder.Uri;
        }
    }
}
