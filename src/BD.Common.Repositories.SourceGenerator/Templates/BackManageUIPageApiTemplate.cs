namespace BD.Common.Repositories.SourceGenerator.Templates;

/// <summary>
/// 后台管理前台页面 API 源码生成模板
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
// @ts-ignore
/* eslint-disable */
import { request } from '@umijs/max';
import config from '@/utils/defaultSettings'
import { getUrl } from '@/utils/index'

"""u8;
        stream.Write(utf8String);
        var routePrefix = metadata.ApiRoutePrefix;
        byte[] routePrefixU8;
        if (string.IsNullOrWhiteSpace(routePrefix))
        {
            routePrefixU8 = "bm/"u8.ToArray();
        }
        else
        {
            routePrefix = routePrefix!.TrimStart("/");
            if (!routePrefix.StartsWith("/"))
            {
                routePrefix = $"/{routePrefix}";
            }
            routePrefixU8 = Encoding.UTF8.GetBytes(routePrefix);
        }
        var classNamePluralize = metadata.ClassName.Pluralize();
        var classNamePluralizeLower = classNamePluralize.ToLowerInvariant();
        var classNamePluralizeLowerU8 = Encoding.UTF8.GetBytes(classNamePluralizeLower);
        if (metadata.BackManageCanTable)
        {
            WriteQuery(stream, metadata, routePrefixU8, classNamePluralizeLowerU8);
        }
        if (metadata.BackManageCanEdit)
        {
            WriteEditById(stream, metadata, routePrefixU8, classNamePluralizeLowerU8);
        }
        if (!metadata.BackManageEditModelReadOnly || metadata.BackManageCanAdd)
        {
            WriteSave(stream, metadata, routePrefixU8, classNamePluralizeLowerU8);
        }
        foreach (var field in fields)
        {
            switch (field.FixedProperty)
            {
                case FixedProperty.Disable:
                    WriteSetDisable(stream, metadata, routePrefixU8, classNamePluralizeLowerU8);
                    break;
                case FixedProperty.Title:
                    WriteGetSelect(stream, metadata, routePrefixU8, classNamePluralizeLowerU8);
                    break;
            }
        }

    }

    void WriteQuery(
        Stream stream,
        Metadata metadata,
        byte[] routePrefixU8,
        byte[] classNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/** 获取 {0} 查询列表 GET {1} */

"""u8;
        stream.WriteFormat(utf8String, metadata.Summary, routePrefixU8);
        utf8String =
"""
export async function {0}Query(data:any)
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<API.PageModel<API.{0}>>>(getUrl(config.ApiUrl + `{1}`, data),
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, routePrefixU8);
        stream.Write(
"""
{
    method: 'GET',    
  });
}

"""u8
);
    }

    void WriteEditById(
        Stream stream,
        Metadata metadata,
        byte[] routePrefixU8,
        byte[] classNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/** 根据主键获取编辑模型 GET {0} */

"""u8;
        stream.WriteFormat(utf8String, routePrefixU8);
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
  return request<API.BApiResponse<API.{0}>>(config.ApiUrl + `{1}/${id}`,
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, routePrefixU8);
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

    void WriteDelete(
       Stream stream,
       Metadata metadata,
       byte[] routePrefixU8,
       byte[] classNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/** 删除{0}记录 DELETE {1} */

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, routePrefixU8);
        utf8String =
"""
export async function {0}Delete(id: string)
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<number>>(config.ApiUrl + `{1}/${id}`,
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, routePrefixU8);
        stream.Write(
"""
{

"""u8
);
        stream.Write(
"""
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
       byte[] classNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;
        utf8String =
"""

/** 保存对 {0} 新增编辑操作 PUT|POST {1} */

"""u8;
        stream.WriteFormat(utf8String, metadata.Summary, routePrefixU8);
        utf8String =
"""
export async function {0}Save(put: boolean,id?: string, options?: { [key: string]: any })
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<number>>(config.ApiUrl + `{0}`,
"""u8;
        stream.WriteFormat(utf8String, routePrefixU8);
        stream.Write(
"""
{

"""u8
);
        stream.Write(
"""
    method: put ? 'PUT' : 'POST',
    id,
    ...(options || {}),
  });
}

"""u8
);
    }

    void WriteSetDisable(
       Stream stream,
       Metadata metadata,
       byte[] routePrefixU8,
       byte[] classNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/** 设置 {0} 的禁用状态 PUT {1} */

"""u8;
        stream.WriteFormat(utf8String, metadata.Summary, routePrefixU8);
        utf8String =
"""
export async function {0}Disable (id: string, disable:boolean)
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
{

"""u8
);
        utf8String =
"""
  return request<API.BApiResponse<number>>(config.ApiUrl + `{0}/disable/${id}/${disable}`,
"""u8;
        stream.WriteFormat(utf8String, routePrefixU8);
        stream.Write(
"""
{

"""u8
);
        stream.Write(
"""
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
       byte[] classNamePluralizeLower)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

/** 获取用于选择框/下拉列表(Select)的数据 GET {0} */

"""u8;
        stream.WriteFormat(utf8String, routePrefixU8);
        utf8String =
"""
export async function {0}Select ()
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
