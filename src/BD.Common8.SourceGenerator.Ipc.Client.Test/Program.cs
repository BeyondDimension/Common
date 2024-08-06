Console.WriteLine("Hello, World!");

//[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.ClientWebApi)]
//sealed partial class TodoService_WebApi
//{
//}

//[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.ClientSignalR)]
//sealed partial class TodoService_SignalR
//{
//}

//[ServiceContractImpl(typeof(ITodoService3), IpcGeneratorType.ClientSignalR)]
//sealed partial class TodoService3_SignalR
//{
//}

//[ServiceContractImpl(typeof(ITodoService4), IpcGeneratorType.ClientSignalR)]
//sealed partial class TodoService4_SignalR
//{
//}

[ServiceContractImpl(typeof(IFileSystemElevatedService), IpcGeneratorType.ClientWebApi)]
sealed partial class FrontendFileSystemElevatedServiceImpl
{
}