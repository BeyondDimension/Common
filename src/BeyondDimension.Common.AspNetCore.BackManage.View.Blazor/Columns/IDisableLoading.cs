// ReSharper disable once CheckNamespace
namespace BD.Common.Columns;

public interface IDisableLoading : IDisable
{
    bool DisableLoading { get; set; }
}
