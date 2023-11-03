// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Types/Shared/Exceptions.shared.cs
namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 调用需要特定权限的 API 时发生的异常
/// </summary>
/// <remarks>
/// 使用指定的消息初始化 <see cref="PermissionException"/> 类的新实例
/// </remarks>
/// <param name="message">A message that describes this exception in more detail.</param>
public sealed class PermissionException(string message) : UnauthorizedAccessException(message)
{
}