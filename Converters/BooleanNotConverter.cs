using System.Globalization;

namespace Boggle.Converters
{
	internal class BooleanNotConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
			value is bool boolean && !boolean;

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => 
			throw new NotImplementedException();
	}
}
