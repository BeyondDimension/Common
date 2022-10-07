// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

public class DisableState : ITitle
{
    public NullableBoolean Id { get; set; }

    public string Title { get; set; } = "";

    public static readonly DisableState True = new()
    {
        Id = NullableBoolean.True,
        Title = SharedStrings.DisableTrue,
    };

    public static readonly DisableState False = new()
    {
        Id = NullableBoolean.False,
        Title = SharedStrings.DisableFalse,
    };

    public static readonly DisableState Null = new()
    {
        Id = NullableBoolean.Null,
        Title = SharedStrings.All,
    };

    public static readonly DisableState[] Items = new[] { Null, False, True, };
}

public static partial class NullableBooleanEnumExtensions
{
    public static string ToDisableString(this NullableBoolean value) => value switch
    {
        NullableBoolean.True => SharedStrings.True,
        NullableBoolean.False => SharedStrings.False,
        _ => string.Empty,
    };

    public static string ToDisableStatus(this NullableBoolean value) => value switch
    {
        NullableBoolean.True => "error",
        NullableBoolean.False => "success",
        _ => "default",
    };
}