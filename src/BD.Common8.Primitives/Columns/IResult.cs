namespace BD.Common8.Primitives.Columns;

#pragma warning disable SA1600 // Elements should be documented

public interface IResult<T>
{
    T Result { get; set; }
}