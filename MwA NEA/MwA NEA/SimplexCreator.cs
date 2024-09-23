using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Windows.Forms.VisualStyles;

namespace MwA_NEA
{
	public class SimplexCreator : IProblem
	{
		private SimplexSolver problem;
		
		private void GenerateLEQ(Random rnd, int dimension, int noConstraints, int LB, int UB, List<Equation> constraints, string[] variableNames)
		{
			// 2-3 <= constraints.
			double[] minAxisIntercepts = new double[dimension].Select(x => double.PositiveInfinity).ToArray();

			for (int i = 0; i < noConstraints; i++)
			{
				double[] tempVariableValues = new double[dimension];
				int c = rnd.Next(LB, UB * 2 + 1);
				int LBCoefficient = Math.Max(1, c / UB);
				int UBCoefficient = c / LB;
				int guaranteed = rnd.Next(dimension);

				for (int j = 0; j < dimension; j++)
				{
					// One in (Dimension) chance that a coefficient is 0.
					// If it is a 0 but that dimension is not yet bounded on the final pass, it is skipped.
					if (rnd.Next(0, dimension) == 0 && !(double.IsPositiveInfinity(minAxisIntercepts[j]) && i == dimension - 1) && j != guaranteed) tempVariableValues[j] = 0;
					else
					{
						tempVariableValues[j] = rnd.Next(LBCoefficient, UBCoefficient + 1);
						minAxisIntercepts[j] = Math.Min(minAxisIntercepts[j], c / tempVariableValues[j]);
					}
				}
				constraints.Add(new Equation(variableNames, tempVariableValues, "<=", c));
			}
		}
		private bool GenerateGEQ(Random rnd, int dimension, int difficulty, int LB, List<Equation> constraints, string[] variableNames)
		{
			// 0-2 >= constraints. 0 if difficulty is 1, up to 1 if difficulty is 2
			// There should always be a feasible region as long as one of the axis intercepts in the equation is less than the smallest axis intercept of any of the <= equations
			int noConstraints;
			bool artificial = false;

			if (difficulty == 1) noConstraints = 0;
			else if (difficulty == 2) noConstraints = rnd.Next(0, 2);
			else noConstraints = rnd.Next(0, 3);

			for (int i = 0; i < noConstraints; i++)
			{
				artificial = true;
				int c = rnd.Next(1, LB * 2 + 1);
				double[] tempVariableValues = new double[dimension];
				int guaranteed = rnd.Next(dimension);

				for (int j = 0; j < dimension; j++)
				{
					if (j == guaranteed) tempVariableValues[j] = rnd.Next(1, c / LB + 1);
					else if (rnd.Next(0, dimension) == 0) tempVariableValues[j] = 0;
					else tempVariableValues[j] = rnd.Next(1, LB);
				}
				constraints.Add(new Equation(variableNames, tempVariableValues, ">=", c));
			}
			return artificial;
		}
		private void GenerateEQ(Random rnd, int dimension, int LB, List<Equation> constraints, string[] variableNames)
		{
			int c = rnd.Next(1, LB * 2 + 1);
			double[] tempVariableValues = new double[dimension];
			int guaranteed = rnd.Next(dimension);

			for (int j = 0; j < dimension; j++)
			{
				if (j == guaranteed) tempVariableValues[j] = rnd.Next(1, c / LB + 1);
				else if (rnd.Next(0, dimension) == 0) tempVariableValues[j] = 0;
				else tempVariableValues[j] = rnd.Next(1, LB);
			}
			constraints.Add(new Equation(variableNames, tempVariableValues, "=", c));
		}
		public void GenerateQuestion(Random rnd)
		{
			Equation objective;
			List<Equation> constraints = new List<Equation>();

			int difficulty = new OptionScroller("Choose Difficulty: ", 1, 5).Select();

			// 2-5 dimensions
			int dimension = rnd.Next(0, 2) + difficulty;
			if (difficulty == 1) dimension = 2;

			string[] variableNames = new string[dimension];
			double[] tempVariableValues = new double[dimension];
			for (int i = 0; i < dimension; i++)
			{
				variableNames[i] = dimension == 2 ? i == 0 ? "x" : "y" : $"x{myMath.GetSubscript(i + 1)}";
				tempVariableValues[i] = rnd.Next(1, 11) * (rnd.Next(0, 5) == 0 ? -1 : 1);
			}

			// Create objective function
			objective = new Equation(new string[] { "P" }, new double[] { 1 }, "=", variableNames, tempVariableValues);

			//Create constraints
			int noConstraints = rnd.Next(2, 4);
			int lowerBound = rnd.Next(8, 80);
			int upperBound = lowerBound + 4 * (int)Math.Round(Math.Pow(lowerBound, 0.5));

			GenerateLEQ(rnd, dimension, rnd.Next(2, 4), lowerBound, upperBound, constraints, variableNames);
			bool GEQConstraints = GenerateGEQ(rnd, dimension, difficulty, lowerBound, constraints, variableNames);
			// If no >= constraint, chance of there being a = constraint as long as the difficulty is greater than 2
			if ((!GEQConstraints && rnd.Next(0, 2) == 0 && difficulty > 3) || (!GEQConstraints && difficulty == 5)) GenerateEQ(rnd, dimension, lowerBound, constraints, variableNames);

			// Display the created problem
			Console.WriteLine($"Maximise: {objective}\nSubject to:");
			foreach (var constraint in constraints)
			{
				Console.WriteLine(constraint);
			}
			Console.WriteLine(String.Join(", ", variableNames.Select(x => $"{x} >= 0")));
			Console.WriteLine();
			problem = new SimplexSolver(objective, constraints, dimension, BasicVarsMenu(), IterationStepMenu());
		}

		private bool BasicVarsMenu()
			=> new Menu(new List<string>() { "Display basic variables at each iteration", "Don't display basic variables at each iteration" }).SelectOption() == 0;
		private bool IterationStepMenu()
			=> new Menu(new List<string>() { "Show how to calculate each row", "Don't show how to calculate each row" }).SelectOption() == 0;

		private Equation InputObjective(int dimension, string[] variableNames, string[] variableConcat)
		{
			Console.Write("Enter objective function: \nP = ");
			Equation objective = new Equation(new string[] { "P" }, new double[] { 1 }, "=", variableNames, new EquationEnter(variableConcat).Select());
			ConsoleHelper.ClearLine(Console.CursorTop - 1);
			Console.WriteLine($"Objective: {objective}");
			return objective;
		}
		private void InputEquations(Equation[] constraints, int leqConstraints, int geqConstraints, string[] variableNames, string[] variableConcat)
		{
			while (true)
			{
				int option = new Menu(Enumerable.Range(1, constraints.Length).Select(x => $"Equation {x} ({(x <= leqConstraints ? "<=" : x - leqConstraints <= geqConstraints ? ">=" : "=")}): {(!(constraints[x - 1] is null) ? constraints[x - 1].ToString() : "")}").Concat(new string[] { "Finish!" }).ToList()).SelectOption();
				if (option == constraints.Length) break;

				string symbol = option < leqConstraints ? "<=" : option - leqConstraints < geqConstraints ? ">=" : "=";
				double[] values;
				if (constraints[option] is null)
				{
					values = new EquationEnter(Enumerable.Range(0, variableConcat.Length)
						.Select(x => x == variableConcat.Length - 1 ? $"{variableConcat[x]} {symbol}" : variableConcat[x])
						.Concat(new string[] { "" }).ToArray()).Select();
				}
				else
				{
					values = new EquationEnter(Enumerable.Range(0, variableConcat.Length)
						.Select(x => x == variableConcat.Length - 1 ? $"{variableConcat[x]} {symbol}" : variableConcat[x])
						.Concat(new string[] { "" }).ToArray(), constraints[option].GetLHSValues().Concat(constraints[option].GetRHSValues()).ToArray()).Select();

				}
				constraints[option] = new Equation(variableNames, values.Take(values.Length - 1).ToArray(), symbol, values[values.Length - 1]);
			}
		}
		private bool CheckBounded(int dimension, Equation[] constraints)
		{
			// Validate input, check if all dimensions are restricted
			bool[] validate = new bool[dimension].Select(x => false).ToArray();
			foreach (Equation constraint in constraints)
			{
				if (constraint is null) return false;
				if (constraint.symbol == ">=") continue;
				double[] LHS = constraint.GetLHSValues();
				for (int i = 0; i < dimension; i++)
				{
					if (LHS[i] != 0)
					{
						validate[i] = true;
					}
				}
			}
			// Returns true if the region is bounded
			return !validate.Contains(false);
		}
		private void InputConstraints(int dimension)
		{
			Equation objective;
			Equation[] constraints = new Equation[new OptionScroller("Select number of constraints: ", 1).Select()];
			Console.WriteLine($"Number of constraints: {constraints.Length}");
			int leqConstraints = new OptionScroller("Number of '<=' constraints: ", 0, constraints.Length).Select();
			Console.WriteLine($"Number of '<=' constraints: {leqConstraints}");
			int geqConstraints = 0;
			if (constraints.Length - leqConstraints > 0) geqConstraints = new OptionScroller("Number of '>=' constraints: ", 0, constraints.Length - leqConstraints).Select();
			Console.WriteLine($"Number of '>=' constraints: {geqConstraints}");
			Console.WriteLine($"Number of '=' constraints: {constraints.Length - leqConstraints - geqConstraints}");

			// Create the variable names array and the same array but connected with + to enter in the equations
			string[] variableNames = new string[dimension];
			for (int i = 0; i < dimension; i++)
			{
				variableNames[i] = dimension == 2 ? i == 0 ? "x" : "y" : $"x{myMath.GetSubscript(i + 1)}";
			}
			string[] variableConcat = String.Join("+ ,", variableNames).Split(',').ToArray();

			objective = InputObjective(dimension, variableNames, variableConcat);
			InputEquations(constraints, leqConstraints, geqConstraints, variableNames, variableConcat);

			// If dimension not restricted, throw error
			try
			{
				if (!CheckBounded(dimension, constraints)) throw new RegionNotBoundedException();
				
				// Display problem and stop
				Console.WriteLine("Subject to:");
				foreach (var constraint in constraints)
				{
					Console.WriteLine(constraint);
				}
				Console.WriteLine();
				problem = new SimplexSolver(objective, constraints.ToList(), dimension, BasicVarsMenu(), IterationStepMenu());
			}
			catch (RegionNotBoundedException)
			{
				Console.WriteLine("Region not bounded or not all constraints filled!");
				Console.ReadKey();
				Console.Clear();
			}
		}
		private bool CheckBasic(double[,] matrix, int col)
		{
			int ones = 0;
			int zeroes = 0;
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				if (matrix[i, col] == 0) zeroes++;
				else if (matrix[i, col] == 1) ones++;
			}
			return ones == 1 && ones + zeroes == matrix.GetLength(0);
		}
		private string[] SetUpTableauVariables(int dimension, int noOfS, int noOfA)
		{
			if (noOfA > 0) return new string[] { "A", "P" }
			.Concat(Enumerable.Range(1, dimension).Select(x => $"x{myMath.GetSubscript(x)}").ToArray())
			.Concat(Enumerable.Range(1, noOfS).Select(x => $"s{myMath.GetSubscript(x)}").ToArray())
			.Concat(Enumerable.Range(1, noOfA).Select(x => $"a{myMath.GetSubscript(x)}").ToArray())
			.Concat(new string[] { "RHS" }).ToArray();

			return new string[] { "P" }
			.Concat(Enumerable.Range(1, dimension).Select(x => $"x{myMath.GetSubscript(x)}").ToArray())
			.Concat(Enumerable.Range(1, noOfS).Select(x => $"s{myMath.GetSubscript(x)}").ToArray())
			.Concat(new string[] { "RHS" }).ToArray();
		}
		private void InputTableau(int dimension)
		{
			// Input number of constraints etc.
			int noOfConstraints = new OptionScroller("Select number of constraints: ", 1).Select();
			Console.WriteLine($"Number of constraints: {noOfConstraints}");
			int noOfS = new OptionScroller("Select number of slack/surplus variables (\"s\"): ", 1).Select();
			Console.WriteLine($"Number of slack/surplus: {noOfS}");
			int noOfA = new OptionScroller("Select number of artificial variables (\"a\"): ", 0).Select();
			Console.WriteLine($"Number of artificial: {noOfA}");

			string[] variables = SetUpTableauVariables(dimension, noOfS, noOfA);

			int artificialOffset = noOfA > 0 ? 1 : 0;
			double[,] values = new double[noOfConstraints + 1 + artificialOffset, variables.Length];
			if (noOfA > 0) values[0, 0] = 1;
			values[0 + artificialOffset, 0 + artificialOffset] = 1;

			values = new MatrixEnter(values, variables).Select();

			// There is no input validation for a tableau apart from checking if A and P are basic
			try
			{
				if (CheckBasic(values, 0) && (noOfA == 0 || ((noOfA > 0) && CheckBasic(values, 1))))
				{
					problem = new SimplexSolver(values, variables, noOfA > 0, BasicVarsMenu(), IterationStepMenu());
				}
				else throw new FormatException("");
			}
			catch (FormatException)
			{
				Console.WriteLine("Input in incorrect format :(");
				Console.ReadKey();
				Console.Clear();
			}
		}
		public void InputQuestion()
		{
			int dimension = new OptionScroller("Select dimension: ", 2).Select();
			Console.WriteLine($"Dimension: {dimension}");

			Console.WriteLine("Select format:");
			if (new Menu(new List<string>() { "Constraints", "Tableau"}).SelectOption() == 0)  // Constraints
			{
				ConsoleHelper.ClearLine(Console.CursorTop - 1);
				InputConstraints(dimension);
			}
			else  // Tableau
			{
				ConsoleHelper.ClearLine(Console.CursorTop - 1);
				InputTableau(dimension);
			}
		}
		public void GenerateAnswer()
		{
			if (!(problem is null))	problem.Solve();
			else Console.WriteLine("There is no problem to solve!");
		}
	}
}
