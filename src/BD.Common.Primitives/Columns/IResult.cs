namespace BD.Common.Columns;

public interface IResult<T>
{
    T Result { get; set; }
}