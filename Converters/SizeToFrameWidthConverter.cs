using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeToFrameWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int result = c_default;
			if (value is int size)
			{
				result = size switch
				{
					4 => c_4x4,
					5 => c_5x5,
					6 => c_6x6,
					_ => c_default
				};
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		// TODO: obviously this converter isn't doing much... but maybe it would be useful on different\
		// devices if we were checking the width or something...
		private const int c_4x4 = 390;
		private const int c_5x5 = 390;
		private const int c_6x6 = 390;
		private const int c_default = 390;
	}
}
