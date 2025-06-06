using System.Diagnostics.CodeAnalysis;

namespace ValveKeyValue;

public static partial class KVCollectionValueExtensions // AddOrSet
{
    static void AddOrSet<TValue>(this KVCollectionValue collectionValue, string name, TValue value, KVValueType valueType) where TValue : IConvertible
    {
        var it = collectionValue.Get(name);
        if (it is KVObjectValue<TValue> objValue)
        {
            objValue.Value = value;
        }
        else
        {
            collectionValue.Set(name, new KVObjectValue<TValue>(value, valueType));
        }
    }

    public static void AddOrSet(this KVCollectionValue collectionValue, string name, string value)
        => AddOrSet(collectionValue, name, value, KVValueType.String);

    public static void AddOrSet(this KVCollectionValue collectionValue, string name, float value)
        => AddOrSet(collectionValue, name, value, KVValueType.FloatingPoint);

    public static void AddOrSet(this KVCollectionValue collectionValue, string name, int value, bool isPointer = false)
        => AddOrSet(collectionValue, name, value, isPointer ? KVValueType.Pointer : KVValueType.Int32);

    public static void AddOrSet(this KVCollectionValue collectionValue, string name, ulong value)
        => AddOrSet(collectionValue, name, value, KVValueType.UInt64);
}

public static partial class KVObjectExtensions // Get KVCollectionValue
{
    public static KVCollectionValue GetOrCreateCollection(this KVObject obj, params IEnumerable<string> names)
    {
        KVCollectionValue? collectionVal = null;
        foreach (var name in names)
        {
            if (collectionVal == null)
            {
                if (obj[name] is KVCollectionValue collectionVal1)
                {
                    collectionVal = collectionVal1;
                }
                else
                {
                    collectionVal = new KVCollectionValue();
                    obj[name] = collectionVal;
                }
            }
            else
            {
                if (collectionVal[name] is KVCollectionValue collectionVal1)
                {
                    collectionVal = collectionVal1;
                }
                else
                {
                    collectionVal1 = new KVCollectionValue();
                    collectionVal.Set(name, collectionVal1);
                    collectionVal = collectionVal1;
                }
            }
        }
        return collectionVal!; // names 不为空数组时必定返回非空值
    }

    public static KVCollectionValue? GetCollection(this KVObject? obj, params IEnumerable<string> names)
    {
        if (obj == null)
        {
            return null;
        }
        KVCollectionValue? collectionVal = null;
        foreach (var name in names)
        {
            if (collectionVal == null)
            {
                if (obj[name] is KVCollectionValue collectionVal1)
                {
                    collectionVal = collectionVal1;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (collectionVal[name] is KVCollectionValue collectionVal1)
                {
                    collectionVal = collectionVal1;
                }
                else
                {
                    return null;
                }
            }
        }
        return collectionVal;
    }
}

public static partial class KVCollectionValueExtensions // KVCollectionValue GetItemValue
{
    public static string? GetString(this KVCollectionValue collectionValue, string name, StringComparison comparison = StringComparison.Ordinal)
    {
        var val = collectionValue.FirstOrDefault(x => string.Equals(x.Name, name, comparison))?.Value;
        if (val is KVObjectValue<string> tVal)
        {
            return tVal.Value;
        }
        else
        {
            return val?.ToString();
        }
    }

    public static bool TryParse<T>(this KVCollectionValue collectionValue, string name, [MaybeNullWhen(false)] out T result, StringComparison comparison = StringComparison.Ordinal) where T : IConvertible
#if NET7_0_OR_GREATER
        , IParsable<T>
#endif
    {
        var val = collectionValue.FirstOrDefault(x => string.Equals(x.Name, name, comparison))?.Value;
        if (val is KVObjectValue<T> tVal)
        {
            result = tVal.Value;
            return true;
        }
        else
        {
#if NET7_0_OR_GREATER
            // 通过泛型+模式匹配类型避免 object? 访问值，引发的装箱与拆箱
            var r = T.TryParse(val?.ToString(), null, out result);
            return r;
#else
            if (Convert.ChangeType(val, typeof(T), null) is T tResult)
            {
                result = tResult;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
#endif
        }
    }

    public static int TryParseInt32(this KVCollectionValue collectionValue, string name, int defaultValue = default, StringComparison comparison = StringComparison.Ordinal)
    {
        if (collectionValue.TryParse(name, out int result, comparison))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    public static uint TryParseUInt32(this KVCollectionValue collectionValue, string name, uint defaultValue = default, StringComparison comparison = StringComparison.Ordinal)
    {
        if (collectionValue.TryParse(name, out uint result, comparison))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    public static long TryParseInt64(this KVCollectionValue collectionValue, string name, long defaultValue = default, StringComparison comparison = StringComparison.Ordinal)
    {
        if (collectionValue.TryParse(name, out long result, comparison))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    public static ulong TryParseUInt64(this KVCollectionValue collectionValue, string name, ulong defaultValue = default, StringComparison comparison = StringComparison.Ordinal)
    {
        if (collectionValue.TryParse(name, out ulong result, comparison))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    public static DateTime TryParseDateTimeS(this KVCollectionValue collectionValue, string name, DateTime defaultValue = default, StringComparison comparison = StringComparison.Ordinal)
    {
        if (collectionValue.TryParse(name, out long result, comparison))
        {
            return DateTimeOffset.FromUnixTimeSeconds(result).LocalDateTime;
        }
        else
        {
            return defaultValue;
        }
    }
}