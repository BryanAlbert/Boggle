using System.Globalization;

namespace Boggle.Converters
{
	internal class PathToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int index = 0;
			if (value is int[] path && parameter is string coordinates)
			{
				if (int.TryParse(coordinates[0].ToString(), out int x) && int.TryParse(coordinates[1].ToString(), out int y))
				{
					int size = (int) Math.Sqrt(path.Length);
					if (x < size && y < size)
						index = Math.Min(path[x + y * size], c_colors.Length - 1);
				}
			}

			return c_colors[index];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		private readonly Color[] c_colors =
		{
			Colors.Ivory,
			Colors.BurlyWood,
			Colors.IndianRed,
			Colors.LightSalmon,
			Colors.Yellow,
			Colors.SpringGreen,
			Colors.LightSkyBlue,
			Colors.BlueViolet,
			Colors.Gainsboro
		};
	}
}
