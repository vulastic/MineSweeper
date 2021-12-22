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
			if (value == null)
			{
				return 0;
			}

			try
			{
				string number = value.ToString();
				switch(parameter)
				{
					case "Hundreds":
						return number.Substring(number.Length - 3, 1);

					case "Tens":
						return number.Substring(number.Length - 2, 1);

					case "Units":
						return number.Substring(number.Length - 1, 1);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Warning: Invalid value. Value = {value}, Parameter = {parameter}, Exception = {ex.Message}");
			}
			return "0";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
