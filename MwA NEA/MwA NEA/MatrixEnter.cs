using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MwA_NEA
{
	public class MatrixEnter
	{
		private double[,] matrix;
		private string[] columns;
		bool graph, symmetry, negative;
		public MatrixEnter(double[,] matrix, string[] columns, bool isGraph = false, bool symmetrical = false, bool negative = true) =>
			(this.matrix, this.columns, graph, symmetry, this.negative) = (matrix, columns, isGraph, symmetrical, negative);

				
		private int WriteOut(int top, int col, int row, string[,] currentValues, int buffer, bool setup = false)
		{
			int startLeft = 0;
			List<string> lines = new List<string>();

			// find largest size in each column
			int[] largestSize = new int[columns.Length];
			for (int i = 0; i < largestSize.Length; i++)
			{
				largestSize[i] = columns[i].Length;
				for (int j = 0; j < matrix.GetLength(0); j++)
				{
					largestSize[i] = Math.Max(largestSize[i], currentValues[j, i].Length);
					if(i == col && j == row) largestSize[i] = Math.Max(largestSize[i], currentValues[j, i].Length + 2);
				}
			}
			lines.Add("|".PadLeft(graph? 2 : 1));

			if (graph) foreach (string column in columns) lines.Add($"{column}|");
			else for (int i = 0; i < matrix.GetLength(0); i++) lines.Add("|");

			for (int i = 0; i < matrix.GetLength(1); i++)
			{
				lines[0] += $"{columns[i]}".PadLeft(largestSize[i]) + "|";
				for (int j = 0; j < matrix.GetLength(0); j++)
				{
					if (j == row && i == col) 
					{ 
						lines[j + 1] += $" {$"{currentValues[j, i]}".PadLeft(largestSize[i] - 2)} |";
						startLeft = lines[j + 1].Length - 2 - buffer;
					}
					else lines[j + 1] += $"{(currentValues[j, i])}".PadLeft(largestSize[i]) + "|";
				}
			}
			Console.CursorTop = top;
			Console.CursorLeft = 0;
			for (int i = 0; i < lines.Count; i++)
			{
				Console.WriteLine(lines[i]);

				// colouring for visibilty
				if (i % 2 == 0) Console.ForegroundColor = ConsoleColor.Cyan;
				else Console.ForegroundColor = ConsoleColor.Gray;
			}
			Console.WriteLine($"{(row == matrix.GetLength(0)? ">" : "")}Enter");
			Console.ForegroundColor = ConsoleColor.Gray;

			// On some consoles, this gets around moving the matrix down a line each time when the matrix goes out of the buffer height
			if (setup && top != Console.CursorTop - (matrix.GetLength(0) + 2)) 
			{
				top = Console.CursorTop - (matrix.GetLength(0) + 2) - 1;
				ConsoleHelper.ClearLine(Console.CursorTop);
			}
			
			Console.SetCursorPosition(startLeft, row + top + 1);
			return top;
		}
		private string[,] SetUpCurrentValues()
		{
			string[,] currentValues = new string[matrix.GetLength(0), matrix.GetLength(1)];
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				for (int j = 0; j < matrix.GetLength(1); j++)
				{
					if (graph && i == j) currentValues[i, j] = "/";
					else if (matrix[i, j] == 0) currentValues[i, j] = "_";
					else currentValues[i, j] = matrix[i, j].ToString();
				}
			}
			return currentValues;
		}
		private void ParseForReturn(string[,] currentValues)
		{
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				for (int j = 0; j < matrix.GetLength(1); j++)
				{
					if (double.TryParse(currentValues[i, j], out double x)) matrix[i, j] = x;
					else matrix[i, j] = 0;
				}
			}
		}
		public double[,] Select()
		{
			int maxWidth = (Console.BufferWidth - (graph ? 2 : 1)) / columns.Length - 1; 
			string[,] currentValues = SetUpCurrentValues();
			int top = Console.CursorTop;
			int col = graph? 1 : 0;
			int row = 0;
			int align = 0;

			top = WriteOut(top, col, row, currentValues, align, true);

			bool exit = false;
			do
			{
				WriteOut(top, col, row, currentValues, align);
				ConsoleKeyInfo key = Console.ReadKey(true);
					
                if (row != matrix.GetLength(0))
				{
                    bool isTooBig = currentValues[row, col].Length == maxWidth;
                    if (!isTooBig && (int.TryParse(key.KeyChar.ToString(), out int num) || (key.KeyChar == '.' && !currentValues[row, col].Contains('.'))))
					{
						if (currentValues[row, col] == "_") currentValues[row, col] = key.KeyChar.ToString();
						else currentValues[row, col] = currentValues[row, col].Insert(currentValues[row, col].Length - align, key.KeyChar.ToString());

						if (symmetry) currentValues[col, row] = currentValues[row, col];
					}
					else if (!isTooBig && key.KeyChar == '-' && negative && !currentValues[row, col].Contains('-'))
					{
						if (currentValues[row, col] == "_") currentValues[row, col] = "-";
						else currentValues[row, col] = $"-{currentValues[row, col]}";
					}
					else if (key.Key == ConsoleKey.Backspace && currentValues[row, col].Length > 0)
					{
						currentValues[row, col] = currentValues[row, col].Remove(currentValues[row, col].Length - 1 - align, 1);
						if (symmetry) currentValues[col, row] = currentValues[row, col];
					}
					else if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.Tab)
					{
						if (graph && col + 1 == row && col + 2 < columns.Length) col++;
						if (align == 0 && col < columns.Length - 1 && !(graph && col + 1 == row))
						{
							col++;
							align = 0;
						}
						else if (align > 0) align--;
					}
					else if (key.Key == ConsoleKey.LeftArrow)
					{
						if (align == currentValues[row, col].Length && col > 0)
						{
							if (graph && col - 1 == row && col - 2 >= 0) col--;
							if (col > 0 && !(graph && col - 1 == row))
							{
								align = 0;
								col--;
							}
						}
						else if (align < currentValues[row, col].Length) align++;
					}
					else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.Enter)
					{
						if (graph && col == row + 1 && row + 2 <= matrix.GetLength(0)) row++;
						if (row < matrix.GetLength(0) - 1 && !(graph && col == row + 1))
						{
							row++;
							align = Math.Min(currentValues[row, col].Length, align);
						}
						else if (row == matrix.GetLength(0) - 1)
						{
							col = 0;
							align = 0;
							row++;
						}
					}
					else if (key.Key == ConsoleKey.Escape)
					{
						exit = true;
					}
				}
				else if (key.Key == ConsoleKey.Enter)
				{
					exit = true;
				}
				if (key.Key == ConsoleKey.UpArrow && row - 1 >= 0)
				{
					if (graph && col == row - 1 && row - 2 >= 0) row--;
					if (row - 1 >= 0 && (!graph || (graph && col != row - 1)))
					{
						row--;
						align = Math.Min(currentValues[row, col].Length, align);
					}
				}	
				ConsoleHelper.ClearLines(top, top + matrix.GetLength(0) + 3);
			} while (!exit);
			Console.SetCursorPosition(0, top);
			ParseForReturn(currentValues);
			return matrix;
		}
	}
}
