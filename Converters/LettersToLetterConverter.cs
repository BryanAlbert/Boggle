using Boggle.Models;
using System.Globalization;

namespace Boggle.Converters
{
	internal class LettersToLetterConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is string letters && parameter is string coordinates && int.TryParse(coordinates[0].ToString(), out int x) &&
				int.TryParse(coordinates[1].ToString(), out int y))
			{
				int size = (int) Math.Sqrt(letters.Length);
				if (x < size && y < size)
				{
					string letter = Game.RenderLetter(letters[x + y * size]);
					return letter != " " ? letter : string.Empty;
				}
			}

			return "$";
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value is string letter && parameter is string coordinates && char.IsDigit(coordinates[0]) && char.IsDigit(coordinates[1])
				? letter.Length > 0 ? coordinates + Game.ParseLetter(letter) : coordinates : "$";
		}
	}
}
