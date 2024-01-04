using static BD.Common8.Primitives.ApiRsp.Resources.SR;

namespace BD.Common8.Extensions;

/// <summary>
/// 提供了 <see cref="ApiRspCode"/> 对象的扩展方法
/// </summary>
public static partial class ApiRspExtensions
{
    /// <summary>
    /// 判断 <see cref="ApiRspCode"/> 是否为 <see cref="ApiRspCode.ClientException"/>
    /// <para>如果是 <see cref="ApiRspCode.ClientException"/> 返回 <see langword="true"/>；否则为 <see langword="false"/></para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsClientExceptionOrServerException(ApiRspCode code) => code switch
    {
        ApiRspCode.ClientDeserializeFail => true,
        ApiRspCode.NetworkConnectionInterruption => true,
        ApiRspCode.EmptyUserAgent => true,
        ApiRspCode.RequiredSecurityKey => true,
        ApiRspCode.IsNotOfficialChannelPackage => true,
        ApiRspCode.EmptyDbAppVersion => true,
        ApiRspCode.RSADecryptFail => true,
        ApiRspCode.AesKeyIsNull => true,
        ApiRspCode.TaskCanceled => true,
        ApiRspCode.OperationCanceled => true,
        ApiRspCode.Timeout => true,
        ApiRspCode.ClientException => true,
        _ => false,
    };

    /// <summary>
    /// 获取 <see cref="ApiRspCode"/> 对应的错误消息
    /// </summary>
    public static string GetMessage(this ApiRspCode code, string? errorAppendText = null, string? errorFormat = null)
    {
        if (code == ApiRspCode.OK || code == ApiRspCode.Canceled)
            return string.Empty;
        else if (code == ApiRspCode.Unauthorized)
            return ApiResponseCode_Unauthorized;
        else if (code == ApiRspCode.IsNotOfficialChannelPackage)
            return IsNotOfficialChannelPackageWarning;
        else if (code == ApiRspCode.AppObsolete)
            return ApiResponseCode_AppObsolete;
        else if (code == ApiRspCode.UserIsBan)
            return ApiResponseCode_UserIsBan;
        else if (code == ApiRspCode.CertificateNotYetValid)
            return ApiResponseCode_CertificateNotYetValid;
        else if (code == ApiRspCode.NetworkConnectionInterruption)
            return NetworkConnectionInterruption;
        else if (code == ApiRspCode.BadGateway)
            return ApiResponseCode_BadGateway;
        string message;
        var notErrorAppendText = string.IsNullOrWhiteSpace(errorAppendText);
        if (string.IsNullOrWhiteSpace(errorFormat))
            if (notErrorAppendText)
                errorFormat = IsClientExceptionOrServerException(code) ? ClientError_ : ServerError_;
            else
                errorFormat = IsClientExceptionOrServerException(code) ? ClientError__ : ServerError__;
        if (notErrorAppendText)
            message = errorFormat.Format($"{(int)code}({code})");
        else
            message = errorFormat.Format($"{(int)code}({code})", errorAppendText);
        return message;
    }

    /// <summary>
    /// 创建 <see cref="IApiRsp"/> 对应的错误消息
    /// </summary>
    public static string CreateMessage(this IApiRsp response, string? errorAppendText = null, string? errorFormat = null)
    {
        string message = string.Empty;
        if (response.Code == ApiRspCode.ClientException &&
               response is ApiRspBase impl &&
               impl.ClientException != null)
        {
            var exMsg = impl.ClientException.GetAllMessage();
            if (string.IsNullOrWhiteSpace(errorAppendText))
                errorAppendText = exMsg;
            else
                errorAppendText = $"{errorAppendText}{Environment.NewLine}{exMsg}";
        }
        message = response.Code.GetMessage(errorAppendText, errorFormat);
        return message;
    }

    /// <summary>
    /// 获取 <see cref="IApiRsp"/> 对应的错误消息
    /// </summary>
    public static string GetMessage(this IApiRsp response, string? errorAppendText = null, string? errorFormat = null)
    {
        var message = ApiRspHelper.GetInternalMessage(response);
        if (string.IsNullOrWhiteSpace(message))
        {
            message = CreateMessage(response, errorAppendText, errorFormat);
        }
        return message;
    }
}
