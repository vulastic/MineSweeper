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
		public enum TileValue
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

		private Tile selectedTile;

		public MineSweeper()
		{
			
		}

		public void Init(int width, int height, bool IsCovered)
		{
			this.Width = width;
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
			this.Mine = 0;
			this.Time = 0;
		}

		public void LeftClick(int x, int y)
		{
			if (x < 0 || y < 0 || x >= width || y >= height)
			{
				return;
			}

			selectedTile = map[x, y];
			if (selectedTile.IsFlag == true)
			{
				// cannot click the flag
				selectedTile = null;
				return;
			}

			// Hit Number Tile
			if (selectedTile.IsCovered == false && selectedTile.Status <= (int)TileValue.Eight)
			{
				// Searching eight direction. if there are not a mine, it will be opened.
				SearchMineAround(x, y, true);
			}

			else if (selectedTile.IsCovered == true)
			{
				selectedTile.IsPressed = true;
			}
		}

		public void RightClick(int x, int y )
		{
			if (x < 0 || y < 0 || x >= width || y >= height)
			{
				return;
			}

			selectedTile = map[x, y];
			if (selectedTile.IsFlag)
			{
				selectedTile.IsFlag = false;
			}
			else if (selectedTile.IsCovered == true)
			{
				selectedTile.IsFlag = true;
			}

			selectedTile = null;
		}

		public void ReleasePressed()
		{
			if (selectedTile == null)
			{
				return;
			}

			// Hit the mine
			if (selectedTile.Status == (int)TileValue.Mine)
			{
				selectedTile.IsCovered = false;
				selectedTile.Status = (int)TileValue.MineHit;
				OpenAllMine();
				return;
			}

			// Empty tile
			if (selectedTile.Status == (int)TileValue.Empty)
			{
				selectedTile.IsCovered = false;
				OpenEmptyTile(selectedTile.X, selectedTile.Y);
				return;
			}

			// Number Tile
			if (selectedTile.Status <= (int)TileValue.Eight)
			{
				// Searching eight direction. if there are not a mine, it will be opened.
				int count = SearchMineAround(selectedTile.X, selectedTile.Y, false);
				if (count == 0)
				{
					OpenAround(selectedTile.X, selectedTile.Y);
				}
			}

			selectedTile.IsPressed = false;
			selectedTile.IsCovered = false;
		}

		private void OpenAllMine()
		{
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					Tile tile = map[x, y];
					if (tile.Status == (int)TileValue.Mine)
					{
						map[x, y].IsCovered = false;
					}
				}
			}
		}

		private void OpenEmptyTile(int x, int y)
		{
			if (x < 0 || width <= x || y < 0 || height <= y)
			{
				// out of range
				return;
			}

			if (map[x, y].Status == (int)TileValue.Empty)
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

						Tile tile = map[i, j];
						if (tile.IsFlag == true || tile.IsCovered == false)
						{
							continue;
						}

						if (tile.Status == (int)TileValue.Mine || tile.Status == (int)TileValue.MineHit)
						{
							continue;
						}

						tile.IsCovered = false;
						OpenEmptyTile(i, j);
					}	
				}
			}
		}

		private int SearchMineAround(int x, int y, bool isPressed = false)
		{
			int mineCount = 0;
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

					Tile tile = map[i, j];
					if (tile.IsFlag == true || tile.IsCovered == false)
					{
						continue;
					}

					if (tile.IsCovered == true)
					{
						tile.IsPressed = isPressed;
					}
					
					if (tile.Status == (int)TileValue.Mine)
					{
						++mineCount;
					}
				}
			}
			return mineCount;
		}

		private void OpenAround(int x, int y)
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

					Tile tile = map[i, j];
					if (tile.IsFlag == true || tile.IsCovered == false)
					{
						continue;
					}

					tile.IsCovered = false;
				}
			}
		}

		public void SetField(int width, int height, bool IsCovered, List<Tile> tiles)
		{
			this.Mine = 0;
			this.Time = 0;

			map = new Tile[width, height];

			foreach (Tile tile in tiles)
			{
				tile.IsCovered = IsCovered;
				map[tile.X, tile.Y] = tile;
				if (tile.Status == (int)TileValue.Mine)
				{
					++this.Mine;
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
					++this.Mine;
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

		private void ReCalculateField(int x, int y)
		{
			if (x < 0 || width <= x || y < 0 || height <= y)
			{
				// out of range
				return;
			}

			int mineCount = 0;
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
						map[i, j].Status -= 1;
					}
					else
					{
						++mineCount;
					}
				}
			}
			map[x, y].Status = mineCount;
		}

		public void SetMine(int x, int y)
		{
			if (map[x, y].Status == (int)TileValue.Mine)
			{
				map[x, y].Status = (int)TileValue.Empty;

				ReCalculateField(x, y);
				--this.Mine;
			}
			else if (map[x, y].Status != (int)TileValue.Mine)
			{
				map[x, y].Status = (int)TileValue.Mine;

				CalculateField(x, y);
				++this.Mine;
			}
		}
	}
}
