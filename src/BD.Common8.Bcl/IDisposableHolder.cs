namespace System;

#pragma warning disable SA1600 // Elements should be documented

public interface IDisposableHolder : IDisposable
{
    ICollection<IDisposable> CompositeDisposable { get; }
}