namespace BD.Common.Services.Implementation.SmsSender.Channels.HuaWeiCloud;

using APIGATEWAY_SDK;
using BD.Common.Models.SmsSender.Channels.HuaWeiCloud;
using SmsOptions = BD.Common.Models.SmsSender.Channels.HuaWeiCloud.SmsHuaWeiCloudOptions;

public class SmsSenderProvider : SmsSenderBase, ISmsSender
{
    public const string Name = nameof(HuaWeiCloud);

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

    public override async Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        var key = options.AppKey.ThrowIsNull(nameof(options.AppKey));
        var sender = options.Sender.ThrowIsNull(nameof(options.Sender));
        var apiAddress = options.Sender.ThrowIsNull(nameof(options.ApiAddress));
        var appSecret = options.Signature.ThrowIsNull(nameof(options.AppSecret));
        var templateId = options.Templates?.FirstOrDefault(x => x.Type == type)?.Template ?? options.DefaultTemplate;
        var statusCallback = options.StatusCallBack.ThrowIsNull(nameof(options.StatusCallBack));
        var signature = options.Signature.ThrowIsNull(nameof(options.Signature));

        var templateParam = $"[\"{message}\"]";
        var body = new Dictionary<string, string>() {
            { "from", sender }, //短信发送方的号码
            { "to", number }, //短信接收方的号码
            { "templateId", templateId.ThrowIsNull(nameof(templateId)) }, //短信模板 ID，用于唯一标识短信模板，请在申请短信模板时获取模板 ID
            { "templateParas", templateParam }, //短信模板的变量值列表
            { "statusCallback", statusCallback }, // 用户的回调地址
            { "signature", signature } //使用国内短信通用模板时,必须填写签名名称
        };

        HttpRequest r = new HttpRequest("POST", new Uri(apiAddress));
        r.body = new FormUrlEncodedContent(body).ReadAsStringAsync().Result;
        r.headers!.Add("Content-Type", "application/x-www-form-urlencoded");

        Signer signer = new Signer();
        signer.Key = key;
        signer.Secret = appSecret;

        var req = signer.Sign(r);

        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        httpClient.DefaultRequestHeaders.Add("x-sdk-date", req.Headers.Get("x-sdk-date"));
        httpClient.DefaultRequestHeaders.Add("authorization", req.Headers.Get("authorization"));
        var content = new StringContent(r.body, Encoding.UTF8);
        var response = await httpClient.PostAsync(apiAddress, content);

        var isSuccess = false;
        SendHuaWeiCloudResult? jsonObject = null;

        if (response.IsSuccessStatusCode)
        {
            jsonObject = await ReadFromJsonAsync<SendHuaWeiCloudResult>(response.Content, cancellationToken);
            isSuccess = jsonObject != default && jsonObject.IsOK();
        }

        var result = new SendSmsResult<SendHuaWeiCloudResult>
        {
            HttpStatusCode = (int)response.StatusCode,
            IsSuccess = isSuccess,
            Result = jsonObject,
            ResultObject = jsonObject,
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
