using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
	public class MSTCreator : IProblem
	{
		MSTSolver problem;
		bool prims;
		bool kruskals;
		public void GenerateQuestion(Random rnd)
		{
			int difficulty = new OptionScroller("Choose Difficulty: ", 1, 6).Select();
			Graph graph = new Graph(rnd.Next(0, 2) + (difficulty - 1) * 2 + 4);
			graph.GenerateGraph(rnd);
			problem = new MSTSolver(graph);
			prims = rnd.Next(0, 2) == 0;
			kruskals = !prims;

			Console.WriteLine($"Perform {(prims ? "Prim" : "Kruskal")}'s algorithm on the following network:\n{graph}\n");
		}
		private void EvaluateChoice(int choice)
		{
			if (choice == 0)
			{
				prims = true;
				kruskals = false;
			}
			else if (choice == 1)
			{
				prims = false;
				kruskals = true;
			}
			else
			{
				prims = true;
				kruskals = true;
			}
		}
		public void InputQuestion()
		{
			int vertices = new OptionScroller("Number of vertices: ", 3, 26).Select();
			Console.WriteLine($"Number of vertices: {vertices}");
			Graph graph = new Graph(vertices);

			EvaluateChoice(new Menu(new List<string>() { "Solve Prim's", "Solve Kruskal's", "Both" }).SelectOption());
			if (prims) Console.WriteLine("Solve using Prim's");
			if (kruskals) Console.WriteLine("Solve using Kruskal's");

			graph.InputGraph();
			Console.WriteLine(graph + Environment.NewLine);
			problem = new MSTSolver(graph);
		}
		public void GenerateAnswer()
		{
			if (problem is null) Console.WriteLine("There is no problem to solve!!");
			else
			{
				if (prims) problem.SolvePrims();
				if (kruskals) problem.SolveKruskals();
			}
			ConsoleHelper.WaitForKey();
		}
	}
}
