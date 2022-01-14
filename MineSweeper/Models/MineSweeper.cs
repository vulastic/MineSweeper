using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

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
			MineHit = 10,
			Flag = 11,
			FlagWrong = 12,
			Closed = 13,
			Pressed = 14
		}

		public enum GameState
		{
			None,
			Play,
			Win,
			Lose
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

		private GameState state = GameState.Play;
		public int State
		{
			get => (int)state;
			set
			{
				state = (GameState)value;
				OnPropertyChanged("State");
			}
		}


		private Tile[,] map;
		public List<Tile> Map => map.Cast<Tile>().ToList();

		private Tile selected;

		private DispatcherTimer timer;

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
						Value = (int)TileValue.Empty,
						Display = IsCovered ? (int)TileValue.Closed : (int)TileValue.Empty
					};
				}
			}
			this.Mine = 0;
			this.Time = 0;
			this.State = (int)GameState.None;
		}

		private void WinGame()
		{
			if (state == GameState.Play)
			{
				State = (int)GameState.Win;

				// Stop Timer
				timer.Stop();
			}
		}

		private void LoseGame()
		{
			if (state == GameState.Play)
			{
				State = (int)GameState.Lose;

				// Stop Timer
				timer.Stop();
			}
		}

		private void PlayGame()
		{
			if (state == GameState.None)
			{
				State = (int)GameState.Play;

				// 타이머 시작
				timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromMilliseconds(1000);
				timer.Tick += new EventHandler(UpdateTimer);
				timer.Start();
			}
		}

		public void PressTile(int x, int y)
		{
			PlayGame();

			if (state != GameState.Play)
			{
				return;
			}

			if (x < 0 || y < 0 || x >= width || y >= height)
			{
				return;
			}

			selected = map[x, y];
			if (selected.Display == (int)TileValue.Flag)
			{
				// cannot press the flag
				selected = null;
				return;
			}

			// press opened number tile
			if (selected.Display <= (int)TileValue.Eight)
			{
				// Press around 
				SetPressAround(x, y);
				return;
			}

			// pass other unclosed tile
			if (selected.Display != (int)TileValue.Closed)
			{
				selected = null;
				return;
			}

			// Press
			selected.Display = (int)TileValue.Pressed;
		}

		private void SetPressAround(int x, int y)
		{
			for (int j = y - 1; j < y + 2; ++j)
			{
				for (int i = x - 1; i < x + 2; ++i)
				{
					if (i < 0 || j < 0 || i >= width || j >= height)
					{
						continue;
					}

					Tile tile = map[i, j];
					if (tile.Display == (int)TileValue.Closed)
					{
						tile.Display = (int)TileValue.Pressed;
					}
				}
			}
		}

		public void SetFlag(int x, int y )
		{
			PlayGame();

			if (state != GameState.Play)
			{
				return;
			}

			if (x < 0 || y < 0 || x >= width || y >= height)
			{
				return;
			}

			selected = map[x, y];
			if (selected.Display == (int)TileValue.Flag)
			{
				selected.Display = (int)TileValue.Closed;
				++this.Mine;
			}
			else if(selected.Display == (int)TileValue.Closed)
			{
				selected.Display = (int)TileValue.Flag;
				--this.Mine;
			}
			selected = null;

			// Check Win
			if (this.Mine == 0)
			{
				int count = 0;
				for (int j = 0; j < height; ++j)
				{
					for (int i = 0; i < width; ++i)
					{
						Tile tile = map[i, j];
						if (tile.Value == (int)TileValue.Mine && tile.Display != (int)TileValue.Flag)
						{
							++count;
						}
					}
				}

				if (count == 0)
				{
					WinGame();
				}
			}
		}

		public void ReleaseTile()
		{
			if (state != GameState.Play)
			{
				return;
			}

			if (selected == null)
			{
				return;
			}

			// Hit the mine
			if (selected.Value == (int)TileValue.Mine)
			{
				OpenAllMine(selected.X, selected.Y);
				selected = null;
				LoseGame();
				return;
			}

			// Empty tile
			if (selected.Value == (int)TileValue.Empty)
			{
				OpenEmptyTile(selected.X, selected.Y);
				selected = null;
				return;
			}

			// Number Tile
			if (selected.Value <= (int)TileValue.Eight)
			{
				// Searching eight direction. if there are not a mine, it will be opened.
				List<Tile> pressed, mines;
				GetPressAround(selected.X, selected.Y, out pressed, out mines);
				if (mines.Count == 1 && mines.Count == pressed.Count)
				{
					// gameover
					OpenAllMine(mines[0].X, mines[0].Y);
					selected = null;
					LoseGame();
					return;
				}

				// Open tile
				if (mines.Count == 0)
				{
					foreach (Tile tile in pressed)
					{
						tile.Display = tile.Value;
					}
				}

				// Rollback tile
				else
				{
					foreach (Tile tile in pressed)
					{
						tile.Display = (int)TileValue.Closed;
					}
				}
			}

			// others
			selected.Display = selected.Value;
			selected = null;


		}

		public void OpenAllTiles()
		{
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					Tile tile = map[x, y];
					tile.Display = tile.Value;
				}
			}
		}

		private void OpenAllMine(int x, int y)
		{
			for (int j = 0; j < height; ++j)
			{
				for (int i = 0; i < width; ++i)
				{
					Tile tile = map[i, j];
					if (tile.Value == (int)TileValue.Mine)
					{
						if (i == x && j == y)
						{
							tile.Display = (int)TileValue.MineHit;
						}
						else if (tile.Display != (int)TileValue.Flag)
						{
							tile.Display = (int)TileValue.Mine;
						}
					}
					else if (tile.Display == (int)TileValue.Flag)
					{
						tile.Display = (int)TileValue.FlagWrong;
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

			if (map[x, y].Value == (int)TileValue.Empty)
			{
				// set self
				map[x, y].Display = map[x, y].Value;

				// Open around
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
						if (tile.Display != (int)TileValue.Closed)
						{
							continue;
						}

						if (tile.Value == (int)TileValue.Mine)
						{
							continue;
						}

						tile.Display = tile.Value;
						OpenEmptyTile(i, j);
					}	
				}
			}
		}

		private void GetPressAround(int x, int y, out List<Tile> pressed, out List<Tile> mines)
		{
			mines = new();
			pressed = new();
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
					if(tile.Display == (int)TileValue.Pressed)
					{
						pressed.Add(tile);

						if (tile.Value == (int)TileValue.Mine)
						{
							mines.Add(tile);
						}
					}
				}
			}
		}

		public void Init(int width, int height, bool IsCovered, List<Tile> tiles)
		{
			this.Mine = 0;
			this.Time = 0;
			this.State = (int)GameState.None;

			map = new Tile[width, height];

			// fill mine
			foreach (Tile tile in tiles)
			{
				map[tile.X, tile.Y] = tile;
				if (tile.Value == (int)TileValue.Mine)
				{
					++this.Mine;
				}
			}

			// fill others
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					if (map[x, y] == null)
					{
						map[x, y] = new Tile()
						{
							X = x,
							Y = y,
							Value = (int)TileValue.Empty,
							Display = IsCovered ? (int)TileValue.Closed : (int)TileValue.Empty
						};
					}
				}
			}

			// calculating
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					CalculateField(x, y);
				}
			}

			// update display value
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					Tile tile = map[x, y];
					if (IsCovered)
					{
						tile.Display = (int)TileValue.Closed;
					}
					else
					{
						tile.Display = tile.Value;
					}
				}
			}
		}

		public void Init()
		{
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					map[x, y].Value = 0;
					map[x, y].Display = (int)TileValue.Closed;
				}
			}
		}

		public void AutoGenerate(int mineCount = -1)
		{
			this.Mine = 0;
			this.Time = 0;
			this.State = (int)GameState.None;

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

				if (map[x, y].Value != (int)TileValue.Mine)
				{
					map[x, y].Value = (int)TileValue.Mine;

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

			if (map[x, y].Value == (int)TileValue.Mine)
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

						if (map[i, j].Value != (int)TileValue.Mine)
						{
							map[i, j].Value += 1;
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

					if (map[i, j].Value != (int)TileValue.Mine)
					{
						map[i, j].Value -= 1;
					}
					else
					{
						++mineCount;
					}
				}
			}
			map[x, y].Value = mineCount;
		}

		public void SetMine(int x, int y)
		{
			if (map[x, y].Value == (int)TileValue.Mine)
			{
				map[x, y].Value = (int)TileValue.Empty;

				ReCalculateField(x, y);
				--this.Mine;
			}
			else if (map[x, y].Value != (int)TileValue.Mine)
			{
				map[x, y].Value = (int)TileValue.Mine;

				CalculateField(x, y);
				++this.Mine;
			}

			// Display
			for (int j = y - 1; j < y + 2; ++j)
			{
				for (int i = x - 1; i < x + 2; ++i)
				{
					if (i < 0 || j < 0 || i >= width || j >= height)
					{
						continue;
					}

					map[i, j].Display = map[i, j].Value;
				}
			}
		}

		private void UpdateTimer(Object sender, EventArgs e)
		{
			this.Time += 1;
		}
	}
}
