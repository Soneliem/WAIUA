using System.Windows;
using System.Windows.Controls;
using WAIUA.Objects;

namespace WAIUA.Controls;

public partial class InventoryControl : UserControl
{
    public static readonly DependencyProperty SkinDataProperty =
        DependencyProperty.Register("SkinDataObject", typeof(SkinData), typeof(InventoryControl), new PropertyMetadata(new SkinData()));

    public static readonly DependencyProperty UsernameProperty =
        DependencyProperty.Register("Username", typeof(string), typeof(InventoryControl), new PropertyMetadata(null));

    public static readonly RoutedEvent CloseButtonEvent =
        EventManager.RegisterRoutedEvent("SettingConfirmedEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(InventoryControl));

    public InventoryControl(SkinData skinData, string username)
    {
        InitializeComponent();
        SkinDataObject = skinData;
        Username = username;
    }

    public SkinData SkinDataObject
    {
        get => (SkinData) GetValue(SkinDataProperty);
        set => SetValue(SkinDataProperty, value);
    }

    public string Username
    {
        get => (string) GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    public event RoutedEventHandler CloseButton
    {
        add => AddHandler(CloseButtonEvent, value);
        remove => RemoveHandler(CloseButtonEvent, value);
    }

    private void CloseBtnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(CloseButtonEvent));
    }
}