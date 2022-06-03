using System.Windows;
using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace WAIUA;

public class ViewModelPresenter : ContentControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewModelPresenter),
            new PropertyMetadata(null, OnViewModelChanged));

    public ViewModelPresenter()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
    }

    public object ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private static void OnViewModelChanged(DependencyObject changedObject, DependencyPropertyChangedEventArgs args)
    {
        var contentControl = (ViewModelPresenter) changedObject;
        contentControl.RefreshContentPresenter();
    }

    private void RefreshContentPresenter()
    {
        if (ViewModel == null)
        {
            Content = null;

            return;
        }

        var viewFactory = Ioc.Default.GetRequiredService<IViewFactory>();
        var view = viewFactory.ResolveView(ViewModel);

        if (view != null)
        {
            view.DataContext = ViewModel;
            Content = view;
        }
        else
        {
            Content = null;
        }
    }
}