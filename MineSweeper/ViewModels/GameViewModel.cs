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
		private Models.MineSweeper game = new Models.MineSweeper();
		public Models.MineSweeper Game => game;

		public int ControlWidth => Game.Width * 24;

		public ICommand LeftClickCommand { get; }
		public ICommand RightClickCommand { get; }
		public ICommand MiddleClickCommand { get; }

		private ObservableCollection<Tile> gameTiles = new();
		public ObservableCollection<Tile> GameTiles
		{
			get => gameTiles;
			set
			{
				gameTiles = value;
				OnPropertyChanging("GameTIles");
			}
		}

		public GameViewModel()
		{
			LeftClickCommand = new RelayCommand<object>(LeftClickEvent);
			RightClickCommand = new RelayCommand<object>(RightClickEvent);
			MiddleClickCommand = new RelayCommand<object>(MiddleClickEvent);

			// Default
			game.Init(16, 16, true);

			game.Map.ForEach(x => GameTiles.Add(x));
		}

		private void LeftClickEvent(object sender)
		{
			Tile tile = sender as Tile;
			tile.Status = 1;

			OnPropertyChanged("GameTiles");
		}

		private void RightClickEvent(object sender)
		{
			Tile tile = sender as Tile;
			tile.Status = 9;

			OnPropertyChanged("GameTiles");
		}

		private void MiddleClickEvent(object sender)
		{
			Tile tile = sender as Tile;
			tile.Status = 1;

			OnPropertyChanged("GameTiles");
		}

		public void AutoPopulate()
		{
			game.Init(40, 40, false);
			game.AutoGenerate(40);

			GameTiles.Clear();
			GameTiles = new ObservableCollection<Tile>();
			game.Map.ForEach(x => GameTiles.Add(x));
		}

		public void SetFieldToXML()
		{

		}
	}
}
