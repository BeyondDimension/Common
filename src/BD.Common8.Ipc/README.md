# BD.Common8.Ipc [![Crowdin](https://badges.crowdin.net/bdcommon8/localized.svg)](https://crowdin.com/project/bdcommon8) [![NuGet](https://img.shields.io/nuget/v/BD.Common8.Ipc.svg)](https://www.nuget.org/packages/BD.Common8.Ipc) [![license](https://img.shields.io/badge/license-MIT%20License-yellow.svg)](https://github.com/BeyondDimension/Common/blob/dev8/LICENSE)
进程间通信 (IPC) 库  

[API browser](https://beyonddimension.github.io/Common/api/index.html)

传输层协议
1. TCP/HTTP 协议，常见的 Web 技术，便于从网页以及跨语言编程中调用，需要监听一个 TCP 端口
2. [NamedPipe](https://learn.microsoft.com/zh-cn/aspnet/core/grpc/interprocess-namedpipes) 命名管道，Windows 原生支持的管道通信，Linux 上使用 Unix 域套接字 (UDS) 来实现，需要一个唯一的字符串作为管道名，Windows 上涉及管理员权限进程交互需要额外配置
3. [UnixSocket](https://learn.microsoft.com/zh-cn/aspnet/core/grpc/interprocess-uds)，简称 UDS，需要一个文件，文件路径字符串长度也有限制，路径不能太长，使用文件流来通信

应用层协议
1. HTTP/2/3 协议，使用 [ControllerBase](https://learn.microsoft.com/zh-cn/aspnet/core/web-api/) 或 [Minimal API](https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis/overview)，控制器需要在启动时反射类遍历查找函数添加进路由字典，而 Minimal API 将直接添加注册路由
2. [SignalR](https://learn.microsoft.com/zh-cn/aspnet/core/signalr/introduction)，使用 WebSockets、Server-Sent Events、长轮询的方式，Hub 作为服务类，也是通过反射遍历查找函数
3. [gRPC](https://learn.microsoft.com/zh-cn/aspnet/core/grpc/aspnetcore)，对 Protobuf 强依赖，服务层接口需要定义 .proto 文件，源生成服务基类，继承子类实现业务函数，由源生成添加路由的代码片段
4. [MagicOnion](https://github.com/Cysharp/MagicOnion)，基于 gRPC，支持以 C# 服务接口直接定义，无需 .proto 文件，服务端使用了反射调用泛型函数以及动态创建程序集，仅客户端可使用源生成

序列化实现类型
1. Newtonsoft.Json
2. MessagePack
3. System.Text.Json
4. MemoryPack
5. Xml
6. Protobuf