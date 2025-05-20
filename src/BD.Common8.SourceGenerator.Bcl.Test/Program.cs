//using BD.Common8.SourceGenerator.Bcl.Test;
//using BD.Common8.SourceGenerator.Bcl.Test.Models;
using BD.Common8.SourceGenerator.Bcl.Test.Services;
//using BD.Common8.SourceGenerator.Bcl.Test.ViewModels;
//using MemoryPack;
//using System.Runtime.CompilerServices;

// https://learn.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/generators/observableproperty

Console.WriteLine(typeof(TodoService).FullName);

//var s1 = TodoService.Current;
//var s2 = TodoService.Current;
//Console.WriteLine(s2.GetHashCode());
//if (s1 != s2)
//    throw new ArgumentOutOfRangeException(nameof(s2));

//var ss = await Task2.InParallel(
//    Enumerable.Range(0, 15).Select(static _ => Task.Run(() =>
//    {
//        var result = TodoService.Current;
//        Console.WriteLine(
//            $"ManagedThreadId: {Environment.CurrentManagedThreadId}, s: {result.GetHashCode()}");
//        return result;
//    })));
//foreach (var item in ss)
//{
//    if (s1 != item)
//        throw new ArgumentOutOfRangeException(nameof(item));
//}

//var m = new TodoModel();
//var d = m.D2;
//var vm = new TodoViewModel(m);

//var c = vm.C;

////var a1 = new C1Model();
////a1.SetC1Model();

////var a2 = new C2Model();
////a2.SetC2Model();

//Console.WriteLine("Wait ReadLine Exit!");
//Console.ReadLine();

namespace BD.Common8.SourceGenerator.Bcl.Test
{
    ///// <summary>
    ///// <see cref="ReactiveObject"/> 的序列化忽略基类
    ///// </summary>
    //public abstract class ReactiveSerializationObject : ReactiveObject
    //{
    //    /// <inheritdoc cref="ReactiveObject.Changing" />
    //    [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
    //    public new IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => base.Changing;

    //    /// <inheritdoc cref="ReactiveObject.Changed" />
    //    [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
    //    public new IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => base.Changed;

    //    /// <inheritdoc cref="ReactiveObject.ThrownExceptions" />
    //    [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
    //    public new IObservable<Exception> ThrownExceptions => base.ThrownExceptions;
    //}
}