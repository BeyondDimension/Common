// ReSharper disable once CheckNamespace
namespace BD.Common.Columns;

public abstract class SharedMaxLengths
{
    protected SharedMaxLengths() => throw new InvalidOperationException();

    public const int NickName = 32;
    public const int UserName = 128;
    public const int Url = 2048;
    public const int PhoneNumber = 32;
    public const int Text = 1000;
    public const int FileExtension = 16;
    public const int WeChatId = 128;
    public const int WeChatUnionId = 192;
    public const int Email = 256;
    public const int DisableReason = 1000;
    public const int Remarks = 1000;
}
