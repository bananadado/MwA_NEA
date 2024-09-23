using System;
using System.Linq;
using System.Numerics;

namespace MwA_NEA
{
	// class used for extra maths functions and constants that the Math static class doesn't contain
	public static class myMath
	{
		public static readonly char[] subscriptNums = new char[] { '₀', '₁', '₂', '₃', '₄', '₅', '₆', '₇', '₈', '₉' };
		public const int sigFigPrecision = 3;
		private const double EPSILON = 1e-12;

		public static string GetSubscript(int num)
		{
			string sub = "";
			foreach(char digit in num.ToString())
			{
				sub += subscriptNums[digit - '0'];
			}
			return sub;
		}
		public static bool WithinPrecision(double value1, double value2) => Math.Abs(value1 - value2) < EPSILON;
		public static double SigFig(double num, int significantFigures = sigFigPrecision)
		{
			if (num >= 0 - EPSILON && num <= 0 + EPSILON) return 0;

			return double.Parse(num.ToString($"G{significantFigures}"));
		}
		public static double Argument(double a, double b)
		{
			double pi = Math.PI;

			// Imaginary axis
			if (a == 0 && b > 0) return pi / 2;
			if (a == 0 && b < 0) return -pi / 2;

			// top-left quadrant, bottom-left qudarant, right 2 quadrants
			if (a < 0 && b >= 0) return Math.Atan(b / a) + pi;
			if (a < 0 && b < 0) return Math.Atan(b / a) - pi;
			return Math.Atan(b / a);
		}
	}
}
