using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MineSweeper.ViewModels
{
	class GameViewModel : ObservableRecipient
	{
		private int mineCount;
		public int MineCount
		{
			get => mineCount;
			set
			{
				mineCount = value;
				OnPropertyChanged("MineCount");
			}
		}

		private int playTime;
		public int PlayTime
		{
			get => playTime;
			set
			{
				playTime = value;
				OnPropertyChanged("PlayTime");
			}
		}

		public int WidthCount { get => (int)Math.Sqrt(GameTiles.Count) * 24; }

		public ICommand TestCommand { get; }

		public ObservableCollection<Tile> GameTiles { get; set; }

		public GameViewModel()
		{
			TestCommand = new RelayCommand<object>(testc);

			GameTiles = new ObservableCollection<Tile>();

			for(int i = 0; i < 16; ++i)
			{
				for (int j = 0; j < 16; ++j)
				{
					GameTiles.Add(new Tile()
					{
						X = i,
						Y = j,
						Status = 12
					});
				}
			}
			OnPropertyChanged("GameTiles");
		}

		private void testc(object sender)
		{
			Tile tile = sender as Tile;
			tile.Status = 1;

			OnPropertyChanged("GameTiles");
		}
	}
}
