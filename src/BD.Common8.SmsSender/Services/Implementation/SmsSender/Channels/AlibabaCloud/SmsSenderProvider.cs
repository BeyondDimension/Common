using SmsOptions = BD.Common8.SmsSender.Models.SmsSender.Channels.AlibabaCloud.SmsAlibabaCloudOptions;

namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.AlibabaCloud;

/// <summary>
/// 短信服务提供商 - 阿里云
/// </summary>
public class SmsSenderProvider : SmsSenderBase, ISmsSender
{
    /// <summary>
    /// 阿里云的名称
    /// </summary>
    public const string Name = nameof(AlibabaCloud);

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

    /// <summary>
    /// 短信API产品名称（短信产品名固定，无需修改）
    /// </summary>
    const string product = "Dysmsapi";

    /// <summary>
    /// 短信API产品域名（接口地址固定，无需修改）
    /// </summary>
    const string domain = "dysmsapi.aliyuncs.com";

    #endregion

    #region helpers

    /// <summary>
    /// 日期格式
    /// </summary>
    const string ISO8601_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    /// <summary>
    /// 编码文本
    /// </summary>
    const string encode_text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

    /// <summary>
    /// 格式化日期
    /// </summary>
    static string FormatIso8601Date(DateTimeOffset date)
       => date.ToUniversalTime().ToString(ISO8601_DATE_FORMAT, CultureInfo.CreateSpecificCulture("en-US"));

    /// <summary>
    /// 对字符串进行 ACS URL 编码
    /// </summary>
    static string AcsUrlEncode(string s)
    {
        var values = Encoding.UTF8.GetBytes(s).Select(x =>
        {
            var c = (char)x;
            if (encode_text.Contains(c))
                return c.ToString();
            else
                return "%" + string.Format(CultureInfo.InvariantCulture, "{0:X2}", x);
        });

        return string.Join(string.Empty, values);
    }

    /// <summary>
    /// 对字符串进行哈希签名，使用指定的密钥生成签名
    /// </summary>
    private static string HashSign(string s, string secret)
    {
        using var hash = new HMACSHA1(Encoding.UTF8.GetBytes(secret.ToCharArray()));
        return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(s)));
    }

    #endregion

    /// <inheritdoc/>
    public override async Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.Now;
        var template_code = options.Templates?.FirstOrDefault(x => x.Type == type)?.Template ?? options.DefaultTemplate;

        var templateParam = "{\"code\":\"" + message + "\"}"; // JsonConvert.SerializeObject(new { code = message });

        var args = new SortedDictionary<string, string>(StringComparer.Ordinal) // https://help.aliyun.com/document_detail/56189.html
        {
#region (系统参数)为POP协议的基本参数，有
           { "AccessKeyId", options.AccessKeyId.ThrowIsNull(nameof(options.AccessKeyId)) },
           { "Timestamp", FormatIso8601Date(now) }, // 格式为：yyyy-MM-dd’T’HH:mm:ss’Z’；时区为：GMT
           { "Format", "JSON" }, // 没传默认为JSON，可选填值：XML
           { "SignatureMethod", "HMAC-SHA1" }, // 建议固定值：HMAC-SHA1
           { "SignatureVersion", "1.0" }, // 建议固定值：1.0
           { "SignatureNonce", Guid.NewGuid().ToString("N") + Random2.GenerateRandomString(3) },
           // 用于请求的防重放攻击，每次请求唯一，JAVA语言建议用：java.util.UUID.randomUUID()生成即可
           //{ "Signature", "" }, // 最终生成的签名结果值

        #endregion

#region 业务参数
           { "Action", "SendSms" }, // API的命名，固定值，如发送短信API的值为：SendSms
           { "Version", "2017-05-25" }, // API的版本，固定值，如短信API的值为：2017-05-25
           { "RegionId", "cn-hangzhou" }, // API支持的RegionID，如短信API的值为：cn-hangzhou

           // https://help.aliyun.com/document_detail/55284.html
           { "PhoneNumbers", number }, // 短信接收号码,支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式；发送国际/港澳台消息时，接收号码格式为00+国际区号+号码，如“0085200000000”
           { "SignName", options.SignName.ThrowIsNull(nameof(options.SignName)) }, // 短信签名 正式环境要写入APP的名称。
           { "TemplateCode", template_code.ThrowIsNull(nameof(template_code)) }, // 短信模板ID，发送国际/港澳台消息时，请使用国际/港澳台短信模版
           { "TemplateParam", templateParam }, // 短信模板变量替换JSON串,友情提示:如果JSON中需要带换行符,请参照标准的JSON协议。

#endregion
        };

        var queryArray = args.Select(x => $"{AcsUrlEncode(x.Key)}={AcsUrlEncode(x.Value)}");
        var queryString = string.Join("&", queryArray);

        var signArray = new[]
        {
            "GET",
            "&",
            AcsUrlEncode("/"),
            "&",
            AcsUrlEncode(queryString),
        };

        var signString = string.Join(string.Empty, signArray);
        signString = HashSign(signString, options.AccessKeySecret + "&");
        signString = AcsUrlEncode(signString);

        args.Add("Signature", signString);

        var requestUri = $"https://{domain}?Signature={signString}&{queryString}";
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);

        var isSuccess = false;
        SendSmsAlibabaCloudResult? jsonObject = null;

        if (response.IsSuccessStatusCode)
        {
            jsonObject = await ReadFromJsonAsync(response.Content, SmsSenderJsonSerializerContext.Default.SendSmsAlibabaCloudResult, cancellationToken);
            isSuccess = jsonObject != default && jsonObject.IsOK();
        }

        var result = new SendSmsResult<SendSmsAlibabaCloudResult>
        {
            HttpStatusCode = (int)response.StatusCode,
            IsSuccess = isSuccess,
            Result = jsonObject,
            ResultObject = jsonObject,
        };

        if (!result.IsSuccess)
            logger.LogError(
                $"调用阿里云短信接口失败，" +
                $"手机号码：{PhoneNumberHelper.ToStringHideMiddleFour(number)}，" +
                $"短信内容：{message}，" +
                $"短信类型：{type}，" +
                $"HTTP状态码：{result.HttpStatusCode}");

        return result;
    }

    /// <inheritdoc/>
    public override Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}