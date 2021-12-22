using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MineSweeper.ViewModels
{
	class GameViewModel : ObservableRecipient
	{
		#region Play Game
		private bool isLoadField = false;
		public bool IsLoadField
		{
			get => isLoadField;
			set
			{
				isLoadField = value;
				this.OnPropertyChanged("IsReplayEnable");
			}
		}

		private bool isLoadGame = false;
		public bool IsLoadGame
		{
			get => isLoadGame;
			set
			{
				isLoadGame = value;
				OnPropertyChanged("IsReplayEnable");
			}
		}

		public bool IsReplayEnable { get => isLoadField & isLoadGame; }

		public ICommand LoadField { get; }
		public ICommand LoadGame { get; }
		public ICommand ReplayGame { get; }
		#endregion

		#region Set Fields
		public ICommand ResetGame { get; }
		public ICommand AutoPopulate { get; }
		public ICommand SetFieldToXML { get; }
		#endregion

		private Models.MineSweeper game = new Models.MineSweeper();
		public Models.MineSweeper Game => game;

		public int Width => Game.Width * 24;

		public ICommand LeftClick { get; }
		public ICommand RightClick { get; }
		public ICommand MouseRelease { get; }

		private ObservableCollection<Tile> gameTiles = new();
		public ObservableCollection<Tile> GameTiles
		{
			get => gameTiles;
			set
			{
				gameTiles = value;
				OnPropertyChanging("GameTiles");
			}
		}

		private bool isEditMode = false;
		public bool IsEditMode 
		{ 
			get => isEditMode;
			set
			{
				isEditMode = value;
				OnPropertyChanged("IsEditMode");
			}
		}

		private int logIndex = 0;
		private List<Log> gameLogs = new List<Log>();

		public GameViewModel()
		{
			// Mouse Event
			LeftClick = new RelayCommand<object>(LeftClickEvent);
			RightClick = new RelayCommand<object>(RightClickEvent);
			MouseRelease = new RelayCommand<object>(MouseReleaseEvent);

			// Play Game
			LoadField = new RelayCommand<object>(LoadFieldEvent);
			LoadGame = new RelayCommand<object>(LoadGameEvent);
			ReplayGame = new RelayCommand<object>(ReplayGameEvent);

			// Set Field
			ResetGame = new RelayCommand<object>(ResetGameEvent);
			AutoPopulate = new RelayCommand<object>(AutoPopulateEvent);
			SetFieldToXML = new RelayCommand<object>(SetFieldToXMLEvent);

			// Set Default Tiles
			game.Init(16, 16, true);
			game.AutoGenerate(40);
			game.Map.ForEach(x => GameTiles.Add(x));
		}

		private void LeftClickEvent(object sender)
		{
			Tile tile = sender as Tile;
			if (tile == null)
			{
				return;
			}

			// Edit Mode
			if (IsEditMode)
			{
				game.SetMine(tile.X, tile.Y);
				return;
			}

			// Game Mode
			game.PressTile(tile.X, tile.Y);

			// Play
			if (game.State == 1)
			{
				gameLogs.Add(new Log()
				{
					X = tile.X,
					Y = tile.Y,
					Time = game.Time,
					User = "user",
					Sign = Log.PlaySign.Click
				});
			}
		}

		private void RightClickEvent(object sender)
		{
			Tile tile = sender as Tile;
			if (tile == null)
			{
				return;
			}

			if (IsEditMode)
			{
				game.SetMine(tile.X, tile.Y);
				return;
			}

			game.SetFlag(tile.X, tile.Y);

			// Play
			if (game.State == 1)
			{
				gameLogs.Add(new Log()
				{
					X = tile.X,
					Y = tile.Y,
					Time = game.Time,
					User = "user",
					Sign = Log.PlaySign.SetFlag
				});
			}
			else if (game.State > 1)
			{
				// save last action
				gameLogs.Add(new Log()
				{
					X = tile.X,
					Y = tile.Y,
					Time = game.Time,
					User = "user",
					Sign = Log.PlaySign.SetFlag
				});

				MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to save your play?", "Save your play", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					// Save Play
					string outpath = "game.xml";
					bool isSuccess = XmlHelper.SetPlayToXML(outpath, this.gameLogs);
					if (isSuccess)
					{
						System.Windows.MessageBox.Show($"Success to create '{outpath}' file.", "Success!");
					}
				}
			}
		}

		private void MouseReleaseEvent(object sender)
		{
			game.ReleaseTile();

			// end game
			if (game.State > 1)
			{
				MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to save your play?", "Save your play", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					// Save Play
					// Save Play
					string outpath = "game.xml";
					bool isSuccess = XmlHelper.SetPlayToXML(outpath, this.gameLogs);
					if (isSuccess)
					{
						System.Windows.MessageBox.Show($"Success to create '{outpath}' file.", "Success!");
					}
				}
			}
		}

		private void LoadFieldEvent(object sender)
		{
			string inputpath = "minefield.xml";

			List<Tile> tiles;
			int width, height;
			bool isSuccess = XmlHelper.GetFieldFromXML(inputpath, out width, out height, out tiles);
			if (isSuccess)
			{
				System.Windows.MessageBox.Show($"'{inputpath}' load successful.", "Success!");
			}

			game.Init(width, height, true, tiles);

			GameTiles.Clear();
			game.Map.ForEach(x => GameTiles.Add(x));

			OnPropertyChanging("GameTiles");

			// set play mode
			this.IsEditMode = false;
		}

		private void LoadGameEvent(object sender)
		{
			logIndex = 0;
			gameLogs.Clear();

			string inputPath = "game.xml";
			bool isSuccess = XmlHelper.GetPlayFromXML(inputPath, out this.gameLogs);
			if (isSuccess)
			{
				System.Windows.MessageBox.Show($"'{inputPath}' load successful.", "Success!");
			}
		}

		private void ReplayGameEvent(object sender)
		{
			if (gameLogs.Count > logIndex)
			{
				Log log = gameLogs[logIndex++];

				if (log.Sign == Log.PlaySign.Click)
				{
					game.PressTile(log.X, log.Y);
					game.ReleaseTile();

				}
				else if (log.Sign == Log.PlaySign.SetFlag)
				{
					game.SetFlag(log.X, log.Y);
				}

				if (game.State > 1)
				{
					System.Windows.MessageBox.Show($"'Replay Completed.", "Success!");
					this.IsLoadField = false;
					this.IsLoadGame = false;
				}
			}
		}

		private void ResetGameEvent(object sender)
		{
			// disable edit mode
			this.IsEditMode = false;

			game.Init();
			game.AutoGenerate(40);

			gameLogs.Clear();
		}

		private void AutoPopulateEvent(object sender)
		{
			// set edit mode
			this.IsEditMode = true;

			//game.Init(16, 16, false);
			game.Init();
			game.AutoGenerate(40);
			game.OpenAllTiles();

			//GameTiles.Clear();
			//game.Map.ForEach(x => GameTiles.Add(x));
		}

		private void SetFieldToXMLEvent(object sender)
		{
			if (!IsEditMode)
			{
				System.Windows.MessageBox.Show($"You should go into set field mode.", "Ooops!");
				return;
			}

			string outpath = "minefield.xml";

			bool isSuccess = XmlHelper.SetFieldToXML(outpath, game.Width, game.Height, game.Map);

			if (isSuccess)
			{
				System.Windows.MessageBox.Show($"Success to create '{outpath}' file.", "Success!");
			}
			else
			{
				System.Windows.MessageBox.Show($"Cannot create '{outpath}' file.", "Ooops!");
			}
		}
	}
}
