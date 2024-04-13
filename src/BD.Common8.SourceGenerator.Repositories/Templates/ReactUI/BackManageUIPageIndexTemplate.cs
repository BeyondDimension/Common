namespace BD.Common8.SourceGenerator.Repositories.Templates.ReactUI;

/// <summary>
/// 后台管理前端页面 Index 源码生成模板
/// </summary>
sealed class BackManageUIPageIndexTemplate : TemplateBase<BackManageUIPageIndexTemplate, BackManageUIPageIndexTemplate.Metadata>
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
import * as api from './api';
export default {
  api,
};

"""u8;
        stream.Write(utf8String);
    }
}
