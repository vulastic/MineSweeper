using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MineSweeper.Controls
{
	/// <summary>
	/// Interaction logic for GameTile.xaml
	/// </summary>
	public partial class GameTile : UserControl
	{
		public static readonly DependencyProperty XProperty = 
			DependencyProperty.Register("X", typeof(int), typeof(GameTile), new PropertyMetadata(0));

		public int X
		{
			get => (int)GetValue(XProperty);
			set
			{
				SetValue(XProperty, value);
			}
		}

		public static readonly DependencyProperty YProperty = 
			DependencyProperty.Register("Y", typeof(int), typeof(GameTile), new PropertyMetadata(0));

		public int Y
		{
			get => (int)GetValue(YProperty);
			set
			{
				SetValue(YProperty, value);
			}
		}

		public static readonly DependencyProperty LeftClickProperty =
			DependencyProperty.Register("LeftClick", typeof(ICommand), typeof(GameTile), new UIPropertyMetadata(null));

		public ICommand LeftClick
		{
			get => (ICommand)GetValue(LeftClickProperty);
			set 
			{
				SetValue(LeftClickProperty, value); 
			}
		}

		public static readonly DependencyProperty RightClickProperty =
			DependencyProperty.Register("RightClick", typeof(ICommand), typeof(GameTile), new UIPropertyMetadata(null));

		public ICommand RightClick
		{
			get => (ICommand)GetValue(RightClickProperty);
			set
			{
				SetValue(RightClickProperty, value);
			}
		}

		public static readonly DependencyProperty MiddleClickProperty =
			DependencyProperty.Register("MiddleClick", typeof(ICommand), typeof(GameTile), new UIPropertyMetadata(null));

		public ICommand MiddleClick
		{
			get => (ICommand)GetValue(MiddleClickProperty);
			set
			{
				SetValue(MiddleClickProperty, value);
			}
		}

		public static readonly DependencyProperty CommandParameterProperty = 
			DependencyProperty.Register("CommandParameter", typeof(object), typeof(GameTile), new UIPropertyMetadata(null));

		public object CommandParameter
		{
			get => (object)GetValue(CommandParameterProperty);
			set
			{ 
				SetValue(CommandParameterProperty, value); 
			}
		}

		public GameTile()
		{
			InitializeComponent();
		}
	}
}
