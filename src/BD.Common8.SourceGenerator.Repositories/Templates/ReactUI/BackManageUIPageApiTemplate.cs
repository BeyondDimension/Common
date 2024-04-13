namespace BD.Common8.SourceGenerator.Repositories.Templates.ReactUI;

/// <summary>
/// 后台管理前端页面 API 源码生成模板
/// </summary>
sealed class BackManageUIPageApiTemplate : TemplateBase<BackManageUIPageApiTemplate, BackManageUIPageApiTemplate.Metadata>
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

        /// <inheritdoc cref="GenerateRepositoriesAttribute.ApiRouteIgnoreRedundantEntityPrefix"/>
        public bool ApiRouteIgnoreRedundantEntityPrefix => GenerateRepositoriesAttribute.ApiRouteIgnoreRedundantEntityPrefix;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanAdd"/>
        public bool BackManageCanAdd => GenerateRepositoriesAttribute.BackManageCanAdd;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanEdit"/>
        public bool BackManageCanEdit => GenerateRepositoriesAttribute.BackManageCanEdit;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanDelete"/>
        public bool BackManageCanDelete => GenerateRepositoriesAttribute.BackManageCanDelete;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditModelReadOnly"/>
        public bool BackManageEditModelReadOnly => GenerateRepositoriesAttribute.BackManageEditModelReadOnly;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanTable"/>
        public bool BackManageCanTable => GenerateRepositoriesAttribute.BackManageCanTable;
    }

    protected override void WriteCore(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        var routePrefix = metadata.ApiRoutePrefix;
        byte[] routePrefixU8;
        if (string.IsNullOrWhiteSpace(routePrefix))
        {
            routePrefixU8 = "bm/"u8.ToArray();
        }
        else
        {
            routePrefix = routePrefix!.TrimStart("/").ToLowerInvariant();
            if (!routePrefix.StartsWith("/"))
            {
                routePrefix = $"/{routePrefix}";
            }
            routePrefixU8 = Encoding.UTF8.GetBytes(routePrefix);
        }
        var routeNamePluralize = metadata.ClassName.ToLowerInvariant().Pluralize();
        // 去除冗余的路由前缀
        var routeEnd = Encoding.UTF8.GetString(routePrefixU8).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
        if (metadata.ApiRouteIgnoreRedundantEntityPrefix &&
            !metadata.ClassName.Equals(routeEnd, StringComparison.OrdinalIgnoreCase) && // 实体名和路由结尾一样的话，不能忽略
            metadata.ClassName.Titleize().Split(' ').First().Equals(routeEnd, StringComparison.OrdinalIgnoreCase)) // 实体名第一个单词和路由结尾一致
        {
            routeNamePluralize = routeNamePluralize.TrimStart(routeEnd, StringComparison.OrdinalIgnoreCase);
        }
        var routeNamePluralizeLower = routeNamePluralize.ToLowerInvariant();
        var routeNamePluralizeLowerU8 = Encoding.UTF8.GetBytes(routeNamePluralizeLower);

        stream.Write(
"""
// @ts-ignore
/* eslint-disable */
import { request } from '@umijs/max';
import config from '@/utils/defaultSettings'


"""u8);

        stream.WriteFormat(
"""
const baseURL = config.ApiUrl + `{0}/{1}`;

"""u8, routePrefixU8, routeNamePluralizeLower);

        if (metadata.BackManageCanTable)
        {
            WriteQuery(stream, metadata, routePrefixU8, routeNamePluralizeLowerU8);
        }
        if (metadata.BackManageCanEdit)
        {
            WriteEditById(stream, metadata, routePrefixU8, routeNamePluralizeLowerU8);
        }
        if (!metadata.BackManageEditModelReadOnly || metadata.BackManageCanAdd)
        {
            WriteSave(stream, metadata, routePrefixU8, routeNamePluralizeLowerU8);
        }
        if (metadata.BackManageCanDelete)
        {
            WriteDelete(stream, metadata, routePrefixU8, routeNamePluralizeLowerU8);
        }
        foreach (var field in fields)
        {
            switch (field.FixedProperty)
            {
                case FixedProperty.Disable:
                    WriteSetDisable(stream, metadata, routePrefixU8, routeNamePluralizeLowerU8);
                    break;
                case FixedProperty.Title:
                    WriteGetSelect(stream, metadata, routePrefixU8, routeNamePluralizeLowerU8);
                    break;
            }
        }
    }

    void WriteQuery(
        Stream stream,
        Metadata metadata,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/**
 * 获取 {0} 分页列表
 * @param params 查询参数
 */

"""u8;
        stream.WriteFormat(utf8String, metadata.Summary, routePrefixU8, routeNamePluralizeLower);
        utf8String =
"""
export async function {0}Query(params: any) 
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<API.PageModel<API.{0}>>>('', 
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, routePrefixU8, routeNamePluralizeLower);
        stream.Write(
"""
{
    baseURL: baseURL,
    method: 'GET',   
    params: params 
  });
}

"""u8
);
    }

    void WriteEditById(
        Stream stream,
        Metadata metadata,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/**
 * 根据主键获取编辑模型
 * @param id 主键
 * @url GET {0}/{1}
 */

"""u8;
        stream.WriteFormat(utf8String, routePrefixU8, routeNamePluralizeLower);
        utf8String =
"""
export async function {0}EditById(id: string) 
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<API.{0}>>(id, 
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, routePrefixU8, routeNamePluralizeLower);
        stream.Write(
"""
{

"""u8
);
        stream.Write(
"""
    baseURL: baseURL,
    method: 'GET'
  });
}

"""u8
);
    }

    void WriteDelete(
       Stream stream,
       Metadata metadata,
       byte[] routePrefixU8,
       byte[] routeNamePluralizeLower
       )
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/**
 * 删除 {0} 记录
 * @param id 主键
 * @url DELETE {1}/{2}
 */

"""u8;
        stream.WriteFormat(utf8String, metadata.Summary, routePrefixU8, routeNamePluralizeLower);
        utf8String =
"""
export async function {0}(id: string) 
"""u8;
        stream.WriteFormat(utf8String, $"{metadata.ClassName}Delete");
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<null>>(id, 
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, routePrefixU8, routeNamePluralizeLower);
        stream.Write(
"""
{

"""u8
);
        stream.Write(
"""
    baseURL: baseURL,
    method: 'DELETE'
  });
}

"""u8
);
    }

    void WriteSave(
       Stream stream,
       Metadata metadata,
       byte[] routePrefixU8,
       byte[] routeNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;
        utf8String =
"""

/**
 * 保存对 {0} 新增/编辑操作
 * @param id 主键
 * @param options 请求配置
 * @url PUT|POST {1}/{2}
 */

"""u8;
        stream.WriteFormat(utf8String, metadata.Summary, routePrefixU8, routeNamePluralizeLower);
        utf8String =
"""
export async function {0}Save(id?: string, options?: { [key: string]: any }) 
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, routeNamePluralizeLower);
        stream.Write(
"""
{
  if (id) {
"""u8
);

        utf8String =
"""

    return request<API.BApiResponse<null>>(id, {

"""u8;
        stream.WriteFormat(utf8String, routePrefixU8, routeNamePluralizeLower);
        stream.Write(
"""
      baseURL: baseURL,
      method: 'PUT',
      ...(options || {}),
    });
  }
  else {

"""u8);
        utf8String =
"""
    return request<API.BApiResponse<null>>('', 
"""u8;
        stream.WriteFormat(utf8String, routePrefixU8, routeNamePluralizeLower);
        stream.Write(
"""
{

"""u8);
        stream.Write(
"""
      baseURL: baseURL,
      method: 'POST',
      ...(options || {}),
    });
  }
}

"""u8);
    }

    void WriteSetDisable(
       Stream stream,
       Metadata metadata,
       byte[] routePrefixU8,
       byte[] routeNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/**
 * 设置 {0} 的禁用状态
 * @param id 主键
 * @param disable 设置状态
 */

"""u8;
        stream.WriteFormat(utf8String, metadata.Summary, routePrefixU8);
        utf8String =
"""
export async function {0}Disable(id: string, disable: boolean) 
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<null>>(`disable/${id}/${disable}`, 
"""u8;
        stream.WriteFormat(utf8String, routePrefixU8, routeNamePluralizeLower);
        stream.Write(
"""
{

"""u8
);
        stream.Write(
"""
    baseURL: baseURL,
    method: 'PUT'
  });
}

"""u8
);
    }

    void WriteGetSelect(
       Stream stream,
       Metadata metadata,
       byte[] routePrefixU8,
       byte[] routeNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/** 获取用于选择框/下拉列表(Select)的数据 GET {0} */

"""u8;
        stream.WriteFormat(utf8String, routePrefixU8);
        utf8String =
"""
export async function {0}Select() 
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<any[]>>(config.ApiUrl + `{0}`, 
"""u8;
        stream.WriteFormat(utf8String, routePrefixU8);
        stream.Write(
"""
{

"""u8
);
        stream.Write(
"""
    method: 'GET'
  });
}

"""u8
);
    }
}
