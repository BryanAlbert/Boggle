using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeToWidthConverter : IValueConverter
	{
		public bool Big { get; set; }
		public bool Entry { get; set; }


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int index = Entry ? 2 : Big ? 0 : 1;
			if (value is int size)
			{
				return size switch
				{
					4 => c_4x4[index],
					5 => c_5x5[index],
					6 => c_6x6[index],
					_ => c_4x4[index]
				};
			}

			return c_4x4[index];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		private readonly int[] c_4x4 = { 392, 103, 90 };
		private readonly int[] c_5x5 = { 390, 117, 73 };
		private readonly int[] c_6x6 = { 390, 139, 58 };
	}
}
