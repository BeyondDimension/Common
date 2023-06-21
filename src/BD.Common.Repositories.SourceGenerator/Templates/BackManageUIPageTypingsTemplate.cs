using Microsoft.CodeAnalysis.Operations;
using System;
using System.IO;
using System.Security.Principal;

namespace BD.Common.Repositories.SourceGenerator.Templates;

/// <summary>
/// 后台管理前台页面 Typings 源码生成模板
/// </summary>
sealed class BackManageUIPageTypingsTemplate : TemplateBase<BackManageUIPageTypingsTemplate, BackManageUIPageTypingsTemplate.Metadata>
{
    public readonly record struct Metadata(
     string Namespace,
     string Summary,
     string ClassName,
     string? PrimaryKeyTypeName = null,
     GenerateRepositoriesAttribute GenerateRepositoriesAttribute = null!) : ITemplateMetadata
    {
        /// <inheritdoc cref="GenerateRepositoriesAttribute.ApiRoutePrefix"/>
        public string? ApiRoutePrefix => GenerateRepositoriesAttribute.ApiRoutePrefix;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanAdd"/>
        public bool BackManageCanAdd => GenerateRepositoriesAttribute.BackManageCanAdd;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanEdit"/>
        public bool BackManageCanEdit => GenerateRepositoriesAttribute.BackManageCanEdit;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditModelReadOnly"/>
        public bool BackManageEditModelReadOnly => GenerateRepositoriesAttribute.BackManageEditModelReadOnly;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanTable"/>
        public bool BackManageCanTable => GenerateRepositoriesAttribute.BackManageCanTable;
    }
    protected override void WriteCore(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
declare namespace API {

"""u8;
        stream.Write(utf8String);
        utf8String =
"""
  /** {0} */

"""u8;
        stream.WriteFormat(utf8String, metadata.Summary);
        utf8String =
"""
  type {0} =
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        utf8String =
"""
 {

"""u8;
        stream.Write(utf8String);
        foreach (var field in fields)
        {
            if (field.PropertyType == nameof(PreprocessorDirective))
                continue;

            var camelizeName = field.CamelizeName;
            switch (camelizeName)
            {
                case "createUserId":
                    camelizeName = "createUser";
                    break;
                case "operatorUserId":
                    camelizeName = "operatorUser";
                    break;
            }

            utf8String =
"""
    /** {0} */

"""u8;
            stream.WriteFormat(utf8String, field.HumanizeName);
            utf8String =
"""
    {0}: {1};

"""u8;
            stream.WriteFormat(utf8String, camelizeName, GetType(field.PropertyType));
        }

        utf8String =
"""
    }
}
"""u8;
        stream.Write(utf8String);
    }

    string GetType(string propertyType)
    {
        string type = "any";
        switch (propertyType.Replace('?', ' ').Trim())
        {
            case "Guid":
                type = "string";
                break;
            case "string":
                type = "string";
                break;
            case "bool":
                type = "boolean";
                break;
            case "int":
                type = "number";
                break;
            case "decimal":
                type = "number";
                break;
            case "double":
                type = "number";
                break;
            case "float":
                type = "number";
                break;
            case "short":
                type = "number";
                break;
            case "ushort":
                type = "number";
                break;
            case "long":
                type = "number";
                break;
            case "ulong":
                type = "number";
                break;
            case "DateTimeOffset":
                type = "Date";
                break;
            case "TimeSpan":
                type = "Date";
                break;
        }
        return type;
    }
}
