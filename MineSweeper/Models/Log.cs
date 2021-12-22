using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Models
{
    class Log
    {
		public enum PlaySign
		{
			Click,
			SetFlag
		}

		public long Time { get; set; }
		public string User { get; set; }
		public PlaySign Sign { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
    }
}
