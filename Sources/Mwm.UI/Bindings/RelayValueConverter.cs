using System;
using System.Globalization;

namespace Mwm.UI
{
	public class RelayValueConverter : IValueConverter
	{
		public RelayValueConverter(Func<object, Type, object, CultureInfo, object > convert, Func<object, Type, object, CultureInfo, object> convertBack)
		{
			this.convert = convert;
			this.convertBack = convertBack;
		}

		private Func<object, Type, object, CultureInfo, object> convert, convertBack;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return this.convertBack(value, targetType, parameter, culture);
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return this.convert(value, targetType, parameter, culture);
		}
	}
}
