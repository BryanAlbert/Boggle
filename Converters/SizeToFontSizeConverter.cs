using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeToFontSizeConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is int size)
			{
				return size switch
				{
					4 => c_4x4,
					5 => c_5x5,
					6 => c_6x6,
					_ => c_4x4
				};
			}

			return c_4x4;
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
			throw new NotImplementedException();


		private readonly double c_4x4 = 52.0;
		private readonly double c_5x5 = 39.0;
		private readonly double c_6x6 = 33.0;
	}
}
