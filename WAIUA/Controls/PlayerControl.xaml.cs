using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WAIUA.Objects;

namespace WAIUA.Controls;

public partial class PlayerControl : UserControl
{
    public static readonly DependencyProperty PlayerProperty =
        DependencyProperty.Register("PlayerCell", typeof(Player), typeof(PlayerControl), new PropertyMetadata(new Player()));

    public PlayerControl()
    {
        InitializeComponent();
        AddHandler(InventoryControl.CloseButtonEvent,
            new RoutedEventHandler(ClosePopupEventHandlerMethod));
    }

    public Player PlayerCell
    {
        get => (Player) GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    private void HandleLinkClick(object sender, MouseButtonEventArgs e)
    {
        var s = sender as FrameworkElement;
        var player = s.DataContext as Player;
        Process.Start(new ProcessStartInfo(player.IgnData.TrackerUri.ToString()) {UseShellExecute = true});
        e.Handled = true;
    }

    private void ButtonUpHandler(object sender, MouseButtonEventArgs e)
    {
        var s = sender as FrameworkElement;
        var player = s.DataContext as Player;
        popup.Child = player.IgnData.Username == "----" ? new InventoryControl(player.SkinData, player.IdentityData.Name) : new InventoryControl(player.SkinData, player.IgnData.Username);
        popup.IsOpen = true;
        e.Handled = true;
    }

    private void ClosePopupEventHandlerMethod(object sender,
        RoutedEventArgs e)
    {
        popup.IsOpen = false;
        e.Handled = true;
    }
}