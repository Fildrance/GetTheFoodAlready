using System.Windows.Data;

namespace GetTheFoodAlready.Ui.Wpf.Localization
{
	public class LocExtension : Binding
    {
        public LocExtension(string name) : base("[" + name + "]")
        {
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}
