using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeToGridDefinitionsConverter : IValueConverter
	{
		public bool GetColumnDefinition { get; set; }
		public bool Big { get; set; }


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int size)
			{
				int index = Big ? 0 : 1;
				if (GetColumnDefinition)
				{
					ColumnDefinition column = size switch
					{
						4 => new ColumnDefinition(c_4x4[index]),
						5 => new ColumnDefinition(c_5x5[index]),
						6 => new ColumnDefinition(c_6x6[index]),
						_ => new ColumnDefinition(c_4x4[index])
					};

					ColumnDefinition[] columns = { column, column, column, column, column, column };
					return new ColumnDefinitionCollection(columns);
				}

				RowDefinition row = size switch
				{
					4 => new RowDefinition(c_4x4[index]),
					5 => new RowDefinition(c_5x5[index]),
					6 => new RowDefinition(c_6x6[index]),
					_ => new RowDefinition(c_4x4[index])
				};

				RowDefinition[] rows = { row, row, row, row, row, row };
				return new RowDefinitionCollection(rows);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		private readonly int[] c_4x4 = { 95, 24 };
		private readonly int[] c_5x5 = { 76, 22 };
		private readonly int[] c_6x6 = { 63, 22 };
	}
}
