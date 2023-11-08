namespace BD.Common.Services.Implementation.SmsSender.Channels.HuaweiCloud;

using APIGATEWAY_SDK;
using BD.Common.Models.SmsSender.Channels.HuaweiCloud;
using System.Reflection.PortableExecutable;
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

    public override async Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken)
    {
        var key = options.AppKey.ThrowIsNull(nameof(options.AppKey));
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
            { "statusCallback", statusCallback }, // 用户的回调地址
            { "signature", signature } //使用国内短信通用模板时,必须填写签名名称
             };

        HttpRequest r = new HttpRequest("POST", new Uri(apiAddress));
        r.Body = await new FormUrlEncodedContent(body).ReadAsStringAsync();

        Signer signer = new Signer();
        signer.Key = key;
        signer.Secret = appSecret;

        var req = await signer.Sign(r);
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiAddress);
        foreach (string value in req.Headers.AllKeys)
        {
            request.Headers.TryAddWithoutValidation(value, req.Headers[value]);
        }
        request.Content = new StringContent(r.Body, Encoding.UTF8, "application/x-www-form-urlencoded");

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
