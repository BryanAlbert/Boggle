using System.Globalization;

namespace Boggle.Converters
{
	internal class SizeToGridDefinitionsConverter : IValueConverter
	{
		public bool GetColumnDefinition { get; set; }


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int size)
			{
				if (GetColumnDefinition)
				{
					ColumnDefinition column = size switch
					{
						4 => new(c_4x4),
						5 => new(c_5x5),
						6 => new(c_6x6),
						_ => new(c_4x4)
					};

					ColumnDefinition[] columns = [column, column, column, column, column, column];
					return new ColumnDefinitionCollection(columns);
				}

				RowDefinition row = size switch
				{
					4 => new(c_4x4),
					5 => new(c_5x5),
					6 => new(c_6x6),
					_ => new(c_4x4)
				};

				RowDefinition[] rows = [row, row, row, row, row, row];
				return new RowDefinitionCollection(rows);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		private readonly double c_4x4 = 95.0;
		private readonly double c_5x5 = 76.0;
		private readonly double c_6x6 = 63.0;
	}
}
