namespace BD.Common.UI.ViewModels.Abstractions;

public interface IReadOnlyItemViewType
{
    int ItemViewType { get; }
}

public interface IReadOnlyItemViewType<TItemViewType> : IReadOnlyItemViewType where TItemViewType : struct, Enum
{
    new TItemViewType ItemViewType { get; }

    int IReadOnlyItemViewType.ItemViewType => ItemViewType.ConvertToInt32();
}