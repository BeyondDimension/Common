// ReSharper disable once CheckNamespace
namespace BD.Common.Columns;

public interface ITitle
{
    const int MaxLength = 30;
    const int MaxLength200 = 200;

    /// <summary>
    /// 标题
    /// </summary>
    string Title { get; set; }
}
