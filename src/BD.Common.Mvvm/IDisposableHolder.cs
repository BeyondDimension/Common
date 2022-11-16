namespace BD.Common;

public interface IDisposableHolder : IDisposable
{
    ICollection<IDisposable> CompositeDisposable { get; }
}