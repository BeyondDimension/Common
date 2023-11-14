namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.HuaweiCloud;

using SmsOptions = BD.Common8.SmsSender.Models.SmsSender.Channels.HuaweiCloud.SmsHuaweiCloudOptions;

/// <summary>
/// 短信服务提供商 - 华为云
/// </summary>
public class SmsSenderProvider : SmsSenderBase, ISmsSender
{
    /// <summary>
    /// 华为云的名称
    /// </summary>
    public const string Name = nameof(HuaweiCloud);

    /// <inheritdoc/>
    public override string Channel => Name;

    /// <inheritdoc/>
    public override bool SupportCheck => false;

    readonly HttpClient httpClient;
    readonly SmsOptions options;
    readonly ILogger logger;

    /// <summary>
    /// 初始化 <see cref="SmsSenderProvider"/> 类的实例，设置所需的日志记录器、配置选项和 HttpClient
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentException"></exception>
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

    #region helpers

    static async Task<HttpRequestMessage> Sign(string appKey, string appSecret, string apiAddress, FormUrlEncodedContent bodyContent, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new(HttpMethod.Post, apiAddress)
        {
            Content = bodyContent,
        };

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
        using var canonicalRequest = await CanonicalRequest(request, signedHeaders, bodyContent, cancellationToken);
        using var stringToSign = await StringToSign(canonicalRequest, xdateTime);
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

    static async Task<MemoryStream> CanonicalRequest(HttpRequestMessage req, IList<string> signedHeaders, FormUrlEncodedContent body, CancellationToken cancellationToken)
    {
        var bodyString = await body.ReadAsStringAsync(cancellationToken);
        var data = Encoding.UTF8.GetBytes(bodyString);
        using MemoryStream stream = new(data);
        var bytes = await SHA256.HashDataAsync(stream, cancellationToken);
        string hexencode = bytes.ToHexString(true);
        var memorySteam = new MemoryStream();
        memorySteam.WriteUtf16StrToUtf8OrCustom(req.Method.ToString());
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
                memorySteam.WriteUtf16StrToUtf8OrCustom(HttpUtility.UrlEncode(v));
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
                        memorySteam.WriteUtf16StrToUtf8OrCustom(key);
                        memorySteam.Write(":"u8);
                        memorySteam.WriteUtf16StrToUtf8OrCustom(value.Trim());
                        memorySteam.Write("\n"u8);
                        req.Headers.Remove(key);
                        req.Headers.TryAddWithoutValidation(key, Encoding.GetEncoding("iso-8859-1").GetString(Encoding.UTF8.GetBytes(value)));
                    }
                }
            }
        }

        for (int i = 0; i < signedHeaders.Count; i++)
        {
            memorySteam.WriteUtf16StrToUtf8OrCustom(signedHeaders[i]);
            if (i != signedHeaders.Count - 1)
                memorySteam.Write(";"u8);
        }

        memorySteam.Write("\n"u8);
        memorySteam.WriteUtf16StrToUtf8OrCustom(hexencode);
        return memorySteam;
    }

    static async Task<MemoryStream> StringToSign(MemoryStream canonicalRequest, DateTime time)
    {
        canonicalRequest.Position = 0;
        var bytes = await SHA256.HashDataAsync(canonicalRequest);
        MemoryStream memorySteam = new();
        memorySteam.WriteUtf16StrToUtf8OrCustom(Algorithm);
        memorySteam.Write("\n"u8);
        memorySteam.WriteUtf16StrToUtf8OrCustom(time.ToUniversalTime().ToString(BasicDateFormat));
        memorySteam.Write("\n"u8);
        memorySteam.WriteUtf16StrToUtf8OrCustom(bytes.ToHexString(true));
        return memorySteam;
    }

    static string SignStringToSign(MemoryStream stringToSign, byte[] signingKey)
    {
        stringToSign.Position = 0;
        using var hmacsha256 = new HMACSHA256(signingKey);
        var hm = hmacsha256.ComputeHash(stringToSign);
        return hm.ToHexString(true);
    }

    static string AuthHeaderValue(string signature, string appKey, IList<string> signedHeaders)
    {
        using MemoryStream memorySteam = new();
        memorySteam.WriteUtf16StrToUtf8OrCustom(Algorithm);
        memorySteam.Write(" Access="u8);
        memorySteam.WriteUtf16StrToUtf8OrCustom(appKey);
        memorySteam.Write(", SignedHeaders="u8);
        for (int i = 0; i < signedHeaders.Count; i++)
        {
            memorySteam.WriteUtf16StrToUtf8OrCustom(signedHeaders[i]);
            if (i != signedHeaders.Count - 1)
                memorySteam.Write(";"u8);
        }
        memorySteam.Write(", Signature="u8);
        memorySteam.WriteUtf16StrToUtf8OrCustom(signature);
        return Encoding.UTF8.GetString(memorySteam.ToArray());
    }

    #endregion

    /// <inheritdoc/>
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
            jsonObject = await ReadFromJsonAsync(response.Content, SmsSenderJsonSerializerContext.Default.SendHuaweiCloudResult, cancellationToken);
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
                $"调用华为云短信接口失败，" +
                $"手机号码：{PhoneNumberHelper.ToStringHideMiddleFour(number)}，" +
                $"短信内容：{message}，" +
                $"短信类型：{type}，" +
                $"HTTP状态码：{result.HttpStatusCode}");
        }
        return result;
    }

    /// <inheritdoc/>
    public override Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
