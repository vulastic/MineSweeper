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
				this.OnPropertyChanged("IsGameEnabled");
			}
		}

		private bool isLoadGame = false;
		public bool IsLoadGame
		{
			get => isLoadGame;
			set
			{
				isLoadGame = value;
				OnPropertyChanged("IsGameEnabled");
			}
		}

		public bool IsGameEnabled { get => isLoadField | isLoadGame; }

		public ICommand LoadField { get; }
		public ICommand LoadGame { get; }
		public ICommand PlayGame { get; }
		#endregion

		#region Set Fields
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

		public GameViewModel()
		{
			LeftClick = new RelayCommand<object>(LeftClickEvent);
			RightClick = new RelayCommand<object>(RightClickEvent);
			MouseRelease = new RelayCommand<object>(MouseReleaseEvent);

			// Play Game
			LoadField = new RelayCommand<object>(LoadFieldEvent);
			LoadGame = new RelayCommand<object>(LoadGameEvent);
			PlayGame = new RelayCommand<object>(PlayGameEvent);

			// Set Field
			AutoPopulate = new RelayCommand<object>(AutoPopulateEvent);
			SetFieldToXML = new RelayCommand<object>(SetFieldToXMLEvent);

			// Default
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
			game.LeftClick(tile.X, tile.Y);
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

			game.RightClick(tile.X, tile.Y);
		}

		private void MouseReleaseEvent(object sender)
		{
			game.ReleasePressed();
		}

		private void LoadFieldEvent(object sender)
		{
			string inputpath = "minefield.xml";

			List<Tile> tiles;
			int width, height;
			bool isSuccess = XmlHelper.GetFieldToXML(inputpath, out width, out height, out tiles);

			game.SetField(width, height, true, tiles);

			GameTiles.Clear();
			game.Map.ForEach(x => GameTiles.Add(x));

			// set play mode
			this.IsEditMode = false;
		}

		private void LoadGameEvent(object sender)
		{

		}

		private void PlayGameEvent(object sender)
		{

		}

		private void AutoPopulateEvent(object sender)
		{
			game.Init(16, 16, false);
			game.AutoGenerate(40);

			GameTiles.Clear();
			game.Map.ForEach(x => GameTiles.Add(x));

			// set edit mode
			this.IsEditMode = true;
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
