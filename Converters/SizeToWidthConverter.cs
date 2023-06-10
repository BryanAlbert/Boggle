using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeToWidthConverter : IValueConverter
	{
		public string Type { get; set; }


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int size)
			{
				int index = Type switch
				{
					"Frame" => 0,
					"Entry" => 1,
					"Radius" => 2,
					"Padding" =>  3,
					"Margin" => 4,
					_ => 0
				};

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

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		private readonly double[] c_4x4 = { 392.0, 90.0, 20.0, 5.0, 3.0 };
		private readonly double[] c_5x5 = { 390.0, 73.0, 15.0, 4.0, 2.0 };
		private readonly double[] c_6x6 = { 386.0, 58.0, 10.0, 3.0, 1.0 };
	}
}
