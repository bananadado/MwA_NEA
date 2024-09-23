using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace MwA_NEA
{
	public class SimplexSolver
	{
		// Unformatted equations
		private Equation objective;
		private List<Equation> constraints;

		private string[] variables;
		private double[,] values;
		private bool stage2, displayBasic, displayRowOperation, skipReformulation;

		// variables for 2D graphing
		private Simplex2DGraph plot;
		private int dimension;

		// constraints
		public SimplexSolver(Equation objective, List<Equation> constraints, int dimension, bool displayBasicVars, bool displayRowCalc)
			=> (this.objective, this.constraints, this.dimension, displayBasic, displayRowOperation, skipReformulation) = (objective, constraints, dimension, displayBasicVars, displayRowCalc, false);

		// tableaux
		public SimplexSolver(double[,] values, string[] variables, bool stage2, bool displayBasicVars, bool displayRowCalc)
			=> (this.values, this.variables, this.stage2, displayBasic, displayRowOperation, dimension, skipReformulation) = (values, variables, stage2, displayBasicVars, displayRowCalc, 0, true);

		
		private int[] FindLargestWidth(int pivotCol)
		{
			//The following for loop finds out how much space each variable should take up when being displayed on the console
			int[] largestSizes = new int[variables.Length];
			for (int i = 0; i < variables.Length; i++)
			{
				int largestSize = variables[i].Length;
				for (int j = 0; j < values.GetLength(0); j++)
				{
					//increase the corresponding largest size if the variable in the column is wider
					if (myMath.SigFig(values[j, i]).ToString().Length > largestSize) largestSize = myMath.SigFig(values[j, i]).ToString().Length;
				}
				largestSize++;
				largestSizes[i] = largestSize;

				if (i == pivotCol) ConsoleHelper.ColourText(ConsoleColor.Yellow, ConsoleColor.Gray, $"{variables[i]}".PadRight(largestSize) + "|");
				else Console.Write($"{variables[i]}".PadRight(largestSize) + "|");
			}
			return largestSizes;
		}
		private void DisplayRatio(double[] ratios, int pivotCol, int i)
		{
			if (ratios.Length == 0) Console.WriteLine();
			else if ((stage2 && i >= 2 && values[i, pivotCol] == 0) || (!stage2 && i >= 1 && values[i, pivotCol] == 0)) Console.WriteLine("undefined");
			else if (stage2 && i >= 2) Console.WriteLine(myMath.SigFig(ratios[i - 2]));
			else if (!stage2 && i >= 1) Console.WriteLine(myMath.SigFig(ratios[i - 1]));
			else Console.WriteLine();
		}
		private void DisplayRow(int pivotRow, int pivotCol, int[] largestSizes, int i)
		{
			//checks if it is a pivot, then changes colour accordingly
			if (i == pivotRow) Console.ForegroundColor = ConsoleColor.Magenta;
			for (int j = 0; j < values.GetLength(1); j++)
			{
				string text = myMath.SigFig(values[i, j]).ToString().PadRight(largestSizes[j]) + "|";
				if (i == pivotRow && j == pivotCol) ConsoleHelper.ColourText(ConsoleColor.Red, ConsoleColor.Magenta, text);
				else if (j == pivotCol) ConsoleHelper.ColourText(ConsoleColor.Yellow, ConsoleColor.Gray, text);
				else Console.Write(text);
			}
			Console.ForegroundColor = ConsoleColor.Gray;
		}
		private void DisplayTableau(string message)
		{
			//The pivot column and row are off the grid so they can't be highlighted
			//There are no ratio tests to be shown
			DisplayTableau(-1, -1, message, Array.Empty<double>());
		}
		private void DisplayTableau(int pivotRow, int pivotCol, string iterationMessage, double[] ratios)
		{
			// Display the current iteration of the tableau. The final tableau is set to have iteration = -1.
			Console.WriteLine(iterationMessage);

			int[] largestSizes = FindLargestWidth(pivotCol);
			Console.WriteLine(ratios.Length > 0 ? "Ratio Test" : "");

			for (int i = 0; i < values.GetLength(0); i++)
			{
				DisplayRow(pivotRow, pivotCol, largestSizes, i);
				DisplayRatio(ratios, pivotCol, i);
			}
			Console.WriteLine();
		}
		private int ChooseLargestColumn()
		{
			double largest = values[0, 2];
			int column = 2;
			for (int i = 1; i < values.GetLength(1) - 1; i++)
			{
				if (values[0, i] > largest)
				{
					largest = values[0, i];
					column = i;
				}
			}
			return column;
		}
		private int ChooseSmallestColumn()
		{
			double smallest = values[0, 1];
			int column = 1;
			for (int i = 1; i < values.GetLength(1) - 1; i++)
			{
				if (values[0, i] < smallest)
				{
					smallest = values[0, i];
					column = i;
				}
			}
			return column;
		}
		private double[] RatioTest(int pivotColumn)
		{
			int buffer = stage2 ? 2 : 1;
			double[] tests = new double[values.GetLength(0) - buffer];
			for (int i = buffer; i < values.GetLength(0); i++)
			{
				if (values[i, pivotColumn] != 0) tests[i - buffer] = values[i, values.GetLength(1) - 1] / values[i, pivotColumn];
				else
				{
					// -1 is a temporary value. In the DisplayTableau() procedure it will display as "undefined"
					tests[i - buffer] = -1;
				}
			}
			return tests;
		}
		private int FindPivotRow(double[] tests)
		{
			double min = tests.Max();
			try
			{
				// This can only happen if it is a user input through a tableau
				if (min < 0) throw new RegionNotBoundedException();
			}
			catch (RegionNotBoundedException)
			{
				ConsoleHelper.ColourText(ConsoleColor.Red, ConsoleColor.Gray, "Region is not bounded!!\n");
				return -1;
			}
			int row = 0;
			for (int i = 0; i < tests.Length; i++)
			{
				if (tests[i] >= 0 && tests[i] <= min)
				{
					min = tests[i];
					row = i + 1;
				}
			}
			return stage2 ? row + 1 : row;
		}
		private double[,] CreateNextTableau(int pivotRow, int pivotCol)
		{
			if (displayRowOperation) Console.WriteLine("To calculate the next table:");
			// Perform the iteration using the pivot row and column
			double[,] newTableau = new double[values.GetLength(0), values.GetLength(1)];
			double pivot = values[pivotRow, pivotCol];
			
			for (int i = 0; i < values.GetLength(1); i++)
			{
				newTableau[pivotRow, i] = values[pivotRow, i] / pivot;
			}
			
			double multiplier;
			for (int i = 0; i < values.GetLength(0); i++)
			{
				if (i == pivotRow) 
				{
					if (displayRowOperation) Console.WriteLine($"new_row_{i + 1} = pivot_row / {myMath.SigFig(pivot, 5)}");
					continue;
				}
				multiplier = values[i, pivotCol] / pivot;
				if (displayRowOperation) Console.WriteLine($"new_row_{i + 1} = previous_row - pivot_row * {myMath.SigFig(multiplier, 3)}");
				for (int j = 0; j < values.GetLength(1); j++)
				{
					newTableau[i, j] = values[i, j] - multiplier * values[pivotRow, j];
					newTableau[i, j] = myMath.WithinPrecision(Math.Round(newTableau[i, j]), newTableau[i, j])? Math.Round(newTableau[i, j]) : newTableau[i, j];
				}
			}

			if (displayRowOperation) Console.WriteLine();
			return newTableau;
		}

		private void FindBasicVars(bool display, string iteration, bool reductionState = false)
		{
			int[] rowOfBasic = new int[variables.Length - 1];

			// 2 stage coordinate plotting
			(double x, double y) coordinates = (0, 0);
			bool plotted = false;

			// identify if each variable is basic and the row it corresponds to
			for (int i = 0; i < variables.Length - 1; i++)
			{
				bool basic = true;
				int zeroes = 0;
				int ones = 0;
				int oneLoc = 0;
				for (int j = 0; j < values.GetLength(0); j++)
				{
					if (values[j, i] == 0) zeroes++;
					else if (values[j, i] == 1)
					{
						ones++;
						oneLoc = j;
					}
					else
					{
						basic = false;
						break;
					}
				}
				if (basic && ones == 1) rowOfBasic[i] = oneLoc;
				else rowOfBasic[i] = 0;
			}

			List<string> nonBasic = new List<string>();
			if (display) Console.WriteLine("Basic variables: ");
			for (int i = stage2 ? 2 : 1; i < variables.Length - 1; i++)
			{
				if (rowOfBasic[i] == -1) continue;
				if (rowOfBasic[i] == 0)
				{
					nonBasic.Add(variables[i]);
				}
				else
				{
					// gather all other variables on the same row.
					int row = rowOfBasic[i];
					int[] occurences = rowOfBasic.Select((x, j) => x == row ? j : -1).Where(j => j != -1).ToArray();
					List<string> namedOccurences = new List<string>();

					for (int j = 0; j < occurences.Length; j++)
					{
						namedOccurences.Add(variables[occurences[j]]);
						rowOfBasic[occurences[j]] = -1;
					}
					if (display) Console.WriteLine(String.Join(" + ", namedOccurences) + " = " + myMath.SigFig(values[row, variables.Length - 1]));

					// Add to plot marker
					if (dimension == 2 && (namedOccurences[0][0] == 'x' || namedOccurences[0][0] == 'y'))
					{
						// if they both lie on the same line
						if (occurences.Length == 2 && namedOccurences.Contains("x") && namedOccurences.Contains("y"))
						{
							plot.AddLinePointToPlot(row, iteration, values, variables);
							plotted = true;
						}
						else if (namedOccurences[0][0] == 'x')
						{
							coordinates.x = values[row, variables.Length - 1];
						}
						else if (namedOccurences[0][0] == 'y')
						{
							coordinates.y = values[row, variables.Length - 1];
						}
					}
				}
			}
			if (display)
			{
				Console.WriteLine("Non-basic variables: ");
				Console.WriteLine(String.Join(", ", nonBasic) + " = 0\n");
			}

			// Plot the marker if not a line
			if (dimension == 2 && !plotted) plot.PlotMarker(iteration, coordinates.x, coordinates.y, values);
		}

		private bool DetermineStage2()
		{
			for (int i = 2; i < values.GetLength(1); i++)
			{
				if (values[0, i] > 0) return true;
			}
			return false;
		}

		private bool DetermineSolved(List<double[,]> seen)
		{
			SeenState(seen);

			for (int i = 0; i < values.GetLength(1) - 1; i++)
			{
				if (values[0, i] < 0) return false;
			}
			return true;
		}
		
		private void SeenState(List<double[,]> seen)
		{
			// Check for instances if the current array is in 'seen'. This means that the state has been seen before, and the problem is unsolvable.
			foreach (double[,] state in seen)
			{
				bool copy = true;
				for (int i = 0; i < values.GetLength(0); i++)
				{
					for (int j = 0; j < values.GetLength(1); j++)
					{
						if (values[i, j] != state[i, j])
						{
							copy = false;
							break;
						}
					}
					if (!copy) break;
				}
				if (copy) throw new RegionNotBoundedException();
			}
		}

		private Equation ReformulateSlack(Equation constraint, int slackCount) =>
			new Equation(constraint.GetLHSvariables().Concat(new string[] { $"s{myMath.subscriptNums[slackCount]}" }).ToArray(),
				constraint.GetLHSValues().Concat(new double[] { 1 }).ToArray(), "=",
				constraint.GetRHSValues()[0]);

		private Equation ReformulateArtificial(Equation constraint, int slackCount, int artificialCount) =>
			new Equation(constraint.GetLHSvariables().Concat(new string[] { $"a{myMath.subscriptNums[artificialCount]}", $"s{myMath.subscriptNums[slackCount]}" }).ToArray(),
				constraint.GetLHSValues().Concat(new double[] { 1, -1 }).ToArray(), "=",
				constraint.GetRHSValues()[0]);

		private void ReformulateObjective(List<Equation> equations)
		{
			Equation newObjective = new Equation(objective.GetLHSvariables().Concat(objective.GetRHSvariables()).ToArray(),
				objective.GetLHSValues().Concat(objective.GetRHSValues().Select(x => -x)).ToArray(), "=", 0);
			equations.Add(newObjective);
			Console.WriteLine($"The objective function becomes:\n{newObjective}");
		}
		private (int, int, List<int>) ReformulateConstraints(List<Equation> equations)
		{
			// Reformulate the constraints
			int slackSurplus = 0;
			int artificial = 0;
			List<int> rowsOfArtificial = new List<int>();

			Console.WriteLine("Subject to: ");
			foreach (Equation constraint in constraints)
			{
				Equation newConstraint;
				if (constraint.symbol == "=")
				{
					newConstraint = ReformulateSlack(constraint, ++slackSurplus);
					equations.Add(newConstraint);
					Console.WriteLine(newConstraint);
					newConstraint = ReformulateArtificial(constraint, ++slackSurplus, ++artificial);
					rowsOfArtificial.Add(equations.Count() + 1);
				}
				else if (constraint.symbol == "<=") newConstraint = ReformulateSlack(constraint, ++slackSurplus);
				else
				{
					newConstraint = ReformulateArtificial(constraint, ++slackSurplus, ++artificial);
					rowsOfArtificial.Add(equations.Count() + 1);
				}

				equations.Add(newConstraint);
				Console.WriteLine(newConstraint);
			}
			return (slackSurplus, artificial, rowsOfArtificial);
		}
		private void SetUpVariables(int slackSurplus, int artificial)
		{
			if (artificial > 0) variables = new string[] { "A", "P" }.Concat(objective.GetRHSvariables())
					.Concat(Enumerable.Range(1, slackSurplus).Select(x => $"s{myMath.subscriptNums[x]}").ToArray())
					.Concat(Enumerable.Range(1, artificial).Select(x => $"a{myMath.subscriptNums[x]}").ToArray())
					.Concat(new string[] { "RHS" }).ToArray();
			else variables = new string[] { "P" }.Concat(objective.GetRHSvariables())
					.Concat(Enumerable.Range(1, slackSurplus).Select(x => $"s{myMath.subscriptNums[x]}").ToArray())
					.Concat(new string[] { "RHS" }).ToArray();
		}
		private void AddEquationsToTableau(int artificial, List<Equation> equations)
		{
			int artificialOffset = artificial > 0 ? 1 : 0;
			values = new double[equations.Count() + artificialOffset, variables.Length];

			for (int i = 0; i < equations.Count(); i++)
			{
				string[] currentNames = equations[i].GetLHSvariables();
				double[] currentValues = equations[i].GetLHSValues();

				// Add LHS
				for (int j = 0; j < variables.Length - 1; j++)
				{
					if (currentNames.Contains(variables[j])) values[i + artificialOffset, j] = currentValues[Array.IndexOf(currentNames, variables[j])];
					else values[i + artificialOffset, j] = 0;
				}
				// Add RHS
				values[i + artificialOffset, variables.Length - 1] = equations[i].GetRHSValues()[0];
			}
		}
		private void CreateSecondObjective(List<int> rowsOfArtificial)
		{
			values[0, 0] = 1;
			for (int i = 1; i < variables.Length; i++)
			{
				values[0, i] = variables[i][0] != 'a' ? rowsOfArtificial.Sum(row => values[row, i]) : 0;
			}
			Equation secondObjective = new Equation(
				variables.Take(variables.Length - 1).ToArray(),
				Enumerable.Range(0, variables.Length - 1).Select(x => values[0, x]).ToArray(), "=",
				values[0, variables.Length - 1]);
			Console.WriteLine($"As there are artificial variables (>= constraints) a second objective is needed (Σa):\n{secondObjective}");
		}
		private void CreateTableau(int slackSurplus, int artificial, List<int> rowsOfArtificial, List<Equation> equations)
		{			
			SetUpVariables(slackSurplus, artificial);
			AddEquationsToTableau(artificial, equations);

			// Create the next objective function if applicable
			if (artificial > 0) CreateSecondObjective(rowsOfArtificial);
			stage2 = artificial > 0;
		}
		private void Reformulate()
		{
			List<Equation> equations = new List<Equation>();

			Console.WriteLine("The problem can then be put into augmented form as follows: ");
			
			// Reformulate the objective function
			ReformulateObjective(equations);
			(int slackSurplus, int artificial, List<int> rowsOfArtificial) = ReformulateConstraints(equations);

			// Create the initial tableau
			CreateTableau(slackSurplus, artificial, rowsOfArtificial, equations);
		}

		private int LoopStage2()
		{
			int iteration = 0;
			List<double[,]> seen = new List<double[,]>();
			// minimise the 2nd objective
			while (stage2)
			{
				int pivotColumn = ChooseLargestColumn();
				double[] ratios = RatioTest(pivotColumn);
				int pivotRow = FindPivotRow(ratios);
                if (pivotRow == -1) throw new RegionNotBoundedException();
                DisplayTableau(pivotRow, pivotColumn, iteration == 0 ? "Initial tableau:" : $"Iteration {iteration}:", ratios);

				if (dimension == 2 || displayBasic)
				{
					FindBasicVars(displayBasic, iteration == 0 ? "" : $"Iteration {iteration}");
				}

				iteration++;
				values = CreateNextTableau(pivotRow, pivotColumn);
				stage2 = DetermineStage2();
				SeenState(seen);
				seen.Add(values.Clone() as double[,]);
			}
			return iteration;
		}
		private void Reduce2Stage()
		{
			List<int> indexToRemove = variables.Select((x, i) => x.ToLower()[0] == 'a' ? i : -1).Where(i => i != -1).ToList();
			variables = variables.Select((x, i) => indexToRemove.Contains(i) ? "*" : x).Where(x => x != "*").ToArray();
			double[,] tempValues = new double[values.GetLength(0) - 1, variables.Length];
			for (int i = 1; i < values.GetLength(0); i++)
			{
				int counter = 0;
				for (int j = 0; j < values.GetLength(1); j++)
				{
					if (indexToRemove.Contains(j)) continue;
					tempValues[i - 1, counter] = values[i, j];
					counter++;
				}
			}
			values = tempValues;
		}
		private void LoopStage1(bool reduced, int iteration)
		{
			bool solved = false;

			List<double[,]> seen = new List<double[,]>();
			while (!solved)
			{
				int pivotColumn = ChooseSmallestColumn();
				double[] ratios = RatioTest(pivotColumn);
				int pivotRow = FindPivotRow(ratios);
				if (pivotRow == -1) break;
				DisplayTableau(pivotRow, pivotColumn, iteration == 0 ? "Initial tableau:" : reduced ? "Reduced tableau:" : $"Iteration {iteration}:", ratios);

				if (dimension == 2 || displayBasic)
				{
					FindBasicVars(displayBasic, iteration == 0 ? "" : $"Iteration {iteration}", reduced);
				}

				iteration++;
				values = CreateNextTableau(pivotRow, pivotColumn);
				solved = DetermineSolved(seen);
				seen.Add(values.Clone() as double[,]);
				reduced = false;
			}
		}
		public void Solve()
		{
			if (!skipReformulation) Reformulate();
			Console.WriteLine();

			int iteration = 0;
			bool reduced = false;

			if (dimension == 2) plot = new Simplex2DGraph(stage2, values, constraints);

			try
			{
				// identify if 2 stage
				if (stage2)
				{
					// Do the iterations to minimise A
					iteration = LoopStage2();
					DisplayTableau($"Iteration {iteration}:");

					// reduce to a 1 stage problem
					Reduce2Stage();
					reduced = true;
				}

				// solve 1 stage simplex iteratively.
				LoopStage1(reduced, iteration);

				DisplayTableau("Final tableau:");
				FindBasicVars(true, "Final iteration");
				Console.WriteLine($"Maximised at: {variables[0]} = {values[0, values.GetLength(1) - 1]}");

				if (dimension == 2) plot.DisplayPlot();
				else ConsoleHelper.WaitForKey();
			}
			catch (RegionNotBoundedException)
			{
				Console.WriteLine("Region was not bounded!");
				ConsoleHelper.WaitForKey();
			}
		}
	}
}