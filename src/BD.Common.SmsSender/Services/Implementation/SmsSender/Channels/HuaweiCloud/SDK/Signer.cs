// HWS API Gateway Signature

namespace APIGATEWAY_SDK
{
    public class HttpRequest
    {
        public string? method;
        public string? host; /*   http://example.com  */
        public string? uri = "/";  /*   /request/uri      */
        public Dictionary<string, List<string>>? query = new Dictionary<string, List<string>>();
        public WebHeaderCollection? headers = new WebHeaderCollection();
        public string? body = "";
        public string? canonicalRequest;
        public string? stringToSign;

        public HttpRequest(string method = "GET", Uri url = null, WebHeaderCollection headers = null, string body = null)
        {
            if (method != null)
            {
                this.method = method;
            }
            if (url != null)
            {
                host = url.Scheme + "://" + url.Host + ":" + url.Port;
                uri = url.GetComponents(UriComponents.Path | UriComponents.KeepDelimiter, UriFormat.Unescaped);
                query = new Dictionary<string, List<string>>();
                if (url.Query.Length > 1)
                {
                    foreach (var kv in url.Query[1..].Split('&'))
                    {
                        string[] spl = kv.Split(new char[] { '=' }, 2);
                        string key = Uri.UnescapeDataString(spl[0]);
                        string value = "";
                        if (spl.Length > 1)
                        {
                            value = Uri.UnescapeDataString(spl[1]);
                        }
                        if (query.ContainsKey(key))
                        {
                            query[key].Add(value);
                        }
                        else
                        {
                            query[key] = new List<string> { value };
                        }
                    }
                }
            }
            if (headers != null)
            {
                this.headers = headers;
            }
            if (body != null)
            {
                this.body = body;
            }
        }
    }

    public partial class Signer
    {
        const string BasicDateFormat = "yyyyMMddTHHmmssZ";
        const string Algorithm = "SDK-HMAC-SHA256";
        const string HeaderXDate = "X-Sdk-Date";
        const string HeaderHost = "host";
        const string HeaderAuthorization = "Authorization";
        const string HeaderContentSha256 = "X-Sdk-Content-Sha256";
        readonly HashSet<string> unsignedHeaders = new HashSet<string> { "content-type" };

        private string? key;
        private string? secret;

        public string AppKey
        {
            get => key;
            set => key = value;
        }

        public string AppSecret
        {
            get => secret;
            set => secret = value;
        }

        public string Key
        {
            get => key;
            set => key = value;
        }

        public string Secret
        {
            get => secret;
            set => secret = value;
        }

        byte[] Hmacsha256(byte[] keyByte, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                return hmacsha256.ComputeHash(messageBytes);
            }
        }

        // Build a CanonicalRequest from a regular request string
        //
        // CanonicalRequest =
        //  HTTPRequestMethod + '\n' +
        //  CanonicalURI + '\n' +
        //  CanonicalQueryString + '\n' +
        //  CanonicalHeaders + '\n' +
        //  SignedHeaders + '\n' +
        //  HexEncode(Hash(RequestPayload))
        string CanonicalRequest(HttpRequest r, List<string> signedHeaders)
        {
            string hexencode;
            if (r.headers.Get(HeaderContentSha256) != null)
            {
                hexencode = r.headers.Get(HeaderContentSha256);
            }
            else
            {
                var data = Encoding.UTF8.GetBytes(r.body!);
                hexencode = HexEncodeSHA256Hash(data);
            }
            return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", r.method, CanonicalURI(r), CanonicalQueryString(r), CanonicalHeaders(r, signedHeaders), string.Join(";", signedHeaders), hexencode);
        }

        string CanonicalURI(HttpRequest r)
        {
            var pattens = r.uri.Split('/');
            List<string> uri = new List<string>();
            foreach (var v in pattens)
            {
                uri.Add(UrlEncode(v));
            }
            var urlpath = string.Join("/", uri);
            if (urlpath[urlpath.Length - 1] != '/')
            {
                urlpath = urlpath + "/"; // always end with /
            }
            //r.uri = urlpath;
            return urlpath;
        }

        string CanonicalQueryString(HttpRequest r)
        {
            List<string> keys = new List<string>();
            foreach (var pair in r.query)
            {
                keys.Add(pair.Key);
            }
            keys.Sort(String.CompareOrdinal);
            List<string> a = new List<string>();
            foreach (var key in keys)
            {
                string k = UrlEncode(key);
                List<string> values = r.query[key];
                values.Sort(String.CompareOrdinal);
                foreach (var value in values)
                {
                    string kv = k + "=" + UrlEncode(value);
                    a.Add(kv);
                }
            }
            return string.Join("&", a);
        }

        string CanonicalHeaders(HttpRequest r, List<string> signedHeaders)
        {
            List<string> a = new List<string>();
            foreach (string key in signedHeaders)
            {
                var values = new List<string>(r.headers.GetValues(key)!);
                values.Sort(String.CompareOrdinal);
                foreach (var value in values)
                {
                    a.Add(key + ":" + value.Trim());
                    r.headers.Set(key, Encoding.GetEncoding("iso-8859-1").GetString(Encoding.UTF8.GetBytes(value)));
                }
            }
            return string.Join("\n", a) + "\n";
        }

        List<string> SignedHeaders(HttpRequest r)
        {
            List<string> a = new List<string>();
            foreach (string key in r.headers.AllKeys)
            {
                string keyLower = key.ToLower();
                if (!unsignedHeaders.Contains(keyLower))
                {
                    a.Add(key.ToLower());
                }
            }
            a.Sort(String.CompareOrdinal);
            return a;
        }

        static char GetHexValue(int i)
        {
            if (i < 10)
            {
                return (char)(i + '0');
            }
            return (char)(i - 10 + 'a');
        }

        public static string ToHexString(byte[] value)
        {
            int num = value.Length * 2;
            char[] array = new char[num];
            int num2 = 0;
            for (int i = 0; i < num; i += 2)
            {
                byte b = value[num2++];
                array[i] = GetHexValue(b / 16);
                array[i + 1] = GetHexValue(b % 16);
            }
            return new string(array, 0, num);
        }

        // Create a "String to Sign".
        string StringToSign(string canonicalRequest, DateTime t)
        {
            SHA256 sha256 = new SHA256Managed();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(canonicalRequest));
            sha256.Clear();
            return string.Format("{0}\n{1}\n{2}", Algorithm, t.ToUniversalTime().ToString(BasicDateFormat), ToHexString(bytes));
        }

        // Create the HWS Signature.
        string SignStringToSign(string stringToSign, byte[] signingKey)
        {
            byte[] hm = Hmacsha256(signingKey, stringToSign);
            return ToHexString(hm);
        }

        // HexEncodeSHA256Hash returns hexcode of sha256
        public static string HexEncodeSHA256Hash(byte[] body)
        {
            SHA256 sha256 = new SHA256Managed();
            var bytes = sha256.ComputeHash(body);
            sha256.Clear();
            return ToHexString(bytes);
        }

        public static string HexEncodeSHA256HashFile(string fname)
        {
            SHA256 sha256 = new SHA256Managed();
            using (var fs = new FileStream(fname, FileMode.Open))
            {
                var bytes = sha256.ComputeHash(fs);
                sha256.Clear();
                return ToHexString(bytes);
            }
        }

        // Get the finalized value for the "Authorization" header. The signature parameter is the output from SignStringToSign
        string AuthHeaderValue(string signature, List<string> signedHeaders)
        {
            return string.Format("{0} Access={1}, SignedHeaders={2}, Signature={3}", Algorithm, key, string.Join(";", signedHeaders), signature);
        }

        public bool Verify(HttpRequest r, string signature)
        {
            if (r.method != "POST" && r.method != "PATCH" && r.method != "PUT")
            {
                r.body = "";
            }
            var time = r.headers.GetValues(HeaderXDate);
            if (time == null)
            {
                return false;
            }
            DateTime t = DateTime.ParseExact(time[0], BasicDateFormat, CultureInfo.CurrentCulture);
            var signedHeaders = SignedHeaders(r);
            var canonicalRequest = CanonicalRequest(r, signedHeaders);
            var stringToSign = StringToSign(canonicalRequest, t);
            return signature == SignStringToSign(stringToSign, Encoding.UTF8.GetBytes(secret!));
        }

        // SignRequest set Authorization header
        public HttpWebRequest Sign(HttpRequest r)
        {
            if (r.method != "POST" && r.method != "PATCH" && r.method != "PUT")
            {
                r.body = "";
            }
            var time = r.headers.GetValues(HeaderXDate);
            DateTime t;
            if (time == null)
            {
                t = DateTime.Now;
                r.headers.Add(HeaderXDate, t.ToUniversalTime().ToString(BasicDateFormat));
            }
            else
            {
                t = DateTime.ParseExact(time[0], BasicDateFormat, CultureInfo.CurrentCulture);
            }
            var queryString = CanonicalQueryString(r);
            if (queryString != "")
            {
                queryString = "?" + queryString;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(r.host + r.uri + queryString);
            string host = null;
            if (r.headers.GetValues(HeaderHost) != null)
            {
                host = r.headers.GetValues(HeaderHost)[0];
                req.Host = host;
            }
            else
            {
                host = req.Host;
            }

            r.headers.Set("host", host);
            var signedHeaders = SignedHeaders(r);
            var canonicalRequest = CanonicalRequest(r, signedHeaders);
            var stringToSign = StringToSign(canonicalRequest, t);
            var signature = SignStringToSign(stringToSign, Encoding.UTF8.GetBytes(secret!));
            var authValue = AuthHeaderValue(signature, signedHeaders);
            r.headers.Set(HeaderAuthorization, authValue);
            req.Method = r.method;
            r.headers.Remove("host");
            string[] reservedHeaders = new String[]
            {
                "content-type","accept","date","if-modified-since","referer","user-agent",
            };
            Dictionary<string, string> savedHeaders = new Dictionary<string, string>();
            foreach (string header in reservedHeaders)
            {
                if (r.headers.GetValues(header) != null)
                {
                    savedHeaders[header] = r.headers.GetValues(header)[0];
                    r.headers.Remove(header);
                }
            }
            req.Headers = r.headers;
            if (savedHeaders.ContainsKey("content-type"))
            {
                req.ContentType = savedHeaders["content-type"];
            }
            if (savedHeaders.ContainsKey("accept"))
            {
                req.Accept = savedHeaders["accept"];
            }
            if (savedHeaders.ContainsKey("date"))
            {
                req.Date = Convert.ToDateTime(savedHeaders["date"]);
            }
            if (savedHeaders.ContainsKey("if-modified-since"))
            {
                req.IfModifiedSince = Convert.ToDateTime(savedHeaders["if-modified-since"]);
            }
            if (savedHeaders.ContainsKey("referer"))
            {
                req.Referer = savedHeaders["referer"];
            }
            if (savedHeaders.ContainsKey("user-agent"))
            {
                req.UserAgent = savedHeaders["user-agent"];
            }
            return req;
        }

        public HttpRequestMessage SignHttp(HttpRequest r)
        {
            var queryString = CanonicalQueryString(r);
            if (queryString != "")
            {
                queryString = "?" + queryString;
            }
            HttpRequestMessage req = new HttpRequestMessage(new HttpMethod(r.method!), r.host + r.uri + queryString);
            if (r.method != "POST" && r.method != "PATCH" && r.method != "PUT")
            {
                r.body = "";
            }
            else
            {
                req.Content = new StringContent(r.body!);
            }
            var time = r.headers.GetValues(HeaderXDate);
            DateTime t;
            if (time == null)
            {
                t = DateTime.Now;
                r.headers.Add(HeaderXDate, t.ToUniversalTime().ToString(BasicDateFormat));
            }
            else
            {
                t = DateTime.ParseExact(time[0], BasicDateFormat, CultureInfo.CurrentCulture);
            }
            string host = null;
            if (r.headers.GetValues(HeaderHost) != null)
            {
                host = r.headers.GetValues(HeaderHost)[0];
                req.Headers.Host = host;
            }
            else
            {
                host = req.RequestUri.Host;
            }

            r.headers.Set("host", host);
            var signedHeaders = SignedHeaders(r);
            var canonicalRequest = CanonicalRequest(r, signedHeaders);
            r.canonicalRequest = canonicalRequest;
            var stringToSign = StringToSign(canonicalRequest, t);
            r.stringToSign = stringToSign;
            var signature = SignStringToSign(stringToSign, Encoding.UTF8.GetBytes(secret!));
            var authValue = AuthHeaderValue(signature, signedHeaders);
            r.headers.Set(HeaderAuthorization, authValue);
            r.headers.Remove("host");
            foreach (string key in r.headers.AllKeys)
            {
                req.Headers.TryAddWithoutValidation(key, r.headers[key]);
            }

            return req;
        }

    }
}
