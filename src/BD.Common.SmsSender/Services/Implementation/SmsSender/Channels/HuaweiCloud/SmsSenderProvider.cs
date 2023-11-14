namespace BD.Common.Services.Implementation.SmsSender.Channels.HuaweiCloud;

using BD.Common.Models.SmsSender.Channels.HuaweiCloud;
using SmsOptions = BD.Common.Models.SmsSender.Channels.HuaweiCloud.SmsHuaweiCloudOptions;

public class SmsSenderProvider : SmsSenderBase, ISmsSender
{
    public const string Name = nameof(HuaweiCloud);

    public override string Channel => Name;

    public override bool SupportCheck => false;

    readonly HttpClient httpClient;
    readonly SmsOptions options;
    readonly ILogger logger;

    public SmsSenderProvider(ILogger<SmsSenderProvider> logger, SmsOptions? options, HttpClient httpClient)
    {
        this.logger = logger;
        if (!options.HasValue()) throw new ArgumentException(null, nameof(options));
        this.options = options.ThrowIsNull(nameof(options));
        this.httpClient = httpClient;
    }

    #region 常量

    const string BasicDateFormat = "yyyyMMddTHHmmssZ";
    const string Algorithm = "SDK-HMAC-SHA256";
    const string HeaderXDate = "X-Sdk-Date";
    const string HeaderHost = "host";
    const string HeaderAuthorization = "Authorization";
    const string HeaderContentSha256 = "X-Sdk-Content-Sha256";

    #endregion

    #region 华为云短信签名 SDK

    async Task<HttpRequestMessage> Sign(string appKey, string appSecret, string apiAddress, FormUrlEncodedContent bodyContent, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new(HttpMethod.Post, apiAddress);
        request.Content = new StringContent(await bodyContent.ReadAsStringAsync(), Encoding.UTF8, "application/x-www-form-urlencoded");

        var time = request.Headers.TryGetValues(HeaderXDate, out var xdate) ? xdate.FirstOrDefault() : null;
        DateTime xdateTime;
        if (time == null)
        {
            xdateTime = DateTime.Now;
            request.Headers.TryAddWithoutValidation(HeaderXDate, xdateTime.ToUniversalTime().ToString(BasicDateFormat));
        }
        else
        {
            xdateTime = DateTime.ParseExact(time, BasicDateFormat, CultureInfo.CurrentCulture);
        }
        request.Headers.TryAddWithoutValidation(HeaderHost, request.RequestUri!.Host);
        var signedHeaders = SignedHeaders(request.Headers);
        var canonicalRequest = await CanonicalRequest(request, signedHeaders, bodyContent, cancellationToken);
        var stringToSign = await StringToSign(canonicalRequest, xdateTime);
        var signature = SignStringToSign(stringToSign, Encoding.UTF8.GetBytes(appSecret));
        var authValue = AuthHeaderValue(signature, appKey, signedHeaders);
        request.Headers.TryAddWithoutValidation(HeaderAuthorization, authValue);
        request.Headers.Remove(HeaderHost);
        return request;
    }

    static SortedList<string> SignedHeaders(HttpHeaders headers)
    {
        SortedList<string> a = new(StringComparer.Ordinal);
        foreach (string key in headers.Select(x => x.Key))
        {
            string keyLower = key.ToLowerInvariant();
            if (keyLower != "content-type")
            {
                a.Add(key.ToLowerInvariant());
            }
        }
        return a;
    }

    async Task<byte[]> CanonicalRequest(HttpRequestMessage req, IList<string> signedHeaders, FormUrlEncodedContent body, CancellationToken cancellationToken)
    {
        var bodyString = await body.ReadAsStringAsync(cancellationToken);
        var data = Encoding.UTF8.GetBytes(bodyString);
        using MemoryStream stream = new(data);
        var bytes = await SHA256.HashDataAsync(stream, cancellationToken);
        string hexencode = bytes.ToHexString(true);
        var memorySteam = new MemoryStream();
        memorySteam.Write(req.Method.ToString());
        memorySteam.Write("\n"u8);
        var requestUri = req.RequestUri!.GetComponents(UriComponents.Path | UriComponents.KeepDelimiter, UriFormat.Unescaped);
        CanonicalURI();
        memorySteam.Write("\n"u8);
        // CanonicalQueryString
        memorySteam.Write("\n"u8);
        void CanonicalURI()
        {
            var pattens = requestUri!.Split('/');
            foreach (var v in pattens)
            {
                memorySteam.Write(HttpUtility.UrlEncode(v));
                memorySteam.Write("/"u8);
            }
        }
        CanonicalHeaders();
        memorySteam.Write("\n"u8);
        void CanonicalHeaders()
        {
            foreach (string key in signedHeaders)
            {
                if (req.Headers!.TryGetValues(key, out var headerValues))
                {
                    SortedList<string> values = new(StringComparer.Ordinal, headerValues);
                    foreach (var value in values)
                    {
                        memorySteam.Write(key);
                        memorySteam.Write(":"u8);
                        memorySteam.Write(value.Trim());
                        memorySteam.Write("\n"u8);
                        req.Headers.Remove(key);
                        req.Headers.TryAddWithoutValidation(key, Encoding.GetEncoding("iso-8859-1").GetString(Encoding.UTF8.GetBytes(value)));
                    }
                }
            }
        }

        foreach (var item in signedHeaders)
        {
            memorySteam.Write(item);
            if (item != signedHeaders[signedHeaders.Count - 1]) memorySteam.Write(";"u8);
        }
        memorySteam.Write("\n"u8);
        memorySteam.Write(hexencode);
        return memorySteam.ToArray();
    }

    async Task<byte[]> StringToSign(byte[] canonicalRequest, DateTime time)
    {
        using MemoryStream stream = new MemoryStream(canonicalRequest);
        var bytes = await SHA256.HashDataAsync(stream);
        using MemoryStream memorySteam = new();
        memorySteam.Write(Algorithm);
        memorySteam.Write("\n");
        memorySteam.Write(time.ToUniversalTime().ToString(BasicDateFormat));
        memorySteam.Write("\n");
        memorySteam.Write(ToHexString(bytes));
        return memorySteam.ToArray();
    }

    string SignStringToSign(byte[] stringToSign, byte[] signingKey)
    {
        using var hmacsha256 = new HMACSHA256(signingKey);
        var hm = hmacsha256.ComputeHash(stringToSign);
        return ToHexString(hm);
    }

    static string ToHexString(byte[] value)
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

    static char GetHexValue(int i)
    {
        if (i < 10)
        {
            return (char)(i + '0');
        }
        return (char)(i - 10 + 'a');
    }

    string AuthHeaderValue(string signature, string appKey, IList<string> signedHeaders)
    {
        return string.Format("{0} Access={1}, SignedHeaders={2}, Signature={3}", Algorithm, appKey, string.Join(";", signedHeaders), signature);
    }

    #endregion

    public override async Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken)
    {
        var appKey = options.AppKey.ThrowIsNull(nameof(options.AppKey));
        var sender = options.Sender.ThrowIsNull(nameof(options.Sender));
        var apiAddress = options.ApiAddress.ThrowIsNull(nameof(options.ApiAddress));
        var appSecret = options.AppSecret.ThrowIsNull(nameof(options.AppSecret));
        var templateId = options.Templates?.FirstOrDefault(x => x.Type == type)?.Template ?? options.DefaultTemplate;
        var statusCallback = options.StatusCallBack.ThrowIsNull(nameof(options.StatusCallBack));
        var signature = options.Signature.ThrowIsNull(nameof(options.Signature));

        var templateParam = $"[\"{message}\"]";
        var body = new Dictionary<string, string>() {
            { "from", sender }, //短信发送方的号码
            { "to", number }, //短信接收方的号码
            { "templateId", templateId.ThrowIsNull(nameof(templateId)) }, //短信模板 ID，用于唯一标识短信模板，请在申请短信模板时获取模板 ID
            { "templateParas", templateParam }, //短信模板的变量值列表
            { "statusCallback", statusCallback }, //用户的回调地址
            { "signature", signature } //使用国内短信通用模板时,必须填写签名名称
             };

        var request = await Sign(appKey, appSecret, apiAddress, new FormUrlEncodedContent(body), cancellationToken);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var isSuccess = false;
        SendHuaweiCloudResult? jsonObject = null;

        if (response.IsSuccessStatusCode)
        {
            jsonObject = await ReadFromJsonAsync<SendHuaweiCloudResult>(response.Content, cancellationToken);
            isSuccess = jsonObject != default && jsonObject.IsOK();
        }

        var result = new SendSmsResult<SendHuaweiCloudResult>
        {
            HttpStatusCode = (int)response.StatusCode,
            IsSuccess = isSuccess,
            Result = jsonObject,
            ResultObject = jsonObject
        };

        if (!result.IsSuccess)
        {
            logger.LogError(
                $"调用化为云短信接口失败，" +
                $"手机号码：{PhoneNumberHelper.ToStringHideMiddleFour(number)}，" +
                $"短信内容：{message}，" +
                $"短信类型：{type}，" +
                $"HTTP状态码：{result.HttpStatusCode}");
        }
        return result;
    }

    public override Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
