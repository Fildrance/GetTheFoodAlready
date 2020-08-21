namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Wrapper for select-box like component items. </summary>
	/// <typeparam name="TValue">Type of value to be held.</typeparam>
	public class SelectBoxItem<TValue>
	{
		#region [c-tor]
		public SelectBoxItem(TValue value, string text)
		{
			Value = value;
			Text = text;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public string Text { get; }

		public TValue Value { get; }
		#endregion
		#endregion
	}
}