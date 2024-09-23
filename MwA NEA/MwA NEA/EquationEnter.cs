using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MwA_NEA
{
	public class EquationEnter
	{
		private string[] columns;
		private double[] values;
		private bool negative;

		public EquationEnter(string[] columnSeparators, bool negative = true) => (columns, values, this.negative) = (columnSeparators, new double[columnSeparators.Length], negative);
		public EquationEnter(string[] columnSeparators, double[] columnValues, bool negative = true) => (columns, values, this.negative) = (columnSeparators, columnValues, negative);

		private void WriteOut(int left, int height, int currentOption, string[] currentValues, int align)
		{
			ConsoleHelper.ClearLines(height + 1, height + 1 + (columns.Sum(x => x.Length) + currentValues.Sum(x => x.Length)) / Console.BufferWidth);
			ConsoleHelper.ClearLine(height, left);
			Console.Write(String.Join(" ", Enumerable.Range(0, currentOption).Select(x => $"{currentValues[x]}{columns[x]}")));
			int startLeft = Console.CursorLeft + currentValues[currentOption].Length - align + 1;
			int startHeight = Console.CursorTop + startLeft / Console.BufferWidth;
			Console.Write($" {currentValues[currentOption]} {columns[currentOption]} ");
			Console.Write(String.Join("", Enumerable.Range(currentOption + 1, values.Length - currentOption - 1).Select(x => $"{currentValues[x]}{columns[x]}")));
			Console.SetCursorPosition(startLeft % Console.BufferWidth, startHeight);
		}
		public double[] Select()
		{
			string[] currentValues = values.Select(x => x == 0 ? "_" : x.ToString()).ToArray();
			int top = Console.CursorTop;
			int left = Console.CursorLeft;
			int currentOption = 0;
			int buffer = 0;
			
			bool exit = false;				
			do
			{
				WriteOut(left, top, currentOption, currentValues, buffer);
				ConsoleKeyInfo key = Console.ReadKey(true);

				if (int.TryParse(key.KeyChar.ToString(), out int num) || (key.KeyChar == '.' && !currentValues[currentOption].Contains('.')))
				{
					if (currentValues[currentOption] == "_") currentValues[currentOption] = key.KeyChar.ToString();
					else currentValues[currentOption] = currentValues[currentOption].Insert(currentValues[currentOption].Length - buffer, key.KeyChar.ToString());
				}
				else if (key.KeyChar == '-' && negative && !currentValues[currentOption].Contains('-'))
				{
					if (currentValues[currentOption] == "_") currentValues[currentOption] = "-";
					else currentValues[currentOption] = $"-{currentValues[currentOption]}";
				}
				else if (key.Key == ConsoleKey.Backspace && currentValues[currentOption].Length > 0 && currentValues[currentOption].Length - 1 - buffer >= 0)
				{
					currentValues[currentOption] = currentValues[currentOption].Remove(currentValues[currentOption].Length - 1 - buffer, 1);
				}
				else if (key.Key == ConsoleKey.RightArrow)
				{
					if (buffer == 0 && currentOption < values.Length - 1) currentOption++;
					else if (buffer > 0) buffer--;
				}
				else if (key.Key == ConsoleKey.LeftArrow)
				{
					if (buffer == currentValues[currentOption].Length && currentOption > 0)
					{
						buffer = 0;
						currentOption--;
					}
					else if (buffer < currentValues[currentOption].Length) buffer++;
				}
				else if ((key.Key == ConsoleKey.Tab || key.Key == ConsoleKey.Enter) && currentOption < values.Length - 1)
				{
					buffer = 0;
					currentOption++;
				}
				else if (key.Key == ConsoleKey.Escape || (key.Key == ConsoleKey.Enter && currentOption == values.Length - 1))
				{
					ConsoleHelper.ClearLines(top, top + (columns.Sum(x => x.Length) + currentValues.Sum(x => x.Length)) / Console.BufferWidth + 1);
					exit = true;
				}				
			} while (!exit);
			return currentValues.Select(x => double.TryParse(x, out double y) ? y : 0).ToArray();
		}
	}
}
