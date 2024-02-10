using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeAndSolvedToGridDefinitionsConverter : IMultiValueConverter
	{
		public bool GetColumnDefinition { get; set; }


		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values[0] is not int || values[1] is not bool)
				values = [4, false];

			int size = (int) values[0];
			bool small = (bool) values[1];
			if (GetColumnDefinition)
			{
				ColumnDefinition column = size switch
				{
					4 => new ColumnDefinition(c_4x4[small ? 0 : 1]),
					5 => new ColumnDefinition(c_5x5[small ? 0 : 1]),
					6 => new ColumnDefinition(c_6x6[small ? 0 : 1]),
					_ => new ColumnDefinition(c_4x4[small ? 0 : 1])
				};

				return new ColumnDefinitionCollection([column, column, column, column, column, column]);
			}

			RowDefinition row = size switch
			{
				4 => new RowDefinition(c_4x4[small ? 0 : 1]),
				5 => new RowDefinition(c_5x5[small ? 0 : 1]),
				6 => new RowDefinition(c_6x6[small ? 0 : 1]),
				_ => new RowDefinition(c_4x4[small ? 0 : 1])
			};

			return new RowDefinitionCollection([row, row, row, row, row, row]);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		private readonly double[] c_4x4 = [26.0, 95.0];
		private readonly double[] c_5x5 = [24.0, 76.0];
		private readonly double[] c_6x6 = [22.0, 63.0];
	}
}
