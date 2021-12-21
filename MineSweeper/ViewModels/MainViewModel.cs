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
	class MainViewModel : ObservableRecipient
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

		#region Set Field
		public bool IsAutoPopulate { get; set; } = false;

		public ICommand SetField { get; } = new RelayCommand(() =>
		{
			Console.WriteLine("aa");
		});
		#endregion

		public ObservableRecipient GameViewModel { get; } = new GameViewModel();

		public MainViewModel()
		{
			
		}


	}
}
