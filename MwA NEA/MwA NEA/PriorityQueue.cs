using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
	public class PriorityQueue
	{
		private List<DijkstraNode> heap;
		private int length;

		public PriorityQueue()
		{
			heap = new List<DijkstraNode>();
			length = 0;
		}

		public void Enqueue(DijkstraNode node)
		{
			heap.Add(node);
			int pos = length++;

			// sift-up
			while (pos > 0)
			{
				if (heap[(pos - 1) / 2].weight > node.weight)
				{
					(heap[(pos - 1) / 2], heap[pos]) = (heap[pos], heap[(pos - 1) / 2]);
					pos = (pos - 1) / 2;
				}
				else break;
			}
		}
		public DijkstraNode Dequeue()
		{
			length--;
			DijkstraNode node = heap[0];
			heap[0] = heap[length];
			heap.RemoveAt(length);
			int pos = 0;

			if (length > 1)
			{
				// sift-down
				while (true)
				{
					double current = heap[pos].weight;
					double left = (pos * 2 + 1 >= length) ? double.PositiveInfinity : heap[pos * 2 + 1].weight;
					double right = (pos * 2 + 2 >= length) ? double.PositiveInfinity : heap[pos * 2 + 2].weight;

					if (current <= left && current <= right) break;

					if (left <= right)
					{
						(heap[pos], heap[pos * 2 + 1]) = (heap[pos * 2 + 1], heap[pos]);
						pos = pos * 2 + 1;
					}
					else
					{
						(heap[pos], heap[pos * 2 + 2]) = (heap[pos * 2 + 2], heap[pos]);
						pos = pos * 2 + 2;
					}
				}
			}
			
			return node;
		}
		public int GetLength() => length;
	}
}
