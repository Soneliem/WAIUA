using System.Windows.Data;

namespace WAIUA.Commands
{
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
			this.Source = WAIUA.Properties.Settings.Default;
			this.Mode = BindingMode.TwoWay;
		}
	}
}