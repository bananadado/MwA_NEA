using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
	public class DijkstraSolver
	{
		Graph network;

		public DijkstraSolver(Graph network) => this.network = network;

		private Dictionary<char, DijkstraNode> SetUpDict(char startNode)
		{
			Dictionary<char, DijkstraNode> nodeDict = new Dictionary<char, DijkstraNode>();
			foreach (char c in network.GetNodeNames()) nodeDict.Add(c, new DijkstraNode(c, startNode == c));
			return nodeDict;
		}
		public void Solve(char startNode)
		{
			Dictionary<char, DijkstraNode> nodeDict = SetUpDict(startNode);
			PriorityQueue q = new PriorityQueue();
			q.Enqueue(nodeDict[startNode]);
			int order = 1;

			while (q.GetLength() > 0)
			{
				DijkstraNode currentNode = q.Dequeue();
				if (currentNode.visited) continue;
				currentNode.visited = true;
				currentNode.order = order++;
				
				foreach(KeyValuePair<char, double> node in network.GetConnections(currentNode.name))
				{
					if (currentNode.weight + node.Value < nodeDict[node.Key].weight)
					{
						nodeDict[node.Key].DecreaseWeight(currentNode.weight + node.Value);
						nodeDict[node.Key].route = currentNode.route + node.Key;
						q.Enqueue(nodeDict[node.Key]);
					}
				}
			}
			foreach (DijkstraNode node in nodeDict.Values) Console.WriteLine(node);
		}
	}
}
