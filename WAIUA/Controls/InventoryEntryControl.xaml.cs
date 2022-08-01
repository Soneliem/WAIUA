using System;
using System.Windows;
using System.Windows.Controls;

namespace WAIUA.Controls;

public partial class InventoryEntryControl : UserControl
{
    public static readonly DependencyProperty GunTooltipNameProperty =
        DependencyProperty.Register("GunTooltipName", typeof(string), typeof(InventoryEntryControl), new PropertyMetadata(null));

    public static readonly DependencyProperty GunImageProperty =
        DependencyProperty.Register("GunImage", typeof(Uri), typeof(InventoryEntryControl), new PropertyMetadata(null));

    public static readonly DependencyProperty BuddyTooltipNameProperty =
        DependencyProperty.Register("BuddyTooltipName", typeof(string), typeof(InventoryEntryControl), new PropertyMetadata(null));

    public static readonly DependencyProperty BuddyImageProperty =
        DependencyProperty.Register("BuddyImage", typeof(Uri), typeof(InventoryEntryControl), new PropertyMetadata(null));

    public InventoryEntryControl()
    {
        InitializeComponent();
    }

    public Uri GunImage
    {
        get => (Uri) GetValue(GunImageProperty);
        set => SetValue(GunImageProperty, value);
    }

    public string GunTooltipName
    {
        get => (string) GetValue(GunTooltipNameProperty);
        set => SetValue(GunTooltipNameProperty, value);
    }

    public Uri BuddyImage
    {
        get => (Uri) GetValue(BuddyImageProperty);
        set => SetValue(BuddyImageProperty, value);
    }

    public string BuddyTooltipName
    {
        get => (string) GetValue(BuddyTooltipNameProperty);
        set => SetValue(BuddyTooltipNameProperty, value);
    }
}