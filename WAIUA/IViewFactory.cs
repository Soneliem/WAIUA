using System.Windows;

namespace WAIUA;

public interface IViewFactory
{
    FrameworkElement? ResolveView(object viewModel);
}