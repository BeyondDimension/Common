namespace BD.Common.SourceGenerator.Models;

/// <summary>
/// 表实体源数据
/// </summary>
/// <param name="ModuleName">模块名称</param>
/// <param name="TypeName">类型名称</param>
/// <param name="KeyTypeName">主键类型</param>
/// <param name="Properties">表的字段</param>
/// <param name="TableName">表名</param>
/// <param name="SecondaryPath">二级路径，例如 Script 或 AppVersion/Article/ExchangeRate 等</param>
/// <param name="DatabaseGeneratedOption">DatabaseGeneratedAttribute System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption</param>
/// <param name="NEWSEQUENTIALID">是否继承自 INEWSEQUENTIALID</param>
public sealed record class EntityMetadata(
    string ModuleName,
    string TypeName,
    string? KeyTypeName,
    EntityProperty[] Properties,
    string? TableName = default,
    string? SecondaryPath = default,
    string? DatabaseGeneratedOption = default,
    bool NEWSEQUENTIALID = default)
{
    public string GetTableName()
    {
        if (!string.IsNullOrWhiteSpace(TableName))
            return TableName!;
        return $"{TypeName}s";
    }
}
