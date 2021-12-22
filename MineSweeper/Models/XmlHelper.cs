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
					writer.WriteStartElement("Field");
					writer.WriteAttributeString("row", tile.Y.ToString());
					writer.WriteAttributeString("column", tile.X.ToString());

					string isMine = ((tile.Status == 9) || (tile.Status == 10)) ? "yes" : "no";    // 9, 10 = Mine
					writer.WriteStartElement("Mine");
					writer.WriteAttributeString("active", isMine);
					writer.WriteEndElement();

					writer.WriteEndElement();
				}
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Flush();
			}

			return true;
		}

		public static bool GetFieldToXML(string fileName, out int width, out int height, out List<Tile> tiles)
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
								Debug.WriteLine($"Invalid 'width' attribute.");
								return false;
							}

							if (!GetAttribute<int>(reader, "height", out height))
							{
								Debug.WriteLine($"Invalid 'Height' attribute.");
								return false;
							}
						}

						else if (reader.IsStartElement("Field"))
						{
							int row = 0;
							if (!GetAttribute<int>(reader, "row", out row))
							{
								Debug.WriteLine($"Invalid 'row' attribute.");
								return false;
							}

							int column = 0;
							if (!GetAttribute<int>(reader, "column", out column))
							{
								Debug.WriteLine($"Invalid 'column' attribute.");
								return false;
							}

							if (reader.ReadToDescendant("Mine"))
							{
								string active = "";
								if (!GetAttribute<string>(reader, "active", out active))
								{
									Debug.WriteLine($"Invalid 'active' attribute.");
									return false;
								}

								tiles.Add(new Tile()
								{
									X = column,
									Y = row,
									Status = active.ToLower() == "yes" ? 9 : 0
								});
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error: {ex.Message}");
			}
			return true;
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
