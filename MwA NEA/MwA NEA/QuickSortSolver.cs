using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
    public class QuickSortSolver<T>
    {
        private bool asc;
        private T[] values;
        private bool[] sorted;

        public QuickSortSolver(T[] values, bool ascending) => (this.values, asc, sorted) = (values, ascending, new bool[values.Length]);

        private List<int> FindPivots()
        {
            List<int> pivots = new List<int>();
            for (int i = 0; i < sorted.Length; i++)
            {
                if (sorted[i] == false && (i == 0 || sorted[i - 1] == true)) pivots.Add(i);
            }
            return pivots;
        }

        private void DisplayFinal()
        {
			Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(String.Join(", ", values));
            Console.ForegroundColor = ConsoleColor.Gray;
		}
        private void DisplayIteration(List<int> pivots)
        {
            for (int i = 0; i < sorted.Length; i++)
            {
                if (sorted[i]) Console.ForegroundColor = ConsoleColor.Green;
                else if (pivots.Contains(i)) Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{values[i]}{(i != sorted.Length - 1? ", " : "")}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine();
        }
              
		private double CMP(T x, T y) => (dynamic)x - (dynamic)y;

		private List<T> AddSorted(int start)
        {
            List<T> append = new List<T>();
			for (int i = start; i < sorted.Length; i++)
			{
				if (!sorted[i]) break;
                append.Add(values[i]);
			}
            return append;
		}
		private T[] PerformSwaps(List<int> pivots)
        {
            List<T> swaps = new List<T>();

            // Append all sorted items before the first pivot
            swaps = swaps.Concat(AddSorted(0)).ToList();
			foreach (int loc in pivots)
            {                
                // Sort the parts into 2 categories: <= (leq) or > (gt) pivot
                List<T> leq = new List<T>();
                List<T> gt = new List<T>();
                int append = sorted.Length;

                for (int i = loc + 1; i < sorted.Length; i++)
                {
                    if (sorted[i])
                    {
                        append = i;
                        break;
                    }
                    if (CMP(values[i], values[loc]) <= 0) leq.Add(values[i]);
                    else gt.Add(values[i]);
                }

                // Add the categories in the correct order
                swaps = swaps.Concat(asc ? leq : gt).ToList();
                sorted[swaps.Count()] = true;
                swaps.Add(values[loc]);
				swaps = swaps.Concat(asc ? gt : leq).ToList();

                //Append all sorted items before the next pivot
                swaps = swaps.Concat(AddSorted(append)).ToList();
			}
            return swaps.ToArray();
        }

        public void Solve()
        {
            Console.WriteLine();
            while (!sorted.All(x => x == true))
            {
				List<int> pivots = FindPivots();
				DisplayIteration(pivots);
                values = PerformSwaps(pivots);
			}
			DisplayFinal();
			ConsoleHelper.WaitForKey();
        }
    }
}
