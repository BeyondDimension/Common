// ReSharper disable once CheckNamespace
namespace System.Security.Cryptography;

public static class ServerSecurity
{
    public static ValueTask<string> Encrypt(IJSRuntime js, string value)
        => js.InvokeAsync<string>("RSA_Encrypt", value);
}