using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MwA_NEA
{
	public static class ConsoleHelper
	{
		public static void ClearLine(int height, int left = 0)
		{
			if (height < 0) height = 0;
			Console.SetCursorPosition(left, height);
			Console.WriteLine(new string(' ', Console.BufferWidth - left));
			Console.SetCursorPosition(left, height);
		}
		public static void ClearLines(int top, int bottom = -1)
		{
			if (bottom == -1) bottom = Console.BufferHeight - 1;
			for (int i = top; i < bottom; i++) ClearLine(i);
			Console.SetCursorPosition(0, top);
		}
        public static void ColourText(ConsoleColor colour, ConsoleColor originalColour, string text, bool slowText = false)
        {
            Console.ForegroundColor = colour;
			if (slowText) SlowText(text);
            else Console.Write(text);
            Console.ForegroundColor = originalColour;
        }
		public static void SlowText(string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				Console.Write(text[i]);
				Thread.Sleep(15);
			}
			Console.WriteLine();
		}
        public static void WaitForKey(string message = "Press any key to continue")
		{
			Console.WriteLine(message);
            Console.ReadKey(true);
		}
	}
}
