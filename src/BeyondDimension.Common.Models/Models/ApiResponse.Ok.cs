// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

partial class ApiResponse
{
    public static readonly ApiResponse Ok = new()
    {
        IsSuccess = true,
    };
}
