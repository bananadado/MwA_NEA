using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MwA_NEA
{
    public static class Tutorial
    {
        public static void StartTutorial()
        {
            ConsoleHelper.SlowText("A Level further maths 'Modelling with Algorithms' tool tutorial");
            ConsoleHelper.WaitForKey();

            ConsoleHelper.ColourText(ConsoleColor.Green, ConsoleColor.Gray, "\nEntering Equations:", true);
            ConsoleHelper.SlowText("To enter an equation you will see something like this:\n_x₁ + _x₂ <= _");
            ConsoleHelper.SlowText("You can enter in numbers where there are underscores \"_\"");
            ConsoleHelper.ColourText(ConsoleColor.Red, ConsoleColor.Gray, "Note that: any underscores left in the equation will automatically be parsed as '0'", true);
            ConsoleHelper.SlowText("You can move around in the equation by pressing tab/enter/right arrow to move right and left arrow to move left.");
            ConsoleHelper.SlowText("If you press enter while you're editing the last variable or press \'esc\' at any point, it will stop editing the equation.");
            ConsoleHelper.ColourText(ConsoleColor.Magenta, ConsoleColor.Gray, "Have a go now!", true);
            
            string[] tutorialArray = new string[] {"x", "y"};
            double[] tutorialValues = new EquationEnter(new string[] { "x + ", "y <=", "" }).Select();
            Console.WriteLine(new Equation(tutorialArray, tutorialValues.Take(2).ToArray(), "<=", tutorialValues[2]));
            ConsoleHelper.WaitForKey();

            ConsoleHelper.ColourText(ConsoleColor.Green, ConsoleColor.Gray, "\nEntering tables", true);
            ConsoleHelper.SlowText("To enter a table (such as a Simplex Tableau or an adjacency matrix) you will see something like this:");
            ConsoleHelper.SlowText(@"
|A|B |C|D|
|_| _|_|_|
|_| _|_|_|
Enter");
            ConsoleHelper.SlowText("The same logic applies as when entering an equation.");
            ConsoleHelper.SlowText("The only difference is that pressing enter moves you down a column instead.");
            ConsoleHelper.SlowText("To exit you can either hit escape or navigate down to the \"Exit\" button and press enter.");
            ConsoleHelper.ColourText(ConsoleColor.Red, ConsoleColor.Gray, "If you see a \"/\" this means that box cannot be filled in (such as to prevent loops in a graph)\n");
			ConsoleHelper.ColourText(ConsoleColor.Magenta, ConsoleColor.Gray, "Have a go now!", true);

            Graph tutorialGraph = new Graph(5);
            tutorialGraph.InputGraph();
            Console.WriteLine(tutorialGraph);
			ConsoleHelper.WaitForKey();

            ConsoleHelper.ColourText(ConsoleColor.Green, ConsoleColor.Gray, "\nThe Simplex Algorithm", true);
            ConsoleHelper.ColourText(ConsoleColor.Magenta, ConsoleColor.Gray, "Pivot rows are displayed in magenta.\n");
            ConsoleHelper.ColourText(ConsoleColor.Yellow, ConsoleColor.Gray, "Pivot columns are displayed in yellow.\n");
            ConsoleHelper.SlowText("Basic variables and row operations can be turned on/off before running.");
            ConsoleHelper.SlowText("The program accepts both constraints (entered through an equation) and a tableau (entered through a table)");
            ConsoleHelper.WaitForKey();

            ConsoleHelper.ColourText(ConsoleColor.Green, ConsoleColor.Gray, "\nDijkstra's Algorithm", true);
            ConsoleHelper.SlowText("You can generate a question or add a graph through an adjacency matrix.");
            ConsoleHelper.SlowText("You can select whether or not the graph is undirected or directed.");
            ConsoleHelper.SlowText("The program will display the boxes you find in the exam as well as the route to each node.");
            ConsoleHelper.WaitForKey();

            ConsoleHelper.ColourText(ConsoleColor.Green, ConsoleColor.Gray, "\nMST algorithms", true);
            ConsoleHelper.SlowText("The same as Dijkstra's algorithm but the graph has to be undirected and you can enter negative values.");
            ConsoleHelper.SlowText("You can choose to compute Prim's or Kruskal's (or both!)");
            ConsoleHelper.WaitForKey();

			ConsoleHelper.ColourText(ConsoleColor.Green, ConsoleColor.Gray, "\nQuickSort Algorithm", true);
			ConsoleHelper.ColourText(ConsoleColor.Red, ConsoleColor.Gray, "Pivots are displayed in red.\n");
			ConsoleHelper.ColourText(ConsoleColor.Green, ConsoleColor.Gray, "Sorted values are displayed in green.\n");
			ConsoleHelper.SlowText("You can sort using characters or numbers.");
            ConsoleHelper.WaitForKey("Press any key to exit.");
		}
    }
}
