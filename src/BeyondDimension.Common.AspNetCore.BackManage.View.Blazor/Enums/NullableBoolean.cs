namespace BD.Common.Enums;

public enum NullableBoolean : sbyte
{
    True = 1,
    False = 0,
    Null = -1,
}

public static class NullableBoolean2
{
    public const NullableBoolean Undefined = (NullableBoolean)sbyte.MinValue;

    public static NullableBoolean Parse(string s, NullableBoolean def = Undefined)
        => Enum.TryParse<NullableBoolean>(s, true, out var r) ? r : def;
}