using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
	public class DisjointSet
	{
		private int[] set;
		private Dictionary<char, int> map;
		public DisjointSet(char[] nodes) 
		{
			// Creates a unique set for each node
			set = new int[nodes.Length];
			map = new Dictionary<char, int>();

			for (int i = 0; i < nodes.Length; i++)
			{
				set[i] = i;
				map.Add(nodes[i], i);
			}
		}

		private int Find(int node)
		{
			if (set[node] != node) return Find(set[node]);
			return node;
		}

		public bool Unify(string edge)
		{
			int p1 = Find(map[edge[0]]);
			int p2 = Find(map[edge[1]]);

			if (p1 == p2) return false;
			set[p2] = p1;
			return true;
		}
	}
}
