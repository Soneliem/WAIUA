using System.Windows.Data;
using WAIUA.Properties;

namespace WAIUA.Commands;

public class SettingBinding : Binding
{
    public SettingBinding()
    {
        Initialize();
    }

    public SettingBinding(string path)
        : base(path)
    {
        Initialize();
    }

    private void Initialize()
    {
        Source = Settings.Default;
        Mode = BindingMode.TwoWay;
    }
}