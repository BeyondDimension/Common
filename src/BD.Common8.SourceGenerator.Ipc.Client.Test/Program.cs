Console.WriteLine("Hello, World!");

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.ClientWebApi)]
sealed partial class TodoService_WebApi
{
}

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.ClientSignalR)]
sealed partial class TodoService_SignalR
{
}