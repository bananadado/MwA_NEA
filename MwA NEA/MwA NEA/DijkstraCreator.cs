using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
	public class DijkstraCreator : IProblem
	{
		DijkstraSolver problem;
		private char startNode;
		public void GenerateQuestion(Random rnd)
		{
			int difficulty = new OptionScroller("Choose Difficulty: ", 1, 6).Select();
			Graph graph = new Graph(rnd.Next(0, 2) + (difficulty - 1) * 2 + 4);
			graph.GenerateGraph(rnd, difficulty <= 2 || rnd.Next(0, 2) != 0);
			problem = new DijkstraSolver(graph);
			startNode = graph.GetNodeNames()[rnd.Next(graph.GetNodeNames().Length)];

			Console.WriteLine($"Perform Dijkstra's algorithm from {startNode} on the following network:\n{graph}");
		}

		public void InputQuestion()
		{
			int vertices = new OptionScroller("Number of vertices: ", 3, 26).Select();
			Console.WriteLine($"Number of vertices: {vertices}");
			Graph graph = new Graph(vertices);
			startNode = graph.GetNodeNames()[new OptionScroller("Start node: ", 0, vertices - 1, true).Select()];
			Console.WriteLine($"Start node: {startNode}");
			graph.InputGraph(new Menu(new List<string>() { "Undirected (symmetrical) Graph", "Directed Graph"}).SelectOption() == 0, true);
			problem = new DijkstraSolver(graph);
			Console.WriteLine(graph + Environment.NewLine);
		}

		public void GenerateAnswer()
		{
			if (!(problem is null)) problem.Solve(startNode);
			else Console.WriteLine("There is no problem to solve!!");
			ConsoleHelper.WaitForKey();
		}
	}
}
