namespace BD.Common8.SourceGenerator.Templates.Abstractions;

/// <summary>
/// 模板基类
/// </summary>
public abstract class TemplateBase
{
    /// <summary>
    /// 写入文件头
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    protected static void WriteFileHeader(
        Stream stream)
    {
        stream.Write(
"""
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable IDE1006 // 命名样式
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name
#pragma warning disable SA1600 // Elements should be documented

"""u8);
    }

    /// <summary>
    /// 写入命名空间
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="namespace"></param>
    /// <param name="cancellationToken"></param>
    protected static void WriteNamespace(
        Stream stream,
        string @namespace)
    {
        stream.WriteFormat(
"""
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
﻿namespace {0};

"""u8, @namespace);
    }

    static readonly Lazy<string> mFileVersion = new(() =>
    {
        var assembly = typeof(TemplateBase).Assembly;
        var fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        return fileVersion ?? string.Empty;
    });

    /// <summary>
    /// 获取当前源生成器的文件版本
    /// </summary>
    protected static string FileVersion => mFileVersion.Value;

    static Random? random;

    /// <inheritdoc cref="System.Random"/>
    protected static Random Random => random ??= new(Guid.NewGuid().GetHashCode());

    /// <summary>
    /// 获取随机字段名
    /// </summary>
    /// <returns></returns>
    protected static string GetRandomFieldName()
    {
        var fieldName = "k__BackingField".ToCharArray();
        for (int i = 0; i < fieldName.Length / 2; i++)
        {
            const string random_chars = "_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var index = Random.Next(fieldName.Length);
            fieldName[index] = random_chars[Random.Next(random_chars.Length)];
        }
        return new(fieldName);
    }
}
