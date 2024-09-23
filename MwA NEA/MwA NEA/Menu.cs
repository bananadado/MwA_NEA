using System;
using System.Collections.Generic;
using System.Linq;

namespace MwA_NEA
{
	// A custom menu that you can utilise with arrow keys.
	public class Menu
	{
		public List<string> items;
		private char pointerType;
		bool back;

		public Menu(List<string> items, char pointer = '>', bool back = false) => (this.items, pointerType, this.back) = (items, pointer, back);

		public void ChangePointer(char newPointer) => pointerType = newPointer;

		public string Select(bool returnItemPos = false)
		{
			Console.CursorVisible = false;

			foreach (string item in items)
			{
				Console.WriteLine($" {item}");
			}

			int maxOption = items.Count();
			int top = Console.CursorTop;
			int option = 0;
			Console.CursorTop = option + top - maxOption;
			Console.CursorLeft = 0;
			Console.Write(pointerType);
			do
			{
				ConsoleKeyInfo choice = Console.ReadKey(true);

				if (choice.Key == ConsoleKey.DownArrow && option < maxOption - 1)
				{
					Console.CursorTop = option + top - maxOption;
					Console.CursorLeft = 0;
					Console.Write(" ");
					option++;
					Console.CursorTop = option + top - maxOption;
					Console.CursorLeft = 0;
					Console.Write(pointerType);
				}
				else if (choice.Key == ConsoleKey.UpArrow && option > 0)
				{
					Console.CursorTop = option + top - maxOption;
					Console.CursorLeft = 0;
					Console.Write(" ");
					option--;
					Console.CursorTop = option + top - maxOption;
					Console.CursorLeft = 0;
					Console.Write(pointerType);
				}
				else if (choice.Key == ConsoleKey.Enter)
				{
					// erase the menu
					ConsoleHelper.ClearLines(top - maxOption, top);
					Console.CursorVisible = true;

					// return the choice
					return returnItemPos? option.ToString() : items[option];
				}
				else if (back && (choice.Key == ConsoleKey.Escape || choice.Key == ConsoleKey.LeftArrow))
				{
					ConsoleHelper.ClearLines(top - maxOption, top);
					Console.CursorVisible = true;
					return "-1";
				}
			} while (true);
		}
		public int SelectOption()
		{
			return int.Parse(Select(true));
		}
	}
}
