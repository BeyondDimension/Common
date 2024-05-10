namespace System.Text.Json.Serialization;

/// <summary>
/// <see cref="ValueTuple{T1, T2, T3, T4, T5, T6, T7, TRest}"/> Json 转换器，与 <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, TRest}"/> 行为一致
/// </summary>
public sealed class ValueTupleConverter : JsonConverter<ITuple>
{
    static readonly ImmutableDictionary<Type, int> lengthCache;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Type DAMAllType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type) => type;

    static ValueTupleConverter()
    {
        var lengthCache_ = new Dictionary<Type, int>();
        lengthCache_.Add(DAMAllType(typeof(ValueTuple<>)), 1);
        lengthCache_.Add(DAMAllType(typeof(ValueTuple<,>)), 2);
        lengthCache_.Add(DAMAllType(typeof(ValueTuple<,,>)), 3);
        lengthCache_.Add(DAMAllType(typeof(ValueTuple<,,,>)), 4);
        lengthCache_.Add(DAMAllType(typeof(ValueTuple<,,,,>)), 5);
        lengthCache_.Add(DAMAllType(typeof(ValueTuple<,,,,,>)), 6);
        lengthCache_.Add(DAMAllType(typeof(ValueTuple<,,,,,,>)), 7);
        lengthCache_.Add(DAMAllType(typeof(ValueTuple<,,,,,,,>)), 8);
        lengthCache = lengthCache_.ToImmutableDictionary();
    }

    /// <inheritdoc/>
    public sealed override bool CanConvert(Type typeToConvert) => IsValueTuple(typeToConvert);

    static bool IsValueTuple(Type type)
    {
        return type == typeof(ValueTuple) ||
            (type.IsValueType &&
            type.IsGenericType &&
            !type.IsGenericTypeDefinition &&
             typeof(ITuple).IsAssignableFrom(type));
    }

    static Type GetGenericArguments(Type tupleType, int index)
    {
        var genericArguments = tupleType.GetGenericArguments();
        return index switch
        {
            0 or 1 or 2 or 3 or 4 or 5 or 6 => genericArguments[index],
            7 => genericArguments[7],
            _ => GetGenericArguments(genericArguments[7], index - 7),
        };
    }

    static int GetLength(Type tupleType)
    {
        var genericTypeDefinition = tupleType.GetGenericTypeDefinition();
        if (lengthCache.TryGetValue(genericTypeDefinition, out var length))
        {
            if (length == 8)
            {
                var typeRest = tupleType.GetGenericArguments()[7];
                return 7 + (IsValueTuple(typeRest) ? GetLength(typeRest) : 0);
            }
            return length;
        }
        return 0;
    }

    static object? GetItem(ITuple tuple, int index, bool getRest = true)
    {
        if (getRest && index == 7)
        {
            var tupleType = tuple.GetType();
            return tupleType.GetField(tupleType.IsValueType ? "Rest" : "m_Rest", BindingFlags.Public | BindingFlags.Instance)?.GetValue(tuple);
        }
        else if (index < 8)
            return tuple[index];
        else if (tuple[7] is ITuple tuple7)
            return GetItem(tuple7, index - 7, getRest);
        return null;
    }

    /// <inheritdoc/>
    public sealed override void Write(Utf8JsonWriter writer, ITuple value, SystemTextJsonSerializerOptions options)
    {
        if (value is null || value is ValueTuple)
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
            return;
        }

        writer.WriteStartObject();
        int numWriteEndObject = 0;
        for (int i = 0; i < value.Length; i++)
        {
            var isRest = i != 0 && i % 7 == 0;
            var item = GetItem(value, i, false);
            string propertyName;
            if (i == 0)
            {
                propertyName = "Item1";
            }
            else
            {
                var num = ((i + 1) % 8) + (numWriteEndObject == 0 ? 0 : 1);
                propertyName = num switch
                {
                    1 => "Item1",
                    2 => "Item2",
                    3 => "Item3",
                    4 => "Item4",
                    5 => "Item5",
                    6 => "Item6",
                    7 => "Item7",
                    0 or 8 => "Rest",
                    _ => $"Item{num}",
                };
            }
            propertyName = options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
            writer.WritePropertyName(propertyName);
            if (item is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                var itemType = item.GetType();
                if (isRest)
                {
                    writer.WriteStartObject();
                    numWriteEndObject++;
                    propertyName = "Item1";
                    propertyName = options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
                    writer.WritePropertyName(propertyName);
                }
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
                SystemTextJsonSerializer.Serialize(writer, item, itemType, options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            }
        }
        writer.WriteEndObject();
        for (int i = 0; i < numWriteEndObject; i++)
        {
            writer.WriteEndObject();
        }
    }

    /// <inheritdoc/>
    public sealed override ITuple Read(ref Utf8JsonReader reader, Type typeToConvert, SystemTextJsonSerializerOptions options)
    {
        if (!reader.Read())
        {
            goto return_default;
        }

        if (reader.TokenType == JsonTokenType.None ||
            reader.TokenType == JsonTokenType.Null ||
            reader.TokenType == JsonTokenType.EndObject ||
            reader.TokenType == JsonTokenType.EndArray ||
            reader.TokenType == JsonTokenType.True ||
            reader.TokenType == JsonTokenType.False)
        {
            goto return_default;
        }

        var length = GetLength(typeToConvert);
        if (length <= 0)
            return default(ValueTuple);

        object?[] items = new object[length];

        int numRest = 0;
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            int index;
            Type? itemType;
            bool isRest = false;
            if ((reader.ValueTextEquals("Item1"u8) || reader.ValueTextEquals("item1"u8)) && reader.Read())
            {
                index = 0 + (numRest * 8);
                itemType = GetGenericArguments(typeToConvert, index);
            }
            else if ((reader.ValueTextEquals("Item2"u8) || reader.ValueTextEquals("item2"u8)) && reader.Read())
            {
                index = 1 + (numRest * 8);
                itemType = GetGenericArguments(typeToConvert, index);
            }
            else if ((reader.ValueTextEquals("Item3"u8) || reader.ValueTextEquals("item3"u8)) && reader.Read())
            {
                index = 2 + (numRest * 8);
                itemType = GetGenericArguments(typeToConvert, index);
            }
            else if ((reader.ValueTextEquals("Item4"u8) || reader.ValueTextEquals("item4"u8)) && reader.Read())
            {
                index = 3 + (numRest * 8);
                itemType = GetGenericArguments(typeToConvert, index);
            }
            else if ((reader.ValueTextEquals("Item5"u8) || reader.ValueTextEquals("item5"u8)) && reader.Read())
            {
                index = 4 + (numRest * 8);
                itemType = GetGenericArguments(typeToConvert, index);
            }
            else if ((reader.ValueTextEquals("Item6"u8) || reader.ValueTextEquals("item6"u8)) && reader.Read())
            {
                index = 5 + (numRest * 8);
                itemType = GetGenericArguments(typeToConvert, index);
            }
            else if ((reader.ValueTextEquals("Item7"u8) || reader.ValueTextEquals("item7"u8)) && reader.Read())
            {
                index = 6 + (numRest * 8);
                itemType = GetGenericArguments(typeToConvert, index);
            }
            else if (reader.ValueTextEquals("Rest"u8) || reader.ValueTextEquals("rest"u8))
            {
                isRest = true;
                index = 7 + (numRest * 8);
                itemType = GetGenericArguments(typeToConvert, index);
                if (IsValueTuple(itemType))
                {
                    reader.Read();
                    numRest++;
                }
            }
            else
            {
                if (!reader.Read())
                {
                    break;
                }
                if (!reader.Read())
                {
                    break;
                }
                continue;
            }

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            if (isRest)
            {
                items[index] = Read(ref reader, itemType!, options);
            }
            else
            {
                items[index] = SystemTextJsonSerializer.Deserialize(ref reader, itemType!, options);
            }
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            reader.Read();
        }

        if (items.All(x => x is null))
        {
            goto return_default;
        }

#pragma warning disable IL2072 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
#pragma warning disable IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
        if (length <= 8)
        {
            return (ITuple)Activator.CreateInstance(typeToConvert, items)!;
        }
        else
        {
            for (int i = (length / 8) - 1; i >= 0; i--)
            {
                var index = (i + 1) * 8;
                var restLength = length - index;
                if (restLength > 8) restLength = 8;
                var itemType = GetGenericArguments(typeToConvert, index);
                object?[] restItems = new object[restLength];
                for (int j = 0; j < restLength; j++)
                {
                    restItems[0] = items[index + 1 + j];
                }
                items[index + 1] = Activator.CreateInstance(itemType, restItems)!;
            }
            return (ITuple)Activator.CreateInstance(typeToConvert, items.Take(8).ToArray())!;
        }

    return_default:
        return typeToConvert.IsValueType ? (ITuple)Activator.CreateInstance(typeToConvert)! : null!;
#pragma warning restore IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
#pragma warning restore IL2072 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
    }
}
