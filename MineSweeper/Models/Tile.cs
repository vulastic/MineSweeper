using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Models
{	
	class Tile : ObservableObject
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Value { get; set; }

		private int display = 0;
		public int Display
		{
			get => display;
			set
			{
				display = value;
				OnPropertyChanged("Display");
			}
		}
	}
}
