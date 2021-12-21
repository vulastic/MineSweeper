using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
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

		public ICommand TestCommand { get; }

		public GameViewModel()
		{
			TestCommand = new RelayCommand(testc);
		}

		private void testc()
		{
			Random rand = new Random();

			MineCount = (int)(rand.Next() % 10000);
			PlayTime = rand.Next();
		}
	}
}
