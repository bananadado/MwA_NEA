using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
    public class QuickSortCreator : IProblem
    {
        QuickSortSolver<char> cProblem;
        QuickSortSolver<double> dProblem;

		public void GenerateQuestion(Random rnd)
        {
            int difficulty = new OptionScroller("Choose Difficulty: ", 1, 5).Select();
            bool alphabet = rnd.Next(0, 2) == 1;
            bool ascending = rnd.Next(0, 2) == 1;

			if (!alphabet) // numbers
			{
				double[] dValues = new double[rnd.Next(4, 7) + (difficulty - 1) * 2];
				for (int i = 0; i < dValues.Length; i++)
				{
                    if (difficulty <= 2) dValues[i] = rnd.Next(0, 200);
					else dValues[i] = rnd.Next(-99, 200);
				}
				dProblem = new QuickSortSolver<double>(dValues, ascending);
				Console.WriteLine($"Sort by {(ascending ? "ascending" : "descending")}: {String.Join(", ", dValues)}");
			}
			else // letters
            {
                char[] cValues = new char[rnd.Next(4, 7) + (difficulty - 1) * 2];
                for (int i = 0; i < cValues.Length; i++)
                {
                    cValues[i] = (char)(rnd.Next(0, 26) + 65);
                }
                cProblem = new QuickSortSolver<char>(cValues, ascending);
				Console.WriteLine($"Sort by {(ascending ? "ascending" : "descending")}: {String.Join(", ", cValues)}");
			}
        }
        
        public void InputQuestion()
        {
            Console.WriteLine("Enter a list of letters or numbers separated by commas:");
            List<string> input = Console.ReadLine().Split(',').Select(x => x.Trim()).ToList();
            bool ascending = new Menu(new List<string>() { "Sort ascending", "Sort descending" }).SelectOption() == 0;
            Console.WriteLine("Bad values are omitted.");

            if (double.TryParse(input[0], out double d))
            {
                List<double> dValues = new List<double>();
                foreach (string s in input)
                {
                    if (double.TryParse(s, out double num)) dValues.Add(num);
                }
                dProblem = new QuickSortSolver<double>(dValues.ToArray(), ascending);
            }
            else
            {
                List<char> cValues = new List<char>();
                foreach (string s in input)
                {
                    if (char.TryParse(s, out char c)) cValues.Add(c);
                }
                cProblem = new QuickSortSolver<char>(cValues.ToArray(), ascending);
            }
        }
        public void GenerateAnswer()
        {
            if (!(cProblem is null)) cProblem.Solve();
            else if (!(dProblem is null)) dProblem.Solve();
			else Console.WriteLine("There is no problem to solve!");
        }
    }
}
