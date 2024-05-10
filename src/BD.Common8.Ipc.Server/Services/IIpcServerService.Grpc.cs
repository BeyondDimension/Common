//using PB = Google.Protobuf;
//using PBC = Google.Protobuf.Collections;

//namespace BD.Common8.Ipc.Services;

//partial interface IIpcServerService
//{
//    /// <summary>
//    /// 用于定义以指定 prefix 为前缀的所有终结点。
//    /// <para>Ipc 服务接口必须继承此接口，且调用 <see cref="IpcServiceCollectionServiceExtensions.AddSingletonWithIpcServer"/> 添加到 <see cref="Ioc"/></para>
//    /// </summary>
//    unsafe interface IHandle
//    {
//        /// <summary>
//        /// 由源生成器提供实现。
//        /// </summary>
//        static abstract delegate* managed<PBC::RepeatedField<string>, PB::ByteString, Stream, CancellationToken, Task<bool>> Stream();

//        /// <summary>
//        /// 由源生成器提供实现。
//        /// </summary>
//        static abstract delegate* managed<PBC::RepeatedField<string>, PB::ByteString, CancellationToken, (bool, IAsyncEnumerable<byte[]>)> AsyncEnumerable();
//    }
//}