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
		private enum TileValue
		{
			Empty = 0,
			One = 1,
			Two = 2,
			Three = 3,
			Four = 4,
			Five = 5,
			Six = 6,
			Seven = 7,
			Eight = 8,
			Mine = 9,
			MineHit = 10
		}

		private int width = 0;
		public int Width
		{
			get => width;
			set
			{
				width = value;
				OnPropertyChanged("Width");
			}
		}

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
						Status = 0,
						IsCovered = IsCovered
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

				if (map[x, y].Status != (int)TileValue.Mine)
				{
					map[x, y].Status = (int)TileValue.Mine;

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

			if (map[x, y].Status == (int)TileValue.Mine)
			{
				for (int j = y - 1; j < y + 2; ++j)
				{
					for (int i = x - 1; i < x + 2; ++i)
					{
						if (i < 0 || j < 0 || i >= width || j >= height)
						{
							continue;
						}

						if (i == x && j == y)
						{
							continue;
						}

						if (map[i, j].Status != (int)TileValue.Mine)
						{
							map[i, j].Status += 1;
						}
					}
				}
			}
		}
	}
}
