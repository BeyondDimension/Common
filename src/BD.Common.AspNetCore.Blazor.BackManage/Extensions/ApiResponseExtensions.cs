// ReSharper disable once CheckNamespace
namespace BD.Common;

public static partial class ApiResponseExtensions
{
    public static string GetErrorMessage(this IApiResponse rsp) => rsp.Messages.FirstOrDefault() ?? "";
}
