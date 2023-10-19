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
import { useState, useRef } from 'react';
import { Button, message } from 'antd';
import { useModel, useAccess } from '@umijs/max'
import { PlusOutlined } from '@ant-design/icons';
import { ProTable, ModalForm, ProFormText, ProFormDateTimePicker, ProFormDigit, ProFormSwitch } from '@ant-design/pro-components'
import type { ActionType, ProColumns, ProFormInstance } from '@ant-design/pro-components';
import AccessPage from '@/components/AccessPage'
import getOperationColumn, { OperationColumnButtonType } from '@/components/OperationColumn';
import { getDisableColumn } from '@/components/CommonColumn/DisableColumn';
import { DateTimeColumn } from '@/components/CommonColumn';

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
            ImportMethod.Add("  " + metadata.ClassName + "Query");
        }
        if (metadata.BackManageCanEdit)
        {
            ImportMethod.Add("  " + metadata.ClassName + "EditById");
        }
        if (!metadata.BackManageEditModelReadOnly || metadata.BackManageCanAdd)
        {
            ImportMethod.Add("  " + metadata.ClassName + "Save");
        }
        foreach (var field in fields)
        {
            switch (field.FixedProperty)
            {
                case FixedProperty.Disable:
                    ImportMethod.Add("  " + metadata.ClassName + "Disable");
                    break;
                case FixedProperty.Title:
                    ImportMethod.Add("  " + metadata.ClassName + "Select");
                    break;
            }
        }
        ImportMethod.Add("  " + metadata.ClassName + "Delete");
        utf8String =
"""
{0}
"""u8;
        stream.WriteFormat(utf8String, string.Join(",\r\n", ImportMethod));
        stream.Write(
"""

}
"""u8);
        utf8String =
"""
 from '@/services/{0}/{1}/Generated/api'

"""u8;
        stream.WriteFormat(utf8String, metadata.GenerateRepositoriesAttribute.ModuleName, metadata.ClassName);
    }

    void WriteConstMethod(
    Stream stream,
    Metadata metadata,
    ImmutableArray<PropertyMetadata> fields,
    object?[] args)
    {
        ReadOnlySpan<byte> utf8String;
        utf8String =
"""

const Manage: React.FC = () => {
  const access = useAccess() as API.RoleData;
  const { tablePagination } = useModel('bodySize');

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

        #region Query
        utf8String =
"""

  const queryTable = async (params: any) => {

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        utf8String =
"""
    const response = await {0}Query(params);
    return {
      data: response.data?.dataSource ?? [],
      success: response.isSuccess,
      total: response.data?.total ?? 0
    };
  }

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);

        #endregion

        #region onEdit
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
    if (formRef?.current) {
      if (info == null) {
        formRef.current.resetFields();
        setEditInfo(null);
      }
      else {
        const data = { ...info };
        formRef.current.setFieldsValue(data);
        setEditInfo(info);
      }
    }
  }

"""u8;
        stream.Write(utf8String);
        #endregion

        #region onSaveFinish

        utf8String =
"""

  const onSaveFinish = async (values: API.{0}) => {
    const data = { ...values };

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        utf8String =
"""
    const response = await {0}Save
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
      formRef.current?.resetFields();
      actionRef.current?.reload();
    }
    return response.isSuccess;
  }

"""u8;
        stream.Write(utf8String);
        #endregion

        #region onDelete
        utf8String =
"""

  const {0} = async (info: API.{1}) =>
"""u8;
        stream.WriteFormat(utf8String, "onDelete", metadata.ClassName);
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

      const response = await {0}(info!.id);

"""u8;
        stream.WriteFormat(utf8String, $"{metadata.ClassName}Delete");
        utf8String =
"""
      if (response.isSuccess) {
        actionRef.current?.reload();
      }
    }
  }

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        #endregion
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

  const operation = access?.{0}?.Edit || access?.{0}?.Delete;
  const operationColumn = getOperationColumn<API.{0}>([

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
    {

"""u8);
        utf8String =
"""
      type: OperationColumnButtonType.Edit,
      authorized: access?.{0}?.Edit,
      action: onEdit

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
    },

"""u8);
        stream.Write(
"""
    {

"""u8);
        utf8String =
"""
      type: OperationColumnButtonType.Delete,
      authorized: access?.{0}?.Delete,
      action: onDelete

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        stream.Write(
"""
    },
  ]);


"""u8);
        utf8String =
"""
  const columns: ProColumns<API.{0}>[] = [

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        foreach (var field in fields)
        {
            if (field.PropertyType == nameof(PreprocessorDirective))
                continue;
            if (field.Name == "SoftDeleted")
                continue;

            var camelizeName = field.CamelizeName;
            var humanizeName = field.HumanizeName;
            switch (camelizeName)
            {
                case "createUserId":
                    camelizeName = "createUser";
                    continue;
                case "operatorUserId":
                    camelizeName = "operatorUser";
                    continue;
                case "disable":
                    stream.Write(
"""
    getDisableColumn({ executeApi: 
"""u8);
                    stream.WriteFormat(
"""
{0}Disable
"""u8, metadata.ClassName);
                    stream.Write(
"""
, onOk: () => actionRef.current?.reload() }),

"""u8);
                    continue;
            }
            stream.Write(
"""
    {
"""u8);
            utf8String =
"""
 title: '{0}', dataIndex: '{1}', width: {2}, ellipsis: true
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
            if (field.PropertyType == "DateTimeOffset")
            {
                utf8String =
"""
, ...DateTimeColumn.GetDateTimeProps()
"""u8;
                stream.Write(utf8String);
            }
            stream.Write(
"""
 },

"""u8);
        }

        utf8String =
"""
  ]
  if (operation) {
    columns.push(operationColumn)
  }

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
        onFinish={onSaveFinish}>

        {editInfo != null && <ProFormText
          key="id"
          name="id"
          readonly
          label="Id"
          hidden />}

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
                    if (field.Name == "SoftDeleted")
                        continue;
                    ProFormSwitch(stream, field.HumanizeName, field.CamelizeName);
                    break;
                case "int":
                    ProFormDigit(stream, field.HumanizeName, field.CamelizeName);
                    break;
                case "decimal":
                    ProFormDigit(stream, field.HumanizeName, field.CamelizeName);
                    break;
                case "long":
                    ProFormDigit(stream, field.HumanizeName, field.CamelizeName);
                    break;
                case "DateTimeOffset":
                    if (field.Name == "CreationTime" || field.Name == "UpdateTime")
                        continue;
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
          request={queryTable}
          actionRef={actionRef}
          rowKey="id"
          columns={columns}
          columnsState={{
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

            persistenceKey: '{0}TableFilter',

"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);
        utf8String =
"""
            persistenceType: 'localStorage',
            defaultValue: {
              "order": { "show": false }
            }
          }}
          cardBordered
          scroll={{ x: 1200 }}
          pagination={tablePagination}
          dateFormatter="string"
          search={{ labelWidth: 'auto' }}
"""u8;
        stream.Write(utf8String);
        if (metadata.BackManageCanAdd)
        {
            utf8String =
"""

          toolBarRender={() => [
            access?.
"""u8;
            stream.Write(utf8String);
            stream.Write(metadata.ClassName);
            utf8String =
"""
?.Add && <Button type="primary" key={'add'} onClick={() => (null)}><PlusOutlined /> 新建 </Button>
          ]}

"""u8;
            stream.Write(utf8String);
        }
        utf8String =
"""
        />
      </AccessPage>

"""u8;
        stream.Write(utf8String);

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

          name="{0}"
          label="{1}"
          placeholder="请输入{1}" />

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
          label="{1}" />

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
          placeholder="请选择{1}" />

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
          placeholder="请输入{1}" />

"""u8, camelizeName, humanizeName);
    }

    #endregion

}
