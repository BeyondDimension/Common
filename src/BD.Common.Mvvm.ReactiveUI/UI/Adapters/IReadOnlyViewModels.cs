namespace BD.Common.UI.Adapters;

public interface IReadOnlyViewModels<out TViewModel>
{
    IReadOnlyList<TViewModel> ViewModels { get; }
}