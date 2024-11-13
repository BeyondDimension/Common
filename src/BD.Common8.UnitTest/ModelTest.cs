using PB = Google.Protobuf;

namespace BD.Common8.UnitTest;

#pragma warning disable SA1414 // Tuple types in signatures should have element names

/// <summary>
/// 提供对模型类的测试
/// </summary>
public sealed class ModelTest
{
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    static extern PB::ByteString ToByteString(ReadOnlyMemory<byte> bytes);

    [Test]
    public void Protobuf_UnsafeAccessor()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        var byteString = ToByteString(bytes);
        TestContext.WriteLine(byteString.ToBase64());
    }

    readonly SystemTextJsonSerializerOptions o = new(SystemTextJsonSerializerOptions.Default)
    {
        AllowTrailingCommas = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    readonly SystemTextJsonSerializerOptions o_ValueTuple_I = new(SystemTextJsonSerializerOptions.Default)
    {
        AllowTrailingCommas = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new ValueTupleConverter() },
    };

    readonly SystemTextJsonSerializerOptions o_ValueTuple = new(SystemTextJsonSerializerOptions.Default)
    {
        AllowTrailingCommas = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        Converters = { new ValueTupleConverter() },
    };

    readonly SystemTextJsonSerializerOptions o_ValueTuple_CamelCase = new(SystemTextJsonSerializerOptions.Default)
    {
        AllowTrailingCommas = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        Converters = { new ValueTupleConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    static int GetLength(ITuple tuple)
    {
        if (tuple.Length == 8)
        {
            if (tuple[7] is ITuple tuple7)
            {
                return 7 + GetLength(tuple7);
            }
            return 8;
        }
        return tuple.Length;
    }

    static KeyValuePair<string, (Type, JsonSerializerOptions)> Serialize(object? value, Type inputType, JsonSerializerOptions options)
    {
        var json = SystemTextJsonSerializer.Serialize(value, inputType, options);
        return new KeyValuePair<string, (Type, JsonSerializerOptions)>(json, (inputType, options));
    }

    [Test]
    public void ValueTuple()
    {
        ValueTuple empty = default;
        TestContext.WriteLine(Serialize(empty, empty.GetType(), o).Key);
        TestContext.WriteLine(Serialize(empty, empty.GetType(), o_ValueTuple_I).Key);

        ITuple[] tuples = [
            new ValueTuple<string>("aaa"),
            ("Hello", 123, true),
            (1, 2, 3, 4, 5, 6, 7, 8),
            (1, 2, 3, 4, 5, 6, 7, (8, 9, 10, 11, 12, 13, 14, 15)),
            (1, 2, 3, 4, 5, 6, 7, (8, 9, 10, 11, 12, 13, 14, (15, 16))),
            (1, 2, 3, 4, 5, 6, 7, (8, 9, 10, 11, 12, 13, 14, (15, (16, 17)))),
            // --------------------- 测试 Tuple 与 ValueTuple 序列化一致 ---------------------
            // https://github.com/dotnet/runtime/issues/1519
            Tuple.Create("aaa"),
            Tuple.Create("Hello", 123, true),
            Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8),
            Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11, 12, 13, 14, 15)),
            Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11, 12, 13, 14, Tuple.Create(15, 16))),
            Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, 9, 10, 11, 12, 13, 14, Tuple.Create(15, Tuple.Create(16, 17)))),
        ];
        var tuple_strings = new List<string>();
        foreach (var tuple in tuples)
        {
            TestContext.WriteLine($"Length: {GetLength(tuple)}");
            KeyValuePair<string, (Type, JsonSerializerOptions)>[] items = [
                Serialize(tuple, tuple.GetType(), o),
                Serialize(tuple, tuple.GetType(), o_ValueTuple_I),
                Serialize(tuple, tuple.GetType(), o_ValueTuple),
                Serialize(tuple, tuple.GetType(), o_ValueTuple_CamelCase),
            ];
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (i != 0)
                {
                    tuple_strings.Add(item.Key);
                }

                if (i == 0 && tuple.GetType().IsValueType)
                {
                    Assert.That(item.Key, Is.EqualTo("{}"));
                }
            }
            TestContext.WriteLine();

            foreach (var item in items)
            {
                if (item.Value.Item2 != o)
                {
                    bool equals = false;
                    try
                    {
                        var obj = SystemTextJsonSerializer.Deserialize(item.Key, item.Value.Item1, item.Value.Item2);
                        equals = tuple.Equals(obj);
                    }
                    catch (Exception ex)
                    {
                        throw new AggregateException(ex, new ApplicationException(item.Key));
                    }
                    Assert.That(equals, Is.True);
                }

                TestContext.WriteLine(item.Key);
            }
        }

        for (int i = 0; i < tuple_strings.Count / 2; i++)
        {
            Assert.That(i + '|' + tuple_strings[i], Is.EqualTo(i + '|' + tuple_strings[i + (tuple_strings.Count / 2)]));
        }
    }

    #region https://github.com/dotnet/runtime/issues/89113

    static DefaultJsonTypeInfoResolver GetDefaultJsonTypeInfoResolver()
    {
        try
        {
            return GetDefaultJsonTypeInfoResolver_V9();
        }
        catch (MissingMethodException)
        {
        }
        return GetDefaultJsonTypeInfoResolver_V8();
    }

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v8.0.11/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Metadata/DefaultJsonTypeInfoResolver.cs#L135
    /// </summary>
    /// <param name="thiz"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "RootDefaultInstance")]
    static extern DefaultJsonTypeInfoResolver GetDefaultJsonTypeInfoResolver_V8(DefaultJsonTypeInfoResolver? @thiz = null);

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v9.0.0/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Metadata/DefaultJsonTypeInfoResolver.cs#L127
    /// </summary>
    /// <param name="thiz"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "get_DefaultInstance")]
    static extern DefaultJsonTypeInfoResolver GetDefaultJsonTypeInfoResolver_V9(DefaultJsonTypeInfoResolver? @thiz = null);

    #endregion

    [Test]
    public void RootDefaultInstance()
    {
        IList<IJsonTypeInfoResolver> typeInfoResolverChain = new List<IJsonTypeInfoResolver>();
        typeInfoResolverChain.Add(GetDefaultJsonTypeInfoResolver());

        var anyDefaultJsonTypeInfoResolver = typeInfoResolverChain.OfType<DefaultJsonTypeInfoResolver>().Any();

        Assert.That(anyDefaultJsonTypeInfoResolver, Is.True);
    }
}
