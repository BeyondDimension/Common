namespace BD.Common8.Columns;

/// <summary>
/// 最大长度
/// </summary>
public static class MaxLengths
{
    /// <summary>
    /// 颜色 16 进制值，#AARRGGBB
    /// </summary>
    public const int ColorHex = 9;

    /// <summary>
    /// 一般名称
    /// </summary>
    public const int Name = 20;

    /// <summary>
    /// 菜单 Key
    /// </summary>
    public const int MenuKey = 100;

    /// <summary>
    /// 菜单名称
    /// </summary>
    public const int MenuName = 50;

    /// <summary>
    /// 一般长名称
    /// </summary>
    public const int LongName = 400;

    /// <summary>
    /// 昵称
    /// </summary>
    public const int NickName = 20;

    /// <summary>
    /// 图标名称
    /// </summary>
    public const int IconName = 20;

    /// <summary>
    /// 短信验证码
    /// </summary>
    public const int SMS_CAPTCHA = 6;

    /// <summary>
    /// 用户名(用户名不是昵称，通常为唯一键，因可用用户名进行登录)
    /// </summary>
    public const int UserName = 128;

    /// <summary>
    /// Url 地址
    /// </summary>
    public const int Url = 2048;

    /// <summary>
    /// 一般文本字符串
    /// </summary>
    public const int Text = 1000;

    /// <summary>
    /// 文件扩展名，例如 .exe/.dll
    /// </summary>
    public const int FileExtension = 16;

    /// <summary>
    /// 微信 OpenId
    /// </summary>
    public const int WeChatId = 128;

    /// <summary>
    /// 微信 UnionId
    /// </summary>
    public const int WeChatUnionId = 192;

    /// <summary>
    /// 邮箱地址
    /// </summary>
    public const int Email = 256;

    /// <summary>
    /// 现实地址/收货地址
    /// </summary>
    public const int RealityAddress = 600;

    /// <summary>
    /// 标题
    /// </summary>
    public const int Title = 30;

    /// <summary>
    /// 长标题
    /// </summary>
    public const int LongTitle = 200;

    /// <summary>
    /// 哈希密码最大长度
    /// </summary>
    public const int Max_PasswordHash = 256;
}