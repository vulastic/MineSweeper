using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Models
{
	class MineSweeper : ObservableObject
	{
		private enum TileCovered
		{
			Uncovered = 0x00,
			Covered = 0x10,
		}

		private enum TileValue
		{
			Empty = 0x00,
			One = 0x01,
			Two = 0x02,
			Three = 0x03,
			Four = 0x04,
			Five = 0x05,
			Six = 0x06,
			Seven = 0x07,
			Eight = 0x08,
			Flag = 0x09,
			Mine = 0x0A,
			MineHit = 0x0B
		}

		private int width = 0;
		public int Width => width;

		private int height = 0;
		public int Height => height;

		private int mine = 0;
		public int Mine
		{
			get => mine;
			set
			{
				mine = value;
				OnPropertyChanged("Mine");
			}
		}

		private long time = 0;
		public long Time
		{
			get => time;
			set
			{
				time = value;
				OnPropertyChanged("Time");
			}
		}

		private Tile[,] map;
		public List<Tile> Map => map.Cast<Tile>().ToList();

		public MineSweeper()
		{
			
		}

		public void Init(int width, int height, bool IsCovered)
		{
			this.width = width;
			this.height = height;

			// create new map
			map = new Tile[width, height];

			// fill the default
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					map[x, y] = new Tile()
					{
						X = x,
						Y = y,
						Status = IsCovered ? (int)TileCovered.Covered : (int)TileCovered.Uncovered
					};
				}
			}

			this.mine = 0;
			this.time = 0;
		}

		public void AutoGenerate(int mineCount = -1)
		{
			Random random = new Random();

			// under zero is random
			if (mineCount <= 0)
			{
				mineCount = random.Next(0, (width * height));
			}

			// fill the random mine
			while (mineCount > 0)
			{
				int x = random.Next(0, width);
				int y = random.Next(0, height);

				if ((map[x, y].Status & (int)TileValue.Mine) == 0)
				{
					map[x, y].Status |= (int)TileValue.Mine;

					--mineCount;
					++this.mine;
				}
			}

			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					CalculateField(x, y);
				}
			}
		}

		private void CalculateField(int x, int y)
		{
			if (x < 0 || width <= x || y < 0 || height <= y)
			{
				// out of range
				return;
			}

			if ((map[x, y].Status & (int)TileValue.Mine) > 0)
			{
				for (int j = y - 1; j < y + 1; ++j)
				{
					for (int i = x - 1; i < x + 1; ++i)
					{
						if (i < 0 || j < 0 || i >= width || j >= height)
						{
							continue;
						}

						if ((map[i, j].Status & (int)TileValue.Mine) == 0)
						{
							map[i, j].Status += 1;
						}
					}
				}
			}
		}
	}
}
