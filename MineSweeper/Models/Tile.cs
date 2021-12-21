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

		private int status;
		public int Status
		{
			get => status;
			set
			{
				status = value;
				OnPropertyChanged("Status");
			}
		}
	}
}
