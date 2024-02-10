using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeAndSolvedToWidthConverter : IMultiValueConverter
	{
		public string Type { get; set; }


		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values != null && values[0] is int size && values[1] is bool small)
			{
				int index = Type switch
				{
					"Frame" => 0,
					"Radius" => 1,
					"Padding" => 2,
					"Margin" => 3,
					_ => 0
				};

				if (small)
				{
					return size switch
					{
						4 => c_small4x4[index],
						5 => c_small5x5[index],
						6 => c_small6x6[index],
						_ => c_small4x4[index]
					};
				}

				return size switch
				{
					4 => c_4x4[index],
					5 => c_5x5[index],
					6 => c_6x6[index],
					_ => c_4x4[index]
				};
			}

			return c_4x4[0];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		private readonly double[] c_small4x4 = [109.0, 5.0, 2.0, 0.5];
		private readonly double[] c_small5x5 = [123.0, 3.0, 1.0, 0.5];
		private readonly double[] c_small6x6 = [135.0, 3.0, 1.0, 0.4];
		private readonly double[] c_4x4 = [392.0, 20.0, 5.0, 3.0];
		private readonly double[] c_5x5 = [390.0, 15.0, 4.0, 2.0];
		private readonly double[] c_6x6 = [386.0, 10.0, 3.0, 1.0];
	}
}
