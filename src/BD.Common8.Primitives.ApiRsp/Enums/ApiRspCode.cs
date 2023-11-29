namespace BD.Common8.Enums;

/// <summary>
/// 接口响应状态码
/// <para>200~299 代表成功，三位数对应 HTTP 状态码，四位数为业务自定义响应码</para>
/// </summary>
public enum ApiRspCode
{
    #region Http 状态码，100 ~ 511

    /// <summary>
    /// 成功
    /// </summary>
    OK = 200,

    Unauthorized = 401,

    NotFound = 404,

    BadRequest = 400,

    TooManyRequests = 429,

    InternalServerError = 500,

    BadGateway = 502,

    #endregion

    #region 通用状态码，1000~1999

    /// <summary>
    /// 无响应正文内容
    /// </summary>
    NoResponseContent = 1000,

    /// <summary>
    /// 无响应正文内容值, bool HasValue() 验证失败
    /// </summary>
    NoResponseContentValue = 1001,

    /// <summary>
    /// 失败，原因见 Message 或无原因
    /// </summary>
    Fail = 1002,

    /// <summary>
    /// 客户端反序列化失败
    /// </summary>
    ClientDeserializeFail = 1003,

    /// <summary>
    /// 不支持的响应媒体类型
    /// </summary>
    UnsupportedResponseMediaType = 1004,

    /// <summary>
    /// 不支持的上传文件媒体类型
    /// </summary>
    UnsupportedUploadFileMediaType = 1005,

    /// <summary>
    /// 客户端抛出异常
    /// </summary>
    ClientException = 1006,

    /// <summary>
    /// 被取消
    /// </summary>
    Canceled = 1007,

    /// <summary>
    /// 找不到 HTTP 请求授权头
    /// </summary>
    MissingAuthorizationHeader = 1008,

    /// <summary>
    /// HTTP 请求授权声明不正确
    /// </summary>
    AuthSchemeNotCorrect = 1009,

    /// <summary>
    /// 用户被封禁
    /// </summary>
    UserIsBan = 1010,

    /// <summary>
    /// 找不到用户
    /// </summary>
    UserNotFound = 1011,

    /// <summary>
    /// 文件上传失败，缺少需要上传的有效文件
    /// </summary>
    LackAvailableUploadFile = 1012,

    /// <summary>
    /// 文件上传失败，上传的文件数与服务端响应结果数不相等
    /// </summary>
    UnequalLengthUploadFile = 1013,

    /// <summary>
    /// 请求模型验证失败
    /// </summary>
    RequestModelValidateFail = 1014,

    /// <summary>
    /// 网络连接中断
    /// </summary>
    NetworkConnectionInterruption = 1015,

    /// <summary>
    /// 请求头中缺少 User-Agent
    /// </summary>
    EmptyUserAgent = 1016,

    /// <summary>
    /// 必须使用安全传输模式
    /// </summary>
    RequiredSecurityKey = 1017,

    /// <summary>
    /// 当前运行程序不为官方渠道包
    /// </summary>
    IsNotOfficialChannelPackage = 1018,

    /// <summary>
    /// 客户端版本已弃用，需要更新版本
    /// </summary>
    AppObsolete = 1019,

    /// <summary>
    /// 必须在 WebView3 中打开
    /// </summary>
    RequiredWebView3 = 1020,

    /// <summary>
    /// 证书不在有效期内或本地系统时间不正确
    /// </summary>
    CertificateNotYetValid = 1021,

    /// <summary>
    /// 空的数据库 App 版本号
    /// </summary>
    EmptyDbAppVersion = 1022,

    /// <summary>
    /// RSA 解密失败
    /// </summary>
    RSADecryptFail = 1023,

    /// <summary>
    /// AES Key 不能为 null
    /// </summary>
    AesKeyIsNull = 1024,

    #endregion

    #region 错误状态码 5000~5999

    /// <summary>
    /// 短信服务故障
    /// </summary>
    SMSServerError = 5001,

    /// <inheritdoc cref="TaskCanceledException"/>
    TaskCanceled = 5002,

    /// <inheritdoc cref="OperationCanceledException"/>
    OperationCanceled = 5003,

    #endregion

    #region 业务常量定义 int 状态码，10000 ~ 99999

    // 5 位数，由业务服务接口定义常量 int 值进行转换
    // 此处仅保留

    #endregion
}
