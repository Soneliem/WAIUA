using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WAIUA.Objects;

namespace WAIUA.Controls;

/// <summary>
///     Interaction logic for OverlayControl.xaml
/// </summary>
public partial class OverlayControl : UserControl
{
    public static readonly DependencyProperty OverlayProperty =
        DependencyProperty.Register("Overlay", typeof(LoadingOverlay), typeof(OverlayControl), new PropertyMetadata(new LoadingOverlay()));

    public OverlayControl()
    {
        InitializeComponent();
    }

    public LoadingOverlay Overlay
    {
        get => (LoadingOverlay) GetValue(OverlayProperty);
        set => SetValue(OverlayProperty, value);
    }

    private void ImageClickAsync(object sender, RoutedEventArgs e)
    {
        var button = (Button) sender;
        Process.Start(new ProcessStartInfo(button.Tag.ToString()) {UseShellExecute = true});
        e.Handled = true;
    }
}