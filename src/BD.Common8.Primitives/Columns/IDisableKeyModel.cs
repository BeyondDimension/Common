namespace BD.Common8.Columns;

/// <summary>
/// 表示一个带有禁用状态和主键的模型的接口
/// </summary>
/// <typeparam name="TPrimaryKey">主键的类型</typeparam>
public interface IDisableKeyModel<TPrimaryKey> : IDisable, IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
}