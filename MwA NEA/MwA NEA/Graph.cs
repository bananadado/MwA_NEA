using System;
using System.Collections.Generic;
using System.Linq;

namespace MwA_NEA
{
	public class Graph
	{
		// A 0 represents no connection
		private double[,] matrix;
		private char[] nodeNames;
		
		public Graph(int numberOfNodes)
		{
			CreateMatrix(numberOfNodes);
		}
		private void CreateMatrix(int numberOfNodes)
		{
			matrix = new double[numberOfNodes, numberOfNodes];
			nodeNames = new char[numberOfNodes];
			for (int i = 0; i < numberOfNodes; i++)
			{
				nodeNames[i] = (char)(i + 65);
			}
		}
		private void GenerateConnectedGraph(Random rnd, int lowerBound, int upperBound)
		{
			// Random Walk, uniform spanning tree
			HashSet<char> nodes = new HashSet<char>(nodeNames);
			HashSet<char> visited = new HashSet<char>();

			char currentNode = nodeNames[rnd.Next(nodeNames.Length)];
			nodes.Remove(currentNode);
			visited.Add(currentNode);

			while (nodes.Count > 0)
			{
				char neighbourNode = nodeNames[rnd.Next(nodeNames.Length)];
				if (!visited.Contains(neighbourNode) && currentNode != neighbourNode)
				{
					matrix[currentNode - 'A', neighbourNode - 'A'] = rnd.Next(lowerBound, upperBound);
					matrix[neighbourNode - 'A', currentNode - 'A'] = matrix[currentNode - 'A', neighbourNode - 'A'];

					nodes.Remove(neighbourNode);
					visited.Add(neighbourNode);
				}
				currentNode = neighbourNode;
			}
		}
		private void AddRandomEdges(int numOfEdges, Random rnd, int lowerBound, int upperBound, bool symmetry)
		{
			HashSet<(int, int)> available = new HashSet<(int, int)>();
			// If symmetrical (undirected), look for locations where both sides are empty
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				for (int j = symmetry? i + 1 : 0; j < matrix.GetLength(0); j++)
				{
					if (matrix[i, j] == 0 && i != j) available.Add((i, j));
				}
			}
			for (int i = 0; i < numOfEdges; i++)
			{
				(int row, int col) = available.ToArray()[rnd.Next(available.Count)];
				available.Remove((row, col));
				matrix[row, col] = rnd.Next(lowerBound, upperBound);
				if (symmetry) matrix[col, row] = matrix[row, col];
			}
		}
		public void GenerateGraph(Random rnd, bool symmetry = true)
		{
			int numberOfExtraEdges = rnd.Next(nodeNames.Length - 1, nodeNames.Length * (nodeNames.Length - 1) / 2 + 1) - (nodeNames.Length - 1);
			
			// Randomly generates a range that the lengths can be
			int lowerBound = rnd.Next(1, 20);
			int upperBound = lowerBound + 4 * (int)Math.Round(Math.Pow(lowerBound, 0.5));
			
			// First thing is to create a connected graph.
			GenerateConnectedGraph(rnd, lowerBound, upperBound);

			// Then add random extra edges
			AddRandomEdges(numberOfExtraEdges, rnd, lowerBound, upperBound, symmetry);
		}

		public void InputGraph(bool symmetry = true, bool dijkstra = false)
		{
			Console.WriteLine("Enter adjacency matrix (anything not a number is automatically 0, LHS is To and top row is From)");
			matrix = new MatrixEnter(matrix, nodeNames.Select(x => x.ToString()).ToArray(), true, symmetry, !dijkstra).Select();
			ConsoleHelper.ClearLine(Console.CursorTop - 1);
		}

		public double[,] GetAdjacencyMatrix() => matrix;
		public char[] GetNodeNames() => nodeNames;

		public Dictionary<char, double> GetConnections(char node)
		{
			if (node - 65 >= nodeNames.Length) return null;
			Dictionary<char, double> connections = new Dictionary<char, double>();
			for (int i = 0; i < nodeNames.Length; i++)
			{
				if (matrix[node - 65, i] != 0) connections.Add((char)(i + 65), matrix[node - 65, i]);
			}
			return connections;
		}

		public override string ToString()
		{
			if (matrix is null) return "";
			List<string> lines = new List<string>();
			int[] largestSize = new int[nodeNames.Length];
			for (int i = 0; i < largestSize.Length; i++)
			{
				for (int j = 0; j < matrix.GetLength(0); j++)
				{
					largestSize[i] = Math.Max(largestSize[i], matrix[j, i].ToString().Length);
				}
			}
			lines.Add(" |");
			foreach (char node in nodeNames) lines.Add($"{node}|");
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				lines[0] += $"{nodeNames[i]}".PadLeft(largestSize[i])+"|";
				for (int j = 0; j < matrix.GetLength(0); j++)
				{
					lines[j + 1] += $"{(matrix[j, i] == 0? "-": matrix[j, i].ToString())}".PadLeft(largestSize[i])+"|";
				}
			}
			return String.Join("\n", lines.ToArray());
		}
	}
}
