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

		public ICommand PlayGame { get; } = new RelayCommand(() =>
		{
			Console.WriteLine("aa");
		});
		#endregion

		#region Set Fields
		public ICommand AutoPopulate { get; }
		public ICommand SetFieldToXML { get; }
		#endregion

		private Models.MineSweeper game = new Models.MineSweeper();
		public Models.MineSweeper Game => game;

		public int Width => Game.Width * 24;

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
				OnPropertyChanging("GameTiles");
			}
		}

		public GameViewModel()
		{
			LeftClickCommand = new RelayCommand<object>(LeftClickEvent);
			RightClickCommand = new RelayCommand<object>(RightClickEvent);
			MiddleClickCommand = new RelayCommand<object>(MiddleClickEvent);

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
			
		}

		private void RightClickEvent(object sender)
		{
			
		}

		private void MiddleClickEvent(object sender)
		{
			
		}

		private void AutoPopulateEvent(object sender)
		{
			game.Init(16, 16, false);
			game.AutoGenerate(40);

			GameTiles.Clear();
			game.Map.ForEach(x => GameTiles.Add(x));
		}

		private void SetFieldToXMLEvent(object sender)
		{

		}
	}
}
