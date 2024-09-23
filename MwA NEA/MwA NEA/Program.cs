using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace MwA_NEA
{
	internal class Program
	{
		private static void DoProblem(Random rnd, IProblem problem, bool userInput)
		{
			if (userInput) problem.InputQuestion();
			else problem.GenerateQuestion(rnd);

			if (new Menu(new List<string>() { "Solve!", "Exit"}).SelectOption() == 0)
			{
				problem.GenerateAnswer();
			}
		}
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			Random rnd = new Random();
			List<string> types = new List<string>() { "Simplex", "Dijkstra", "Minimum Spanning Tree", "QuickSort", "Tutorial", "Exit" };

			do
			{
                Console.WriteLine("Main menu");
				int type = new Menu(types).SelectOption();
				if (type == 5) break;
                ConsoleHelper.ClearLine(Console.CursorTop - 1);
				if (type == 4) Tutorial.StartTutorial();
				else
				{
					int enter = new Menu(new List<string>() { "Generate new problem", "Enter your own problem" }, back: true).SelectOption();
					if (enter == -1) continue;
					switch (type)
					{
						case 0:
							DoProblem(rnd, new SimplexCreator(), enter == 1);
							break;
						case 1:
							DoProblem(rnd, new DijkstraCreator(), enter == 1);
							break;
						case 2:
							DoProblem(rnd, new MSTCreator(), enter == 1);
							break;
						case 3:
							DoProblem(rnd, new QuickSortCreator(), enter == 1);
							break;
					}
				}
				// Leave a gap in the console between this and last question, making sure the whole last question stays in the console history
				ConsoleHelper.ClearLine(Console.CursorTop - 1);
				Console.WriteLine("-------------------");

				// The first option is compatible with the school console, the other option is compatible with all other consoles
				try
				{
					Console.SetWindowPosition(0, Console.CursorTop);
				}
				catch (ArgumentOutOfRangeException e)
				{
					for (int i = 0; i < Console.BufferHeight; i++)
					{
						Console.WriteLine();
					}
					Console.CursorTop = 0;
				}
            } while (true);
		}
	}
}
