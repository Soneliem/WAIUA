using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WAIUA.Controls
{
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
            get => (Uri)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }
        
        public String TooltipName
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(TooltipNameProperty, value);
        }
    }
}
