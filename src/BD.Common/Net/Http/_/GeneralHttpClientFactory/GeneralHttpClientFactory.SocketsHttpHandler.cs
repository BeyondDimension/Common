//#if !NETSTANDARD

//// ReSharper disable once CheckNamespace
//namespace System.Net.Http;

//partial class GeneralHttpClientFactory
//{
//    [Obsolete("dotnet 7+ is not compatible with Windows 7")]
//    public static SocketsHttpHandler CreateSocketsHttpHandler(SocketsHttpHandler? handler = null)
//    {
//        handler ??= new();
//        Compat.Compatibility(handler);
//        return handler;
//    }
//}

//#endif