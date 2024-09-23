using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MwA_NEA
{
	public class OptionScroller
	{
		private string text;
		private int low, high;
		private bool ascii;

		public OptionScroller(string text, int lowestValue = int.MinValue, int highestValue = int.MaxValue, bool ascii = false) => (this.text, low, high, this.ascii) = (text, lowestValue, highestValue, ascii);

		public int Select()
		{
			Console.CursorVisible = false;
			int top = Console.CursorTop + 1;
			int currentOption = low == int.MinValue? 0 : low;
			do
			{
				Console.CursorTop = top;
				Console.Write($"{text}{(ascii ? ((char)(currentOption + 65)).ToString() : currentOption.ToString())}");
				if (currentOption != high)
				{
					Console.CursorTop = top - 1;
					Console.CursorLeft = text.Length + (int)Math.Round((double)currentOption.ToString().Length / 2);
					Console.Write("Λ");
				}
				if (currentOption != low)
				{
					Console.CursorTop = top + 1;
					Console.CursorLeft = text.Length + (int)Math.Round((double)currentOption.ToString().Length / 2);
					Console.Write("V");
				}

				ConsoleKeyInfo key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.UpArrow && currentOption < high) currentOption++;
				else if (key.Key == ConsoleKey.DownArrow && currentOption > low) currentOption--;
				else if (key.Key == ConsoleKey.Enter)
				{
					ConsoleHelper.ClearLines(top - 1, top + 2);
					Console.CursorVisible = true;
					return currentOption;
				}
				ConsoleHelper.ClearLines(top - 1, top + 2);
			} while (true);			
		}
	}
}
