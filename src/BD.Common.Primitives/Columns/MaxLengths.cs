// ReSharper disable once CheckNamespace

namespace BD.Common.Columns;

public static class MaxLengths
{
    /// <summary>
    /// 颜色16进制值，#AARRGGBB
    /// </summary>
    public const int ColorHex = 9;

    /// <summary>
    /// 昵称
    /// </summary>
    public const int NickName = 20;

    /// <summary>
    /// 短信验证码
    /// </summary>
    public const int SMS_CAPTCHA = 6;

    public const int UserName = 128;
    public const int Url = 2048;
    public const int Text = 1000;
    public const int FileExtension = 16;
    public const int WeChatId = 128;
    public const int WeChatUnionId = 192;
    public const int Email = 256;
    public const int DisableReason = 1000;
    public const int Remarks = 1000;
    public const int RealityAddress = 150;
    public const int Title = 30;
    public const int LongTitle = 200;
    public const int Describe = 500;
}
