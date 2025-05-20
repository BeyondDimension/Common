using BD.Common8.Extensions;
using BD.Common8.Helpers;
using BD.Common8.SmsSender.Models.SmsSender;
using BD.Common8.SmsSender.Models.SmsSender.Abstractions;
using BD.Common8.SmsSender.Models.SmsSender.Channels.TencentCloud;
using Microsoft.Extensions.Logging;
using System.Extensions;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SmsOptions = BD.Common8.SmsSender.Models.SmsSender.Channels.TencentCloud.SmsTencentCloudOptions;

namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.TencentCloud;

/// <summary>
/// 短信服务提供商 - 腾讯云
/// </summary>
public partial class SmsSenderProvider : SmsSenderBase, ISmsSender
{
    /// <summary>
    /// 阿里云的名称
    /// </summary>
    public const string Name = nameof(TencentCloud);

    /// <inheritdoc/>
    public override string Channel => Name;

    /// <inheritdoc/>
    public override bool SupportCheck => false;

    readonly HttpClient httpClient;
    readonly SmsOptions options;
    readonly ILogger logger;

    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = null,
    };

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

    private const string Endpoint = "https://sms.tencentcloudapi.com";
    private const string Version = "2021-01-11";
    private const string SdkVersion = "SDK_NET_3.0.1207";
    private const string Action = "SendSms";
    private const string Method = "POST";
    private const string ContentType = "application/json";

    #endregion

    private Dictionary<string, string> BuildHeaders(byte[] requestPayload)
    {
        // https://github.com/TencentCloud/tencentcloud-sdk-dotnet/blob/8a2d9b3e0247eb258058d8a557e5f2e08cdb6b34/TencentCloud/Common/AbstractClient.cs#L302

        string endpoint = Endpoint;
        string httpRequestMethod = Method;
        string contentType = ContentType;
        string canonicalQueryString = "";

        string canonicalURI = "/";
        string canonicalHeaders = "content-type:" + contentType + "\nhost:" + endpoint + "\n";
        string signedHeaders = "content-type;host";

        var hashBytes = Hashs.ByteArray.SHA256(requestPayload);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; ++i)
        {
            builder.Append(hashBytes[i].ToString("x2"));
        }

        string hashedRequestPayload = builder.ToString();
        string canonicalRequest = httpRequestMethod + "\n"
                                                    + canonicalURI + "\n"
                                                    + canonicalQueryString + "\n"
                                                    + canonicalHeaders + "\n"
                                                    + signedHeaders + "\n"
                                                    + hashedRequestPayload;

        string algorithm = "TC3-HMAC-SHA256";
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        string requestTimestamp = timestamp.ToString();
        string date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp)
            .ToString("yyyy-MM-dd");
        string service = endpoint.Split('.')[0];
        string credentialScope = date + "/" + service + "/" + "tc3_request";
        string hashedCanonicalRequest = Hashs.String.SHA256(canonicalRequest);
        string stringToSign = algorithm + "\n"
                                        + requestTimestamp + "\n"
                                        + credentialScope + "\n"
                                        + hashedCanonicalRequest;

        byte[] tc3SecretKey = Encoding.UTF8.GetBytes("TC3" + options.SecretKey);
        byte[] secretDate = HMACSHA256.HashData(tc3SecretKey, Encoding.UTF8.GetBytes(date));
        byte[] secretService = HMACSHA256.HashData(secretDate, Encoding.UTF8.GetBytes(service));
        byte[] secretSigning = HMACSHA256.HashData(secretService, Encoding.UTF8.GetBytes("tc3_request"));
        byte[] signatureBytes = HMACSHA256.HashData(secretSigning, Encoding.UTF8.GetBytes(stringToSign));
        string signature = Convert.ToHexStringLower(signatureBytes);

        string authorization = algorithm + " "
                                         + "Credential=" + options.SecretId + "/" + credentialScope + ", "
                                         + "SignedHeaders=" + signedHeaders + ", "
                                         + "Signature=" + signature;

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", authorization);
        headers.Add("Host", endpoint);
        headers.Add("Content-Type", contentType);
        headers.Add("X-TC-Timestamp", requestTimestamp);
        headers.Add("X-TC-Version", Version);
        headers.Add("X-TC-Region", "");
        headers.Add("X-TC-RequestClient", SdkVersion);
        headers.Add("X-TC-Language", "zh-CN");
        headers.Add("X-TC-Action", Action);

        return headers;
    }

    private HttpRequestMessage GenerateHttpRequestMessage(string number, string message, string templateId)
    {
        // https://cloud.tencent.com/document/api/382/55981

        var params_dic = new Dictionary<string, object>();
        params_dic.Add("Action", Action);
        params_dic.Add("Version", Version);
        params_dic.Add("Region", "");
        params_dic.Add("SmsSdkAppId", options.SmsSdkAppId!);
        params_dic.Add("TemplateId", templateId);
        params_dic.Add("SignName", options.SignName!);
        params_dic.Add("PhoneNumberSet", new string[] { number });
        params_dic.Add("TemplateParamSet", new string[] { message });

        var requestPayload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(params_dic, JsonSerializerOptions));

        var headers = BuildHeaders(requestPayload);

        var requestUri = $"{Endpoint}/{Action}";
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);

        foreach (KeyValuePair<string, string> kvp in headers)
        {
            if (kvp.Key.Equals("Content-Type"))
            {
                ByteArrayContent content = new ByteArrayContent(requestPayload);
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", kvp.Value);
                requestMessage.Content = content;
            }
            else if (kvp.Key.Equals("Host"))
            {
                requestMessage.Headers.Host = kvp.Value;
            }
            else if (kvp.Key.Equals("Authorization"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("TC3-HMAC-SHA256",
                    kvp.Value["TC3-HMAC-SHA256".Length..]);
            }
            else
            {
                requestMessage.Headers.Add(kvp.Key, kvp.Value);
            }
        }

        return requestMessage;
    }

    /// <inheritdoc/>
    public override async Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken)
    {
        var template_code = options.Templates?.FirstOrDefault(x => x.Type == type)?.Template ?? options.DefaultTemplate;

        using var request = GenerateHttpRequestMessage(number, message, template_code.ThrowIsNull(nameof(template_code)));
        using var response = await httpClient.SendAsync(request, cancellationToken);

        var isSuccess = false;
        TencentCloudResult<SendSmsTencentCloudResult>? tencentCloudResult = null;

        if (response.IsSuccessStatusCode)
        {
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            tencentCloudResult = await JsonSerializer.DeserializeAsync<TencentCloudResult<SendSmsTencentCloudResult>>(stream, JsonSerializerOptions, cancellationToken);

            isSuccess =
                tencentCloudResult != null &&
                tencentCloudResult.Response.IsOk();
        }

        var result = new SendSmsResult<TencentCloudResult<SendSmsTencentCloudResult>>
        {
            HttpStatusCode = (int)response.StatusCode,
            IsSuccess = isSuccess,
            Result = tencentCloudResult,
            ResultObject = tencentCloudResult,
        };

        if (!result.IsSuccess)
        {
            SendSmsError(logger, PhoneNumberHelper.ToStringHideMiddleFour(number), message, type, result.HttpStatusCode);
        }

        return result;
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message =
"""
调用腾讯云短信接口失败，手机号码：{phoneNumber}，短信内容：{message}，短信类型：{type}，HTTP 响应状态码：{httpStatusCode}
""")]
    private static partial void SendSmsError(ILogger logger, string phoneNumber, string? message, ushort type, int httpStatusCode);

    /// <inheritdoc/>
    public override Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}
