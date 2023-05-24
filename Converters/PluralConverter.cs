using System.Globalization;

namespace Boggle.Converters
{
	internal class PluralConverter : IValueConverter
	{
		public string Word { get; set; }


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is int count ? string.Format(Word, count != 1 ? "s" : "") : Word;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
