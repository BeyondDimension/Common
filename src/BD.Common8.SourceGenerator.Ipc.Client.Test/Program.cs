// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.ClientWebApi)]
sealed partial class TodoService_WebApi(IpcAppConnectionString connectionString) : IpcClientService(connectionString)
{
}

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.ClientSignalR)]
sealed partial class TodoService_SignalR(IpcAppConnectionString connectionString) : IpcClientService(connectionString)
{
}