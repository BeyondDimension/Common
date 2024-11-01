// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace System;

/// <summary>
/// https://github.com/dotnet/runtime/blob/v8.0.0-rc.2.23479.6/src/libraries/Common/src/Extensions/TypeNameHelper/TypeNameHelper.cs
/// </summary>
public static partial class TypeNameHelper
{
    /// <summary>
    /// 默认的嵌套类型分隔符
    /// </summary>
    const char DefaultNestedTypeDelimiter = '+';

    /// <summary>
    /// 内置输入类型字典
    /// </summary>
    static readonly Dictionary<Type, string> _builtInTypeNames = new()
    {
        { typeof(void), "void" },
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(object), "object" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "short" },
        { typeof(string), "string" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(ushort), "ushort" }
    };

    /// <summary>
    /// 获取对象的类型名称
    /// </summary>
    [return: NotNullIfNotNull(nameof(item))]
    public static string? GetTypeDisplayName(object? item, bool fullName = true)
    {
        return item == null ? null : GetTypeDisplayName(item.GetType(), fullName);
    }

    /// <summary>
    /// Pretty print a type name.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="fullName"><c>true</c> to print a fully qualified name.</param>
    /// <param name="includeGenericParameterNames"><c>true</c> to include generic parameter names.</param>
    /// <param name="includeGenericParameters"><c>true</c> to include generic parameters.</param>
    /// <param name="nestedTypeDelimiter">Character to use as a delimiter in nested type names</param>
    /// <returns>The pretty printed type name.</returns>
    public static string GetTypeDisplayName(Type type, bool fullName = true, bool includeGenericParameterNames = false, bool includeGenericParameters = true, char nestedTypeDelimiter = DefaultNestedTypeDelimiter)
    {
        StringBuilder? builder = null;
        string? name = ProcessType(ref builder, type, new DisplayNameOptions(fullName, includeGenericParameterNames, includeGenericParameters, nestedTypeDelimiter));
        return name ?? builder?.ToString() ?? string.Empty;
    }

    static string? ProcessType(ref StringBuilder? builder, Type type, in DisplayNameOptions options)
    {
        if (type.IsGenericType)
        {
            Type[] genericArguments = type.GetGenericArguments();
            builder ??= new StringBuilder();
            ProcessGenericType(builder, type, genericArguments, genericArguments.Length, options);
        }
        else if (type.IsArray)
        {
            builder ??= new StringBuilder();
            ProcessArrayType(builder, type, options);
        }
        else if (_builtInTypeNames.TryGetValue(type, out string? builtInName))
        {
            if (builder is null) return builtInName;

            builder.Append(builtInName);
        }
        else if (type.IsGenericParameter)
        {
            if (options.IncludeGenericParameterNames)
            {
                if (builder is null) return type.Name;

                builder.Append(type.Name);
            }
        }
        else
        {
            string name = options.FullName ? type.FullName! : type.Name;

            if (builder is null)
            {
                if (options.NestedTypeDelimiter != DefaultNestedTypeDelimiter)
                {
                    return name.Replace(DefaultNestedTypeDelimiter, options.NestedTypeDelimiter);
                }

                return name;
            }

            builder.Append(name);
            if (options.NestedTypeDelimiter != DefaultNestedTypeDelimiter)
            {
                builder.Replace(DefaultNestedTypeDelimiter, options.NestedTypeDelimiter, builder.Length - name.Length, name.Length);
            }
        }

        return null;
    }

    static void ProcessArrayType(StringBuilder builder, Type type, in DisplayNameOptions options)
    {
        Type innerType = type;
        while (innerType.IsArray)
        {
            innerType = innerType.GetElementType()!;
        }

        ProcessType(ref builder!, innerType, options);

        while (type.IsArray)
        {
            builder.Append('[');
            builder.Append(',', type.GetArrayRank() - 1);
            builder.Append(']');
            type = type.GetElementType()!;
        }
    }

    static void ProcessGenericType(StringBuilder builder, Type type, Type[] genericArguments, int length, in DisplayNameOptions options)
    {
        int offset = 0;
        if (type.IsNested)
        {
            offset = type.DeclaringType!.GetGenericArguments().Length;
        }

        if (options.FullName)
        {
            if (type.IsNested)
            {
                ProcessGenericType(builder, type.DeclaringType!, genericArguments, offset, options);
                builder.Append(options.NestedTypeDelimiter);
            }
            else if (!string.IsNullOrEmpty(type.Namespace))
            {
                builder.Append(type.Namespace);
                builder.Append('.');
            }
        }

        int genericPartIndex = type.Name.IndexOf('`');
        if (genericPartIndex <= 0)
        {
            builder.Append(type.Name);
            return;
        }

        builder.Append(type.Name, 0, genericPartIndex);

        if (options.IncludeGenericParameters)
        {
            builder.Append('<');
            for (int i = offset; i < length; i++)
            {
                ProcessType(ref builder!, genericArguments[i], options);
                if (i + 1 == length)
                {
                    continue;
                }

                builder.Append(',');
                if (options.IncludeGenericParameterNames || !genericArguments[i + 1].IsGenericParameter)
                {
                    builder.Append(' ');
                }
            }
            builder.Append('>');
        }
    }

    readonly struct DisplayNameOptions(bool fullName, bool includeGenericParameterNames, bool includeGenericParameters, char nestedTypeDelimiter)
    {
        public bool FullName { get; } = fullName;

        public bool IncludeGenericParameters { get; } = includeGenericParameters;

        public bool IncludeGenericParameterNames { get; } = includeGenericParameterNames;

        public char NestedTypeDelimiter { get; } = nestedTypeDelimiter;
    }
}