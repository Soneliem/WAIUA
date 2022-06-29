using System.Windows;
using System.Windows.Controls;
using WAIUA.Objects;

namespace WAIUA.Controls
{
    public partial class InventoryControl : UserControl
    {
        
        public static readonly DependencyProperty SkinDataProperty =
            DependencyProperty.Register("SkinDataObject", typeof(SkinData), typeof(InventoryControl), new PropertyMetadata(new SkinData()));

        public InventoryControl(SkinData skinData)
        {
            InitializeComponent();
            SkinDataObject = skinData;
        }

        public SkinData SkinDataObject
        {
            get => (SkinData) GetValue(SkinDataProperty);
            set => SetValue(SkinDataProperty, value);
        }

        public static readonly RoutedEvent CloseButtonEvent =
            EventManager.RegisterRoutedEvent("SettingConfirmedEvent", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(InventoryControl));

        public event RoutedEventHandler CloseButton
            {
                add { AddHandler(CloseButtonEvent, value); }
                remove { RemoveHandler(CloseButtonEvent, value); }
            }

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(InventoryControl.CloseButtonEvent));

        }

    }
}
