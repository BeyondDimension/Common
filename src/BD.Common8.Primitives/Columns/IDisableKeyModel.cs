namespace BD.Common8.Primitives.Columns;

#pragma warning disable SA1600 // Elements should be documented

public interface IDisableKeyModel<TPrimaryKey> : IDisable, IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
}