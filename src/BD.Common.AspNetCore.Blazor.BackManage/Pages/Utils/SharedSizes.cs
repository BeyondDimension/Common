// ReSharper disable once CheckNamespace
namespace BD.Common.Pages.Utils;

public abstract class SharedSizes
{
    protected SharedSizes() => throw new InvalidOperationException();

    public const string Width_Id = "60px";
    public const string Width_State_4 = "110px";

    public const string Width_Action_1 = "98px";
    public const string Width_Action_1_4 = "130px";
    public const string Width_Action_2 = "200px";
    public const string Width_Action_3 = "282px";
    public const string Width_Action_4 = "372px";

    public const string Width_Type = "160px";
    public const string Width_Date = "73px";
    public const string Width_PublishUser = "130px";
    public const string Width_DateTime = "128px";
    public const string Height_Max_Modal_Body = "calc(100vh - 306px)";

    public const string Width_Guid = "300px";
    public const string Width_ShortGuid = "240px";
    public const string Width_Int64 = "180px";

    public const string Width_WXOpenId = "280px";
    public const string Width_WXUnionId = "280px";
    public const string Width_Gender = "62px";
    public const string Width_BadgeChinese1 = "48px";
    public const string Width_Chinese4 = "74px";

    public const int PageSize_QueryRow2 = 11;
    public const int PageSize_QueryRow1 = 12;

    public static readonly ListGridType Grid_ImageAttachment = new()
    {
        Gutter = 16,
        Xs = 1,
        Sm = 2,
        Md = 4,
        Lg = 4,
        Xl = 6,
        Xxl = 3,
    };
}
