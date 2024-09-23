using System;
using System.Linq;

namespace MwA_NEA
{
	public class Equation
	{
		private string[] LHSvars, RHSvars;
		private double[] LHS, RHS;
		public string symbol;
		public Equation(string[] LHSvariables, double[] LHScoefficient, string equality, string[] RHSvariables, double[] RHScoefficient) => 
			(symbol, LHSvars, LHS, RHSvars, RHS) = (equality, LHSvariables, LHScoefficient, RHSvariables, RHScoefficient);
		public Equation(string[] LHSvariables, double[] LHScoefficient, string equality, double RHSvalue) => 
			(symbol, LHSvars, LHS, RHSvars, RHS) = (equality, LHSvariables, LHScoefficient, new string[1], new double[] { RHSvalue });

		private string JoinCoefficients(string[] vars, double[] values)
		{
			string currentSide = "";
			for (int i = 0; i < vars.Length; i++)
			{
				string toAdd;
				if (values[i] == 0 && !(vars[i] is null)) continue;

				if (values[i] == 1 && !(vars[i] is null)) toAdd = $"+ {vars[i]} ";
				else if (values[i] == -1) toAdd = $"- {vars[i]} ";
				else if (values[i] < 0) toAdd = $"- {Math.Abs(values[i])}{vars[i]} ";
				else toAdd = $"+ {values[i]}{vars[i]} ";

				currentSide += currentSide == "" ? toAdd.TrimStart('+') : toAdd;
			}
			return currentSide.Trim();
		}
		public string[] GetLHSvariables() => LHSvars;
		public double[] GetLHSValues() => LHS;
		public double[] GetRHSValues() => RHS;
		public string[] GetRHSvariables() => RHSvars;

		public override string ToString() => $"{JoinCoefficients(LHSvars, LHS)} {symbol} {JoinCoefficients(RHSvars, RHS)}";
	}
}
