﻿using System.Globalization;

namespace Boggle.Converters
{
	internal class NullToBoolConverter : IValueConverter
	{
		public bool Not { get; set; }


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Not ? value != null : value == null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
