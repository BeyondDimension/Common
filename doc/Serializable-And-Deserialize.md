### 序列化与反序列化
- 对于新的功能业务需要用到的，首选 MemoryPack  
- 对于要与之前的数据保持兼容的，使用 MessagePack  
- 对于对接其他平台需要使用 JSON 的，首选 System.Text.Json
- Common 中 B64U 代表 Base64Url 编码，将 byte[] 编码成 string

#### **JSON**(JavaScript Object Notation, JS 对象简谱) 是一种轻量级的数据交换格式。
Newtonsoft.Json 与 System.Text.Json  
之前想用新的 System.Text.Json 完全替换旧的 Newtonsoft.Json， 所以在 DefineConstants 中加入了 ```__HAVE_N_JSON__``` 与 ```__NOT_HAVE_S_JSON__``` 配合 ```#if``` 进行源码中排除  
有关 DefineConstants 定义符号可参考以下文档  
https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/compiler-options/language#defineconstants  
但是两者有一些行为不一致，导致无法替换，这仅影响 Steam++/WattTookit 客户端上，在服务端上，应当使用 System.Text.Json 完全替换  
行为不一致可参考以下单元测试中的代码  
https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.UnitTest/JsonTest.cs#L30  
在 Common 库中使用  
- 序列化
	- System.Text.Json
		- ```Serializable.SJSON(Serializable.JsonImplType.SystemTextJson```
	- Newtonsoft.Json
		- ```Serializable.SJSON(Serializable.JsonImplType.NewtonsoftJson```
- 反序列化
	- System.Text.Json
		- ```Serializable.DJSON<T>(Serializable.JsonImplType.SystemTextJson```
	- Newtonsoft.Json
		- ```Serializable.DJSON<T>(Serializable.JsonImplType.NewtonsoftJson```

#### **MessagePack** 是一种高效的二进制序列化格式。它允许您像 JSON 一样在多个语言之间交换数据。但是，它更快并且更小。
https://msgpack.org/  
https://www.jianshu.com/p/8c24bef40e2f  
https://www.cnblogs.com/sunzhenchao/p/8448929.html  
https://cloud.tencent.com/developer/article/1489591  
https://learn.microsoft.com/zh-cn/aspnet/core/signalr/messagepackhubprotocol  
使用的包为 https://www.nuget.org/packages/messagepack  
https://github.com/neuecc/MessagePack-CSharp  
作者 neuecc 是日本一家游戏公司 Cysharp 的 Founder/CEO/CTO  
在 Common 库中使用  
- 序列化
	- ```Serializable.SMP(```
	- ```Serializable.SMPAsync(```
	- ```Serializable.SMPB64U(```
- 反序列化
	- ```Serializable.DMP<T>(```
	- ```Serializable.DMPAsync<T>(```
	- ```Serializable.DMPB64U<T>(```

#### **MemoryPack** 适用于 C# 和 Unity 的零编码极端性能二进制序列化程序。
https://github.com/Cysharp/MemoryPack  
https://www.cnblogs.com/InCerry/p/Dotnet-Perf-Opt-Serialization-Protocol.html  
https://www.cnblogs.com/InCerry/p/how-to-make-the-fastest-net-serializer-with-net-7-c-11-case-of-memorypack.html  
在 Common 库中使用  
- 序列化
	- ```Serializable.SMP2(```
	- ```Serializable.SMP2Async(```
	- ```Serializable.SMP2B64U(```
- 反序列化
	- ```Serializable.DMP2<T>(```
	- ```Serializable.DMP2Async<T>(```
	- ```Serializable.DMP2B64U<T>(```

#### **Protobuf** 协议缓冲区是一种与语言无关、与平台无关的可扩展机制，用于序列化结构化数据。
https://learn.microsoft.com/zh-cn/aspnet/core/grpc/protobuf  
https://protobuf.dev/getting-started/csharptutorial/  
使用的包为 https://www.nuget.org/packages/Google.Protobuf  
协议缓冲区是 Google 公司内部的混合语言数据标准， 在 Steam 的 API 上有用到  
在 https://github.com/BeyondDimension/SteamClient 项目中使用  
在 Common 库中使用  
	- 尚不支持，没有支持的计划
