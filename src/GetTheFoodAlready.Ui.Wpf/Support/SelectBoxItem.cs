using System.ComponentModel;
using System.Runtime.CompilerServices;

using GetTheFoodAlready.Ui.Wpf.Annotations;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Wrapper for select-box like component items. </summary>
	/// <typeparam name="TValue">Type of value to be held.</typeparam>
	public class SelectBoxItem<TValue> : INotifyPropertyChanged
	{
		#region [Fields]
		private bool _isSelected;
		#endregion

		#region [c-tor]
		public SelectBoxItem(TValue value, string text)
		{
			Value = value;
			Text = text;
		}
		#endregion

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region [Public]
		#region [Public properties]
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}
		public string Text { get; }

		public TValue Value { get; }
		#endregion
		#endregion

		#region [Protected]
		#region [Protected methods]
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#endregion
	}
}