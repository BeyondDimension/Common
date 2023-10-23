// https://bcssstorage.blob.core.chinacloudapi.cn/docs/CCS/DEMO.zip

using SmsOptions = BD.Common8.SmsSender.Models.SmsSender.Channels._21VianetBlueCloud.Sms21VianetBlueCloudOptions;

namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels._21VianetBlueCloud;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 短信服务提供商 - 世纪互联蓝云
/// </summary>
public class SmsSenderProvider : SmsSenderBase, ISmsSender
{
    public const string Name = nameof(_21VianetBlueCloud);

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

    const string Schema = "SharedAccessSignature";
    const string SignKey = "sig";
    const string KeyNameKey = "skn";
    const string ExpiryKey = "se";
    const string _endpoint = "https://bluecloudccs.21vbluecloud.com/services/sms/messages?api-version=2018-10-01";

    /// <summary>
    /// create token
    /// </summary>
    /// <param name="key">密钥：密钥分为两种：-full: 可以用于 REST API 和设备端 SDK，-device: 只能用于设备端 SDK</param>
    /// <param name="keyName">full/device</param>
    /// <param name="timeout">超时时间</param>
    /// <returns></returns>
    static string CreateSASToken(string key, string keyName, TimeSpan timeout)
    {
        var values = new Dictionary<string, string>
        {
            { KeyNameKey, keyName },
            { ExpiryKey, (DateTimeOffset.UtcNow + timeout).ToUnixTimeSeconds().ToString() },
        };

        var signContent = string.Join("&", values
            .Where(pair => pair.Key != SignKey)
            .OrderBy(pair => pair.Key)
            .Select(pair => $"{pair.Key}={HttpUtility.UrlEncode(pair.Value)}"));

        string sign;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            sign = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(signContent)));

        return $"{Schema} {SignKey}={HttpUtility.UrlEncode(sign)}&{signContent}";
    }

    public override async Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken)
    {
        var key = options.KeyValue.ThrowIsNull(nameof(options.KeyValue));
        var keyName = options.KeyName.ThrowIsNull(nameof(options.KeyName));
        var template_name = options.Templates?.FirstOrDefault(x => x.Type == type)?.Template ?? options.DefaultTemplate;

        var requestData = new RequestData
        {
            PhoneNumber = [number],
            ExtendCode = options.ExtendCode,
            MessageBody = new MessageBody
            {
                TemplateName = template_name,
                TemplateParam = new()
                {
                    { options.CodeTemplateKeyName, message },
                },
            },
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = GetJsonContent(requestData, SmsSenderJsonSerializerContext.Default.RequestData),
        };

        request.Headers.Add("Account", options.Account);

        var token = CreateSASToken(key, keyName, TimeSpan.FromSeconds(600));
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        var isSuccess = response.IsSuccessStatusCode;
        SendSms21VianetBlueCloudResult? jsonObject = null;

        if (isSuccess)
            jsonObject = await ReadFromJsonAsync(response.Content, SmsSenderJsonSerializerContext.Default.SendSms21VianetBlueCloudResult, cancellationToken);
        else
            logger.LogError(
                $"调用世纪互联蓝云短信接口接口失败，" +
                $"手机号码：{PhoneNumberHelper.ToStringHideMiddleFour(number)}，" +
                $"短信内容：{message}，" +
                //$"Content：{jsonPayload}，" +
                //$"Account：{options.Account}，" +
                //$"SASToken：{token}，" +
                $"TemplateName：{template_name}，" +
                $"HTTP状态码：{(int)response.StatusCode}");

        var result = new SendSmsResult<SendSms21VianetBlueCloudResult>
        {
            HttpStatusCode = (int)response.StatusCode,
            IsSuccess = isSuccess,
            Result = jsonObject,
            ResultObject = jsonObject,
        };

        return result;
    }

    public override Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal sealed class RequestData
    {
        /// <summary>
        /// 接收手机号
        /// </summary>
        [SystemTextJsonProperty("phoneNumber")]
        public string[]? PhoneNumber { get; set; }

        /// <summary>
        /// 下发扩展码，两位纯数字
        /// </summary>
        [SystemTextJsonProperty("extend")]
        public string? ExtendCode { get; set; }

        [SystemTextJsonProperty("messageBody")]
        public MessageBody? MessageBody { get; set; }
    }

    internal sealed class MessageBody
    {
        /// <summary>
        /// 短信模板名称
        /// </summary>
        [SystemTextJsonProperty("templateName")]
        public string? TemplateName { get; set; }

        /// <summary>
        /// 短信模板参数，和模板中变量一一对应,没有变量则不需要
        /// </summary>
        [SystemTextJsonProperty("templateParam")]
        public Dictionary<string, string> TemplateParam { get; set; } = new();
    }
}