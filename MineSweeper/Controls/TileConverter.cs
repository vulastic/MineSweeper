using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MineSweeper.Controls
{
	class TileConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				int number = System.Convert.ToInt32(value);
				if (number < 12)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error: Exception = {ex.Message}");
			}
			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
