namespace BD.Common.Models;

public class NullableEnum<TEnum> : ITitle where TEnum : struct, Enum
{
    public TEnum? Id { get; set; }

    string? title;

    public string Title
    {
        get
        {
            title ??= Id.HasValue ? ToString(Id.Value) : string.Empty;
            return title;
        }
        set => title = value;
    }

    protected virtual string ToString(TEnum value) => value.ToString();

    static readonly Lazy<NullableEnum<TEnum>> _Nullable = new(() => GetNullable<NullableEnum<TEnum>>());

    public static NullableEnum<TEnum> Nullable => _Nullable.Value;

    static readonly Lazy<NullableEnum<TEnum>> _None = new(() => GetNone<NullableEnum<TEnum>>());

    public static NullableEnum<TEnum> None => _None.Value;

    static readonly Lazy<NullableEnum<TEnum>[]> _NotNullItems = new(() => GetItems<NullableEnum<TEnum>>());

    public static NullableEnum<TEnum>[] NotNullItems => _NotNullItems.Value;

    static readonly Lazy<NullableEnum<TEnum>[]> _Items = new(() => GetItems(Nullable));

    public static NullableEnum<TEnum>[] Items => _Items.Value;

    static readonly Lazy<NullableEnum<TEnum>[]> _NItems = new(() => GetItems(None));

    public static NullableEnum<TEnum>[] NItems => _NItems.Value;

    public static TNullableEnum[] GetItems<TNullableEnum>(TNullableEnum? nullable = default)
        where TNullableEnum : NullableEnum<TEnum>, new()
    {
        var none = ConvertibleHelper.Convert<TEnum, sbyte>(sbyte.MinValue);
        var enumerable = Enum.GetValues<TEnum>()
            .Where(x => !EqualityComparer<TEnum>.Default.Equals(none, x))
            .Select(x => new TNullableEnum
            {
                Id = x,
            }).AsEnumerable();
        if (nullable != default) enumerable = new[] { nullable, }.Concat(enumerable);
        return enumerable.ToArray();
    }

    protected static TNullableEnum GetNullable<TNullableEnum>()
        where TNullableEnum : NullableEnum<TEnum>, new() => new()
        {
            Title = SharedStrings.All,
        };

    protected static TNullableEnum GetNone<TNullableEnum>()
        where TNullableEnum : NullableEnum<TEnum>, new() => new()
        {
            Title = SharedStrings.None,
            Id = ConvertibleHelper.Convert<TEnum, sbyte>(sbyte.MinValue),
        };
}

public abstract class NullableEnum<TEnum, TNullableEnum> : NullableEnum<TEnum>
    where TEnum : struct, Enum
    where TNullableEnum : NullableEnum<TEnum, TNullableEnum>, new()
{
    static readonly Lazy<TNullableEnum> _Nullable = new(() => GetNullable<TNullableEnum>());

    public static new TNullableEnum Nullable => _Nullable.Value;

    static readonly Lazy<TNullableEnum> _None = new(() => GetNone<TNullableEnum>());

    static readonly Lazy<TNullableEnum[]> _NotNullItems = new(() => GetItems<TNullableEnum>());

    public static new TNullableEnum[] NotNullItems => _NotNullItems.Value;

    public static new TNullableEnum None => _None.Value;

    static readonly Lazy<TNullableEnum[]> _Items = new(() => GetItems(Nullable));

    public static new TNullableEnum[] Items => _Items.Value;

    static readonly Lazy<TNullableEnum[]> _NItems = new(() => GetItems(None));

    public static new TNullableEnum[] NItems => _NItems.Value;
}
