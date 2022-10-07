namespace BD.Common.Models;

public class SelectItemDTO : ITitle, IDisable
{
    public string Title { get; set; } = "";

    public bool Disable { get; set; }

    public const int Count = 100;
}

public class SelectItemDTO<T> : SelectItemDTO
{
    public T? Id { get; set; }

#if BLAZOR
    public static readonly SelectItemDTO<T> Null = new()
    {
        Id = default,
        Title = SharedStrings.All,
    };

    public string ToDisableStatus()
    {
        var equalityComparer = EqualityComparer<T>.Default;
        if (equalityComparer.Equals(Id, default) || (IsEnumOrInteger(typeof(T)) && equalityComparer.Equals(Id, ConvertibleHelper.Convert<T, sbyte>(sbyte.MinValue)))) return "default";
        if (Disable) return "error";
        return "success";

        static bool IsEnumOrInteger(Type type) => type.IsEnum || Type.GetTypeCode(type) switch
        {
            TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 => true,
            _ => false,
        };
    }
#endif
}
