using System;
using System.Windows;
using System.Windows.Controls;

namespace WAIUA.Controls;

public partial class InventoryEntryControl : UserControl
{
    public static readonly DependencyProperty TooltipNameProperty =
        DependencyProperty.Register("TooltipName", typeof(string), typeof(InventoryEntryControl), new PropertyMetadata(null));

    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register("Image", typeof(Uri), typeof(InventoryEntryControl), new PropertyMetadata(null));

    public InventoryEntryControl()
    {
        InitializeComponent();
    }

    public Uri Image
    {
        get => (Uri) GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public string TooltipName
    {
        get => (string) GetValue(NameProperty);
        set => SetValue(TooltipNameProperty, value);
    }
}