using Boggle.Models;
using System.Globalization;

namespace Boggle.Converters
{
	internal class WordConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is string letters)
				return Game.RenderWord(letters);

			return "Error";
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}
}
