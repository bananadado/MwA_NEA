using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
	public class MSTSolver
	{
		private Graph network;

		public MSTSolver(Graph network) => this.network = network;

		private Queue<(string, double)> SortAllEdges()
		{
			List<(string, double)> allEdges = new List<(string, double)>();

			// Get all edges
			double[,] matrix = network.GetAdjacencyMatrix();
			for (int i = 0; i < matrix.GetLength(0) - 1; i++)
			{
				for (int j = i + 1; j < matrix.GetLength(1); j++)
				{
					if (matrix[i, j] != 0) allEdges.Add(($"{(char)(i + 65)}{(char)(j + 65)}", matrix[i, j]));
				}
			}

			// sort by weight
			return new Queue<(string, double)>(allEdges.OrderBy(x => x.Item2));
		}
		public void SolveKruskals()
		{
			DisjointSet sets = new DisjointSet(network.GetNodeNames());

			int accepted = network.GetNodeNames().Length - 1;
			// edge name, weight, in MST?
			List<(string, double, bool)> edges = new List<(string, double, bool)>();
			Queue<(string, double)> allEdges = SortAllEdges();
								
			while (accepted > 0)
			{
				if (allEdges.Count == 0)
				{
					ConsoleHelper.ColourText(ConsoleColor.Red, ConsoleColor.Gray, "The tree is not connected!\n");
					break;
				}
                (string edge, double weight) = allEdges.Dequeue();
				bool inMST = sets.Unify(edge);
				edges.Add((edge, weight, inMST));
				if (inMST) accepted--;
			}

			Console.WriteLine($"Kruskal's Algorithm: \nMST of weight {edges.Sum(x => x.Item3? x.Item2 : 0)}");
			for (int i = 0; i < edges.Count; i++)
			{
				Console.WriteLine($"{i + 1}: {edges[i].Item1} - {edges[i].Item2}  {(edges[i].Item3? "☻" : "X")}");
			}
		}
		public void SolvePrims()
		{
			List<char> visited = new List<char>() { 'A' };
			List<(string, double)> tree = new List<(string, double)>();

			for (int i = 0; i < network.GetNodeNames().Length - 1; i++)
			{
				double currentMin = double.PositiveInfinity;
				char currentNode = 'A';
				char currentNodePair = 'A';

				foreach(char node in visited)
				{
					foreach(KeyValuePair<char, double> connection in network.GetConnections(node))
					{
						if (connection.Value < currentMin && !visited.Contains(connection.Key))
						{
							currentMin = connection.Value;
							currentNode = connection.Key;
							currentNodePair = node;
						}
					}
				}

				if (double.IsInfinity(currentMin))
				{
                    ConsoleHelper.ColourText(ConsoleColor.Red, ConsoleColor.Gray, "The tree is not connected!\n");
                    break;
                }

				visited.Add(currentNode);
				tree.Add(($"{currentNodePair}{currentNode}", currentMin));
			}
			Console.WriteLine($"Prim's Algorithm: \nMST of weight {tree.Sum(x => double.IsInfinity(x.Item2) ? 0 : x.Item2)}");
			for (int i = 0; i < tree.Count; i++)
			{
				Console.WriteLine($"{i + 1}: {tree[i].Item1} - {tree[i].Item2}");
			}
            Console.WriteLine();
        }
	}
}
