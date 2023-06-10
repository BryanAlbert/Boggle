using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeAndSolvedToFontSizeConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values != null && values[0] is int size && values[1] is bool small)
			{
				return size switch
				{
					4 => c_4x4[small ? 0 : 1],
					5 => c_5x5[small ? 0 : 1],
					6 => c_6x6[small ? 0 : 1],
					_ => c_4x4[small ? 0 : 1]
				};
			}

			return c_4x4[1];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		private readonly double[] c_4x4 = { 14.0, 52.0 };
		private readonly double[] c_5x5 = { 12.0, 39.0 };
		private readonly double[] c_6x6 = { 12.0, 33.0 };
	}
}
