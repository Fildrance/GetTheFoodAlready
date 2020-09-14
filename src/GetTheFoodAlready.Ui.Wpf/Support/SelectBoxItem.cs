using System;
using System.ComponentModel;
using System.Globalization;
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
		private Func<string> _textFactory;
		private readonly string _text;
		#endregion

		#region [c-tor]
		public SelectBoxItem(TValue value, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException(text);
			}

			Value = value;
			_text = text;
		}
		public SelectBoxItem(TValue value, Func<string> textFactory, IObservable<CultureInfo> textChanged)
		{
			Value = value;
			_textFactory = textFactory ?? throw new ArgumentNullException(nameof(textFactory));

			textChanged.Subscribe(x => OnPropertyChanged(nameof(Text)));
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
		public string Text 
		{ 
			get 
			{
				return _text ?? _textFactory();
			}
		}
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