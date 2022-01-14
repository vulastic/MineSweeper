using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MineSweeper.Models
{
	class XmlHelper
	{
		public static bool SetFieldToXML(string fileName, int width, int height, List<Tile> tiles)
		{
			if (tiles == null)
			{
				return false;
			}

			StringWriter stringWriter = new StringWriter();
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace,
				OmitXmlDeclaration = false
			};

			using (XmlWriter writer = XmlWriter.Create(fileName, settings))
			{
				writer.WriteStartDocument();

				// Field infomation
				writer.WriteStartElement("Fields");
				writer.WriteAttributeString("width", width.ToString());
				writer.WriteAttributeString("height", height.ToString());

				// Write each field
				foreach (Tile tile in tiles)
				{
					if (((tile.Value == 9) || (tile.Value == 10)))
					{
						writer.WriteStartElement("Field");
						writer.WriteAttributeString("row", tile.Y.ToString());
						writer.WriteAttributeString("column", tile.X.ToString());

						string isMine = ((tile.Value == 9) || (tile.Value == 10)) ? "yes" : "no";    // 9, 10 = Mine
						writer.WriteStartElement("Mine");
						writer.WriteAttributeString("active", isMine);
						writer.WriteEndElement();

						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Flush();
			}

			return true;
		}

		public static bool GetFieldFromXML(string fileName, out int width, out int height, out List<Tile> tiles)
		{
			width = 0;
			height = 0;
			tiles = new();

			try
			{
				using (XmlReader reader = XmlReader.Create(fileName))
				{
					while (reader.Read())
					{
						if (reader.IsStartElement("Fields"))
						{
							if (!GetAttribute<int>(reader, "width", out width))
							{
								Console.WriteLine($"Invalid 'width' attribute.");
								return false;
							}

							if (!GetAttribute<int>(reader, "height", out height))
							{
								Console.WriteLine($"Invalid 'Height' attribute.");
								return false;
							}
						}

						else if (reader.IsStartElement("Field"))
						{
							int row = 0;
							if (!GetAttribute<int>(reader, "row", out row))
							{
								Console.WriteLine($"Invalid 'row' attribute.");
								return false;
							}

							int column = 0;
							if (!GetAttribute<int>(reader, "column", out column))
							{
								Console.WriteLine($"Invalid 'column' attribute.");
								return false;
							}

							if (reader.ReadToDescendant("Mine"))
							{
								string active = "";
								if (!GetAttribute<string>(reader, "active", out active))
								{
									Console.WriteLine($"Invalid 'active' attribute.");
									return false;
								}

								tiles.Add(new Tile()
								{
									X = column,
									Y = row,
									Value = active.ToLower() == "yes" ? 9 : 0
								});
							}
						}
					}
				}
				if (tiles.Count == 0)
				{
					Console.WriteLine($"Tile data is empty.");
					return false;
				}

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
			return false;
		}

		public static bool SetPlayToXML(string fileName, List<Log> logs)
		{
			if (logs == null)
			{
				return false;
			}

			StringWriter stringWriter = new StringWriter();
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace,
				OmitXmlDeclaration = false
			};

			using (XmlWriter writer = XmlWriter.Create(fileName, settings))
			{
				writer.WriteStartDocument();

				// Field infomation
				writer.WriteStartElement("Game");
				writer.WriteStartElement("Move");

				// Write each field
				int id = 0;
				foreach (Log log in logs)
				{
					writer.WriteStartElement("Step");
					writer.WriteAttributeString("id", (++id).ToString());
					writer.WriteAttributeString("time", log.Time.ToString());

					writer.WriteStartElement("Player");
					writer.WriteAttributeString("type", log.User);
					writer.WriteEndElement();   // player

					writer.WriteStartElement("Play");
					writer.WriteAttributeString("sign", log.Sign.ToString());
					writer.WriteString($"{log.X}/{log.Y}");
					writer.WriteEndElement();

					
					writer.WriteEndElement();	// Step
				}
				writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Flush();
			}

			return true;
		}

		public static bool GetPlayFromXML(string fileName, out List<Log> logs)
		{
			logs = new();

			try
			{
				using (XmlReader reader = XmlReader.Create(fileName))
				{
					while (reader.Read())
					{
						if (reader.IsStartElement("Step"))
						{
							long time = 0;
							string type = string.Empty, sign = string.Empty, position = string.Empty;
							if (!GetAttribute<long>(reader, "time", out time))
							{
								Console.WriteLine($"Invalid 'time' attribute.");
								return false;
							}

							if (reader.ReadToDescendant("Player"))
							{
								if (!GetAttribute<string>(reader, "type", out type))
								{
									Console.WriteLine($"Invalid 'type' attribute.");
									return false;
								}
							}

							if (reader.ReadToNextSibling("Play"))
							{
								if (!GetAttribute<string>(reader, "sign", out sign))
								{
									Console.WriteLine($"Invalid 'sign' attribute.");
									return false;
								}

								position = reader.ReadElementContentAsString();
							}

							string[] pos = position.Split('/');
							int x, y;
							
							int.TryParse(pos[0], out x);
							int.TryParse(pos[1], out y);

							logs.Add(new Log()
							{
								Time = time,
								User = type,
								Sign = sign == "Click" ? Log.PlaySign.Click : Log.PlaySign.SetFlag,
								X = x,
								Y = y
							});
						}
					}
				}
				if (logs.Count == 0)
				{
					Console.WriteLine($"Log data is empty.");
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
			return false;
		}

		private static bool GetAttribute<T>(XmlReader reader, string name, out T result)
		{
			result = default(T);
			try
			{
				string attribute = reader.GetAttribute(name);
				var converter = TypeDescriptor.GetConverter(typeof(T));
				result = (T)converter.ConvertFromString(attribute);
				return true;
			}
			catch (NotSupportedException)
			{
				return false;
			}
		}
	}
}
