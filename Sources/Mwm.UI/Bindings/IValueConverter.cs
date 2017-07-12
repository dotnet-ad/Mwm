using System;
using System.Globalization;

namespace Mwm.UI
{
	public interface IValueConverter
	{
		object Convert(object value, Type targetType, object parameter, CultureInfo culture);

		object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
	}

	public static class ValueConverterExtensions
	{
		public static IValueConverter Invert(this IValueConverter converter)
		{
			return new RelayValueConverter((arg1, arg2, arg3, arg4) => converter.ConvertBack(arg1, arg2, arg3, arg4), (arg1, arg2, arg3, arg4) => converter.Convert(arg1, arg2, arg3, arg4));
		}
	}
}
