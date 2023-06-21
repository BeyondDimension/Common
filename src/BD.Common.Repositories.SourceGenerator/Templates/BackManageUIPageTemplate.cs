using System;

namespace BD.Common.Repositories.SourceGenerator.Templates;

/// <summary>
/// 后台管理前台页面源码生成模板
/// </summary>
sealed class BackManageUIPageTemplate : TemplateBase<BackManageUIPageTemplate, BackManageUIPageTemplate.Metadata>
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
        WriteImport(stream, metadata, fields, args);
        WriteConstMethod(stream, metadata, fields, args);
        WriteTableColums(stream, metadata, fields, args);
        stream.Write(
"""

  return (
    <>

"""u8);
        WriteModalForm(stream, metadata, fields, args);
        WriteProTable(stream, metadata, fields, args);
        stream.Write(
"""
    </>
  );
}
export default Manage;

"""u8);
    }

    void WriteImport(
        Stream stream,
        Metadata metadata,
        ImmutableArray<PropertyMetadata> fields,
        object?[] args)
    {
        ReadOnlySpan<byte> utf8String;
        utf8String =
"""
import { useEffect, useState, useRef } from 'react';
import { ProTable, ModalForm, ProFormText, ProFormDateTimePicker, ProFormDigit, ProFormSwitch} from '@ant-design/pro-components'
import type { ActionType, ProColumns, ProFormInstance } from '@ant-design/pro-components';
import AccessPage from '@/components/AccessPage'
import { PlusOutlined,ExclamationCircleFilled } from '@ant-design/icons';
import { Button, message,Space,Modal} from 'antd';
import { useModel, useAccess } from '@umijs/max'

"""u8;
        stream.Write(utf8String);
        utf8String =
"""
import {
"""u8;
        stream.Write(utf8String);
        List<string> ImportMethod = new List<string>();

        if (metadata.BackManageCanTable)
        {
            ImportMethod.Add(string.Concat(metadata.ClassName, "Query"));
        }
        if (metadata.BackManageCanEdit)
        {
            ImportMethod.Add(string.Concat(metadata.ClassName, "EditById"));
        }
        if (!metadata.BackManageEditModelReadOnly || metadata.BackManageCanAdd)
        {
            ImportMethod.Add(string.Concat(metadata.ClassName, "Save"));
        }
        var isSoft = false;
        foreach (var field in fields)
        {
            switch (field.FixedProperty)
            {
                case FixedProperty.Disable:
                    ImportMethod.Add(string.Concat(metadata.ClassName, "Disable"));
                    break;
                case FixedProperty.Title:
                    ImportMethod.Add(string.Concat(metadata.ClassName, "Select"));
                    break;
                case FixedProperty.SoftDeleted:
                    ImportMethod.Add(string.Concat(metadata.ClassName, "SoftDelete"));
                    isSoft = true;
                    break;
            }
        }
        if (!isSoft)
        {
            ImportMethod.Add(string.Concat(metadata.ClassName, "Delete"));
        }
        utf8String =
"""
{0} 
"""u8;
        var method = string.Join(",", ImportMethod);
        stream.WriteFormat(utf8String, method);
        stream.Write(
"""
}
"""u8);
        utf8String =
"""
 from '@/services/Basics/{0}/api'

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
    }

    void WriteConstMethod(
    Stream stream,
    Metadata metadata,
    ImmutableArray<PropertyMetadata> fields,
    object?[] args)
    {
        ReadOnlySpan<byte> utf8String;
        var isSoft = false;
        var disable = false;
        utf8String =
"""

const Manage: React.FC = () => {
      const access = useAccess() as API.RoleData;
      const { tablePagination } = useModel('bodySize');
      const { confirm } = Modal;

"""u8;
        stream.Write(utf8String);
        if (!metadata.BackManageEditModelReadOnly || metadata.BackManageCanAdd)
        {
            utf8String =
"""
      const [editInfo, setEditInfo] = useState<API.{0} | null>(null)
      const [editModel, setEditModel] = useState(false);
      const actionRef = useRef<ActionType>();
      const formRef = useRef<ProFormInstance>();

"""u8;
            stream.WriteFormat(utf8String, metadata.ClassName);
        }
        utf8String =
"""

  useEffect(() => {
  }, [])

"""u8;
        #region Query
        utf8String =
"""

 const GetTable = async (params: any, sort: any, filter: any): Promise<API.PageModel<API.{0}> | null> =>
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        utf8String =
"""
 {
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    const { 
"""u8;
        stream.Write(utf8String);
        List<string> constParams = new List<string>();
        foreach (var field in fields)
        {
            var camelizeName = field.CamelizeName;
            switch (camelizeName)
            {
                case "createUserId":
                    camelizeName = "createUser";
                    break;
                case "operatorUserId":
                    camelizeName = "operatorUser";
                    break;
                case "softDeleted":
                    isSoft = true;
                    break;
                case "disable":
                    disable = true;
                    break;
            }
            if (field.BackManageField == null || !field.BackManageField.Query)
                continue;
            constParams.Add(camelizeName);
        }
        utf8String =
"""
{0}
"""u8;
        stream.WriteFormat(utf8String, string.Join(",", constParams));
        utf8String =
"""
} = params;

"""u8;
        stream.Write(utf8String);
        stream.Write(
"""
    var query = {

"""u8);
        foreach (var Params in constParams)
        {
            utf8String =
"""
      {0},

"""u8;
            stream.WriteFormat(utf8String, Params);
        }
        stream.Write(
"""
    } as any;

"""u8);
        utf8String =
"""
    var data = await {0}Query(query);
    return data.data;
  }

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);

        #endregion

        #region onEdit
        if (!metadata.BackManageEditModelReadOnly || metadata.BackManageCanAdd)
        {
            utf8String =
"""

  const onEdit = (info: API.{0} | null = null) =>
"""u8;
            stream.WriteFormat(utf8String, metadata.ClassName);
            stream.Write(
"""
 {
"""u8);
            utf8String =
"""

    setEditModel(true);
    if (formRef && formRef?.current != null) {
      if (info == null) {
        formRef.current?.resetFields();
        setEditInfo(info);
      }
      else {
        LoadInfo(info);
      }
    }
  }

"""u8;
            stream.Write(utf8String);
            #region LoadInfo
            utf8String =
"""

  const LoadInfo = async (info: API.{0}) =>
"""u8;
            stream.WriteFormat(utf8String, metadata.ClassName);
            stream.Write(
"""
 {
"""u8);
            utf8String =
"""

      var data = {
        ...info,
      }
      formRef.current?.setFieldsValue(data);
      setEditInfo(info);
 }

"""u8;
            stream.Write(utf8String);
            #endregion

            #region onSaveFinish
            utf8String =
"""

   const onSaveFinish = async (values: any) => {
    var data = {
      ...values,
    };

"""u8;
            stream.Write(utf8String);
            utf8String =
"""
    var response = await {0}Save
"""u8;
            stream.WriteFormat(utf8String, metadata.ClassName);
            utf8String =
"""
(data?.id, { data: data });
"""u8;
            stream.Write(utf8String);
            utf8String =
"""

    if (response.isSuccess && response.data) {
      message.success('操作成功');
      if (formRef && formRef?.current != null) {
        formRef.current?.resetFields();
      }
      if (actionRef.current?.reset)
        actionRef.current?.reset();
    }
    else {
      message.error(response.messages);
    }
    return response.isSuccess && (response.data ?? false);
  }

"""u8;
            stream.Write(utf8String);
            #endregion

            #endregion
        }
        utf8String =
"""

  const showDeleteConfirm = (info: any, type: boolean) => {
    confirm({
      title: '提示',
      icon: <ExclamationCircleFilled />,
      content: '确认删除吗?',
      cancelText: '取消',
      okText: '确认',
      okType: 'danger',
      centered: true,
      okButtonProps: {
      },
      onOk() {
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

        {0}(info);

"""u8;
        stream.WriteFormat(utf8String, !isSoft ? "OnDelte" : "OnSoftDelete");
        utf8String =
"""
      },
    });
  };


"""u8;
        stream.Write(utf8String);
        utf8String =
"""
  const {0} = async (info: any): Promise<void> =>
"""u8;
        stream.WriteFormat(utf8String, !isSoft ? "OnDelte" : "OnSoftDelete");
        utf8String =
"""
 {

"""u8;
        stream.Write(utf8String);
        utf8String =
"""
    if (info != null) {
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

      var response = await {0}(info!.id);

"""u8;
        stream.WriteFormat(utf8String, !isSoft ? $"{metadata.ClassName}Delte" : $"{metadata.ClassName}SoftDelete");
        utf8String =
"""
      if (response.isSuccess) {
        message.success("删除完成");
        if (actionRef.current?.reset)
          actionRef.current?.reset();
      } else {
        message.error(response.messages);
      }
    }
  }

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        if (disable)
        {
            utf8String =
"""

 const SetUpDisable = async (e: CheckboxChangeEvent, id: any) => {

"""u8;
            stream.Write(utf8String);
            utf8String =
"""
 var response = await {0}Disable(id, e.target.checked);

"""u8;
            stream.WriteFormat(utf8String, metadata.ClassName);
            utf8String =
"""
    if (response.isSuccess) {
      message.success("操作成功");
      if (actionRef.current?.reset)
        actionRef.current?.reset();
    } else {
      message.error(response.messages);
    }
  };

"""u8;
            stream.Write(utf8String);
        }
    }
    void WriteTableColums(
    Stream stream,
    Metadata metadata,
    ImmutableArray<PropertyMetadata> fields,
    object?[] args)
    {

        ReadOnlySpan<byte> utf8String;
        utf8String =
"""

const operation = access?.{0}?.Edit || access?.{1}?.Delete;

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName, metadata.ClassName);
        utf8String =
"""
const columns: ProColumns<API.{0}>[] =[

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        foreach (var field in fields)
        {
            if (field.PropertyType == nameof(PreprocessorDirective))
                continue;

            var camelizeName = field.CamelizeName;
            var humanizeName = field.HumanizeName;
            switch (camelizeName)
            {
                case "createUserId":
                    camelizeName = "createUser";
                    break;
                case "operatorUserId":
                    camelizeName = "operatorUser";
                    break;
                case "disable":
                    stream.Write(
"""
    {
"""u8);
                    utf8String =
        """
title: '{0}', dataIndex:'{1}', width: {2}, ellipsis: true,
      render: (_, record: API.AccelerateProjectGroup) => [
        <Checkbox checked={record.disable} className='my-checkbox' onChange={(CheckboxChangeEvent) => SetUpDisable(CheckboxChangeEvent, record)}></Checkbox>],
      valueEnum: {
        'null': { text: '全部' },
        false: { text: '已禁用', status: 'Error' },
        true: { text: '已启用', status: 'Success' }
      }},

"""u8;
                    stream.WriteFormat(utf8String, humanizeName, camelizeName, humanizeName.Length * 10);

                    break;
            }
            stream.Write(
"""
    {
"""u8);
            utf8String =
"""
title: '{0}', dataIndex:'{1}', width: {2}, ellipsis: true
"""u8;
            stream.WriteFormat(utf8String, humanizeName, camelizeName, humanizeName.Length * 10);

            if (field.BackManageField == null || !field.BackManageField.Query)
            {
                utf8String =
"""
, hideInSearch: true 
"""u8;
                stream.Write(utf8String);
            }

            stream.Write(
"""
},

"""u8);
        }
        stream.Write(
"""
    (operation ? { title: '操作', valueType: 'option',key: 'option',
        width: 200,
        ellipsis: true,
        fixed: 'right',
"""u8);
        utf8String =
"""
 render: (text: any, record: API.{0}, index: number) => [
        <Space size="middle">
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        if (!metadata.BackManageEditModelReadOnly)
        {
            stream.Write(
"""

        {
"""u8);
            utf8String =
"""
access?.{0}?.Edit?
"""u8;
            stream.WriteFormat(utf8String, metadata.ClassName);

            utf8String =
"""
<a key={index} onClick={() => { onEdit(record)}}> 编辑 </a>:null }
"""u8;
            stream.Write(utf8String);
        }

        stream.Write(
"""

        {
"""u8);
        utf8String =
    """
access?.{0}?.Delete?
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        utf8String =
    """
<Button type="link" onClick={() => { showDeleteConfirm(record, true) }} danger>删除</Button>:null}

"""u8;
        stream.Write(utf8String);
        utf8String =
"""
        </Space>
      ],
    }:{
      hideInTable: true,
      hideInSearch: true
    }),

]

"""u8;
        stream.Write(utf8String);
    }

    void WriteModalForm(
    Stream stream,
    Metadata metadata,
    ImmutableArray<PropertyMetadata> fields,
    object?[] args)
    {
        if (metadata.BackManageCanAdd && metadata.BackManageEditModelReadOnly)
            return;

        ReadOnlySpan<byte> utf8String;
        utf8String =
"""
        <ModalForm
        title={editInfo != null ? "编辑" : "新增"}
        formRef={formRef}
        modalProps={{
          forceRender: true
        }}
        open={editModel}
        width={800}
        grid={true}
        onOpenChange={setEditModel}
        onFinish={async (values: any) => await onSaveFinish(values)}
      >
          {editInfo != null && <ProFormText
            key="id"
            name="id"
            readonly
            label="Id"
            hidden
          />}

"""u8;
        stream.Write(utf8String);
        foreach (var field in fields)
        {
            if (field.CamelizeName == "id")
                continue;

            else if (field.PropertyType == nameof(PreprocessorDirective))
                continue;

            else if (field.CamelizeName == "createUserId")
                continue;

            else if (field.CamelizeName == "operatorUserId")
                continue;

            switch (field.PropertyType.Replace('?', ' ').Trim())
            {
                case "string":
                    ProFormText(stream, field.HumanizeName, field.CamelizeName);
                    break;
                case "bool":
                    ProFormSwitch(stream, field.HumanizeName, field.CamelizeName);
                    break;
                case "int":
                    ProFormDigit(stream, field.HumanizeName, field.CamelizeName);
                    break;
                case "decimal":
                    ProFormDigit(stream, field.HumanizeName, field.CamelizeName);
                    break;
                case "DateTimeOffset":
                    ProFormDateTimePicker(stream, field.HumanizeName, field.CamelizeName);
                    break;
                default:
                    ProFormText(stream, field.HumanizeName, field.CamelizeName);
                    break;
            }
        }
        utf8String =
"""
        </ModalForm>

"""u8;
        stream.Write(utf8String);
    }

    void WriteProTable(
    Stream stream,
    Metadata metadata,
    ImmutableArray<PropertyMetadata> fields,
    object?[] args)
    {
        ReadOnlySpan<byte> utf8String;
        utf8String =
"""

    <AccessPage accessible={
"""u8;
        stream.Write(utf8String);
        utf8String =
"""
access?.{0}?.Query ?? false
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        utf8String =
"""
}>
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);

        utf8String =
"""

        <ProTable<API.{0}>

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);

        utf8String =
"""
          columns={columns}
          actionRef={actionRef}
          cardBordered
          request={async (params = {}, sort: any, filter: any) => {
          var data = await GetTable(params, sort, filter);
          return {
              data: data?.dataSource ?? [],
              success: true,
              total: data?.total
            };
          }}
          columnsState={{
          persistenceKey: 'NoticeTypeTableFilter',
          persistenceType: 'localStorage',
          }}
          rowKey="id"
          pagination={tablePagination}
          dateFormatter="string"
          search={{
          labelWidth: 'auto',
          }}
"""u8;
        stream.Write(utf8String);
        if (metadata.BackManageCanAdd)
        {
            utf8String =
"""

         toolBarRender={() => [
            <Button type="primary" onClick={() => onEdit(null)}>
              <PlusOutlined />
              新建
            </Button>
          ]}

"""u8;
            stream.Write(utf8String);
            utf8String =
"""
        />
        </AccessPage>

"""u8;
            stream.Write(utf8String);
        }
    }

    #region ProForm---组件

    void ProFormText(Stream stream, string humanizeName, string camelizeName)
    {
        stream.Write(
"""

          <ProFormText
          colProps={{ xs: 24, md: 12, xl: 8 }}
          rules={[]}
"""u8);
        stream.WriteFormat(
 """

          key="{0}"
          name="{0}"
          label="{1}"
          placeholder="请输入{1}"
          />

"""u8, camelizeName, humanizeName);

    }
    void ProFormSwitch(Stream stream, string humanizeName, string camelizeName)
    {
        stream.Write(
"""

          <ProFormSwitch
          colProps={{ xs: 24, md: 12, xl: 8 }}
          rules={[]}
"""u8);
        stream.WriteFormat(
 """

          key="{0}"
          name="{0}"
          label="{1}"
          />

"""u8, camelizeName, humanizeName);
    }

    void ProFormDateTimePicker(Stream stream, string humanizeName, string camelizeName)
    {
        stream.Write(
"""

          <ProFormDateTimePicker
          colProps={{ xs: 24, md: 12, xl: 8 }}
          rules={[]}
"""u8);
        stream.WriteFormat(
 """

          key="{0}"
          name="{0}"
          label="{1}"
          placeholder="请选择{1}"
          />

"""u8, camelizeName, humanizeName);
    }

    void ProFormDigit(Stream stream, string humanizeName, string camelizeName)
    {
        stream.Write(
"""

          <ProFormDigit
          colProps={{ xs: 24, md: 12, xl: 8 }}
          rules={[]}
"""u8);
        stream.WriteFormat(
 """

          key="{0}"
          name="{0}"
          label="{1}"
          placeholder="请输入{1}"
          />

"""u8, camelizeName, humanizeName);
    }

    #endregion

}
