using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using WAIUA.Objects;

namespace WAIUA.Controls;

public partial class PlayerControl : UserControl
{
    public static readonly DependencyProperty PlayerProperty =
        DependencyProperty.Register("PlayerCell", typeof(Player), typeof(PlayerControl), new PropertyMetadata(new Player()));

    public PlayerControl()
    {
        InitializeComponent();
    }

    public Player PlayerCell
    {
        get => (Player) GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    private void HandleLinkClick(object sender, RequestNavigateEventArgs e)
    {
        var hl = (Hyperlink) sender;
        var navigateUri = hl.NavigateUri.ToString();
        Process.Start(new ProcessStartInfo(navigateUri) {UseShellExecute = true});
        e.Handled = true;
    }
}