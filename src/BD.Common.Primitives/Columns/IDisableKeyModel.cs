// ReSharper disable once CheckNamespace
namespace BD.Common.Columns;

public interface IDisableKeyModel<TPrimaryKey> : IDisable, IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{

}