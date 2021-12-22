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
		public GameViewModel GameViewModel { get; } = new GameViewModel();

		public MainViewModel()
		{
		}
	}
}
