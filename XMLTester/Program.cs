using System;
using System.Collections.Generic;
using MineSweeper.Models;

namespace XMLTester
{
	class Program
	{
		static void Main(string[] args)
		{
			// Test Case 01 (Missing attribute field)
			{
				List<Tile> tiles;
				int width, height;
				if (!XmlHelper.GetFieldFromXML("minefield_1.xml", out width, out height, out tiles))
				{
					Console.WriteLine("Filt to read 'minefiled_1.xml' file.");
				}
			}

			// Test Case 02 (Invalid datatype of game)
			{
				List<Tile> tiles;
				int width, height;
				if (!XmlHelper.GetFieldFromXML("minefield_2.xml", out width, out height, out tiles))
				{
					Console.WriteLine("Filt to read 'minefiled_2.xml' file.");
				}
			}

			// Test Case 03 (Empty body about field)
			{
				List<Tile> tiles;
				int width, height;
				if (!XmlHelper.GetFieldFromXML("minefield_3.xml", out width, out height, out tiles))
				{
					Console.WriteLine("Filt to read 'minefiled_3.xml' file.");
				}
			}

			// Test Case 04 (Game Missing element)
			{
				List<Log> logs = new();
				if (!XmlHelper.GetPlayFromXML("game_1.xml", out logs))
				{
					Console.WriteLine("Fail to read game_1.xml");
				}
			}

			// Test Case 05 (Invalid datatype of game)
			{
				List<Log> logs = new();
				if (!XmlHelper.GetPlayFromXML("game_2.xml", out logs))
				{
					Console.WriteLine("Fail to read game_2.xml");
				}
			}

			// Test Case 06 (Empty body about game)
			{
				List<Log> logs = new();
				if (!XmlHelper.GetPlayFromXML("game_3.xml", out logs))
				{
					Console.WriteLine("Fail to read game_3.xml");
				}
			}





		}
	}
}
