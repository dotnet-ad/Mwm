namespace Mwm.UI
{
	using System;
	using System.ComponentModel;

	public interface IElement : INotifyPropertyChanged, IDisposable
	{
		IElement Parent { get; set; }

		object DataContext { get; set; }

		int Identifier { get; }

		string Name { get; set; }

		Visibility Visibility { get; set; }

		Alignment HorizontalAlignment { get; set; }

		Alignment VerticalAlignment { get; set; }

		Thickness Margin { get; set; }

		int Width { get; set; }

		int Height { get; set; }

		void Bind(string toProperty, string fromContextProperty, IValueConverter converter = null);

		void BindTwoWay(string toProperty, string fromContextProperty, IValueConverter converter = null);

		void UpdateBindings();
	}
}
