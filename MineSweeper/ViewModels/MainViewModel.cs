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
		public ICommand AutoPopulate { get; }
		public ICommand SetFieldToXML { get; }
		#endregion

		public GameViewModel GameViewModel { get; } = new GameViewModel();

		public MainViewModel()
		{
			this.AutoPopulate = new RelayCommand(AutoPopulateEvent);
			this.SetFieldToXML = new RelayCommand(SetFieldToXMLEvent);
		}

		private void AutoPopulateEvent()
		{
			GameViewModel.AutoPopulate();
		}

		private void SetFieldToXMLEvent()
		{
			GameViewModel.SetFieldToXML();
		}


	}
}
