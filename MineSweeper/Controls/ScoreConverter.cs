using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Diagnostics;

namespace MineSweeper.Controls
{
	class ScoreConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				int number = (int)value;
				switch(parameter)
				{
					case "Hundreds":
						return number % 1000 / 100;

					case "Tens":
						return number % 100 / 10;

					case "Units":
						return number % 10;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Warning: Invalid value. Value = {value}, Parameter = {parameter}, Exception = {ex.Message}");
			}
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
