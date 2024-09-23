using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
	public class DijkstraNode
	{
		public char name;
		public int order;
		public bool visited;
		public string route;
		private List<double> previous;
		public double weight;

		public DijkstraNode(char name, bool start)
		{
			this.name = name;
			visited = false;
			previous = new List<double>();
			route = "";

			if (start)
			{
				weight = 0;
				previous.Add(0);
				route += name;
			}
			else weight = double.PositiveInfinity;
		}

		public void DecreaseWeight(double value)
		{
			previous.Add(value);
			weight = value;
		}

		public override string ToString()
		{
			if (double.IsPositiveInfinity(weight)) return $"{name}: Not Visited";
			string bottomLine = $"|{(previous.Count == 1 ? $"{previous[0]}".PadRight(weight.ToString().Length) : String.Join(",", previous))}";
			string topLine = "|" + $"{order}".PadRight(bottomLine.Length / 2 - 1) + "|" + $"{weight}|".PadLeft(bottomLine.Length / 2 - (bottomLine.Length+1)%2);
			bottomLine += "|".PadLeft(topLine.Length - bottomLine.Length);
			return $"{name}: {String.Join(",", route.ToCharArray())}\n{topLine}\n{bottomLine}";
		}
	}
}
