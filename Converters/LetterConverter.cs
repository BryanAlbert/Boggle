using Boggle.Models;
using System.Globalization;

namespace Boggle.Converters
{
	internal class LetterConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string letters && parameter is string coordinates)
			{
				if (int.TryParse(coordinates[0].ToString(), out int x) && int.TryParse(coordinates[1].ToString(), out int y))
				{
					int size = (int) Math.Sqrt(letters.Length);
					if (x < size && y < size)
						return Game.RenderLetter(letters[x + y * size]);
				}
			}

			return "$";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
