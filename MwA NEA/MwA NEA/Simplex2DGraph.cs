using ScottPlot.Drawing.Colormaps;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace MwA_NEA
{
    public class Simplex2DGraph
    {
        private ScottPlot.Plot plot;
        public Simplex2DGraph(bool stage2, double[,] values, List<Equation> constraints)
		{
			plot = new ScottPlot.Plot();
			plot.Palette = ScottPlot.Palette.OneHalfDark;
			plot.Title($"Linear Programming Problem");
			plot.Style(ScottPlot.Style.Black);
			Bitmap image = new Bitmap("A_black_image.jpg");
			plot.Style(figureBackgroundImage: image);
			plot.XAxis.TickLabelStyle(color: Color.WhiteSmoke, fontName: "comic sans ms");
			plot.YAxis.TickLabelStyle(color: Color.WhiteSmoke, fontName: "comic sans ms");

			FindPolygon(stage2, values, constraints);
		}
		private (double, double) FindIntersection(double m1, double c1, double m2, double c2)
        {
            // parallel
            if (m1 == m2) return (-1, -1);

            // handles equations in the form x = c
            if (double.IsInfinity(m1)) return (c1, m2 * c1 + c2);
            if (double.IsInfinity(m2)) return (c2, m1 * c2 + c1);

            double x = (c2 - c1) / (m1 - m2);
            double y = m1 * x + c1;
            return (x, y);
        }

        private List<(double, double)> FindAllIntersections(List<(double, double)> slopeIntercept)
        {
            List<(double, double)> lineIntersections = new List<(double, double)>();
            for (int i = 0; i < slopeIntercept.Count(); i++)
            {
                for (int j = i + 1; j < slopeIntercept.Count(); j++)
                {
                    (double x, double y) = FindIntersection(slopeIntercept[i].Item1, slopeIntercept[i].Item2, slopeIntercept[j].Item1, slopeIntercept[j].Item2);
                    if (x < 0 || y < 0) continue;
                    lineIntersections.Add((x, y));
                }
            }
            return lineIntersections;
        }

        private List<(double, double)> ParseConstraintsToGraph(int stage2Offset, double[,] values)
        {
            List<Func<double, double?>> functions = new List<Func<double, double?>>();
            double xMax = 0;
            double yMax = 0;

            List<(double, double)> lineIntersections = new List<(double, double)>() { (0, 0) };
            List<(double, double)> slopeIntercept = new List<(double, double)>();

            // create all the necessary functions
            for (int i = stage2Offset; i < values.GetLength(0); i++)
            {
                double xcoefficient = values[i, stage2Offset];
                double ycoefficient = values[i, stage2Offset + 1];
                double yintercept = values[i, values.GetLength(1) - 1];

                // Checks if the y coefficient is 0 - this would result in a divide by 0 error.
                // Adds the gradient (m) along with the y intercept (c) to the list "gradientIntercepts" which is used to find all intersections
                if (ycoefficient == 0)
                {
                    plot.AddVerticalLine(yintercept / xcoefficient);
                    slopeIntercept.Add((double.PositiveInfinity, yintercept / xcoefficient));
                }
                else
                {
                    functions.Add(new Func<double, double?>((x) => (xcoefficient / ycoefficient) * -x + yintercept / ycoefficient));
                    slopeIntercept.Add((-xcoefficient / ycoefficient, yintercept / ycoefficient));
                }

                // Find axis and add to list of intersection coords
                if (xcoefficient > 0)
                {
                    lineIntersections.Add((yintercept / xcoefficient, 0));
                    xMax = Math.Max(yintercept / xcoefficient, xMax);
                }
                if (ycoefficient > 0)
                {
                    lineIntersections.Add((0, yintercept / ycoefficient));
                    yMax = Math.Max(yintercept / ycoefficient, yMax);
                }
            }
            foreach (Func<double, double?> func in functions)
            {
                plot.AddFunction(func, lineWidth: 2);
            }
            plot.SetAxisLimits(0, xMax + xMax / 5, 0, yMax + yMax / 5);

            return lineIntersections.Concat(FindAllIntersections(slopeIntercept)).ToList();
        }
        private List<(double, double)> ValidateCoords(List<(double, double)> lineIntersections, List<Equation> constraints)
        {
            // Find all the vertices of the feasible region
            // Loop through and see if the intersection satisfies all the constraints
            List<(double, double)> validatedCoords = new List<(double, double)>();
            foreach ((double x, double y) in lineIntersections)
            {
                bool isInFR = true;
                foreach (Equation constraint in constraints)
                {
                    double LHS = constraint.GetLHSValues()[0] * x + constraint.GetLHSValues()[1] * y;
                    if (!((constraint.symbol == "<=" && LHS <= constraint.GetRHSValues()[0])
                        || (constraint.symbol == ">=" && LHS >= constraint.GetRHSValues()[0])
                        || (constraint.symbol == "=" && LHS == constraint.GetRHSValues()[0])
                        || myMath.WithinPrecision(LHS, constraint.GetRHSValues()[0])))
                    {
                        isInFR = false;
                        break;
                    }
                }
                if (isInFR)
                {
                    validatedCoords.Add((x, y));
                }
            }
            return validatedCoords;
        }
        private (double[], double[]) FindPolygonRoute(List<(double, double)> coords)
        {
            (double x, double y) mean = (coords.Sum(x => x.Item1) / coords.Count, coords.Sum(x => x.Item2) / coords.Count);
            coords.Sort((x1, x2) => myMath.Argument(x1.Item1 - mean.x, x1.Item2 - mean.y).CompareTo(myMath.Argument(x2.Item1 - mean.x, x2.Item2 - mean.y)));
            return (coords.Select(x => x.Item1).ToArray(), coords.Select(x => x.Item2).ToArray());
        }
        private void FindPolygon(bool stage2, double[,] values, List<Equation> constraints)
        {
            int stage2Offset = stage2 ? 2 : 1;
            // Find the polygon that is the feasible region and add it to the graph
            List<(double, double)> lineIntersections = ParseConstraintsToGraph(stage2Offset, values);
            List<(double, double)> validatedCoords = ValidateCoords(lineIntersections, constraints);
            (double[] xCoords, double[] yCoords) = FindPolygonRoute(validatedCoords);
            plot.AddPolygon(xCoords.ToArray(), yCoords.ToArray(), plot.GetNextColor(0.5));
        }

        
        public void AddLinePointToPlot(int row, string iteration, double[,] values, string[] variables)
        {
            plot.AddFunction(new Func<double, double?>((x) => -1 * x + values[row, variables.Length - 1]), color: Color.DarkBlue);
            plot.AddText($"{iteration} (x + y = {values[row, variables.Length - 1]})", 0, 0, size: 10, color: Color.DarkBlue);
        }
        public void PlotMarker(string iteration, double x, double y, double[,] values)
        {
            var marker = plot.AddMarker(x, y, size: 7);
            marker.Text = $"{iteration} ({myMath.SigFig(x)}, {myMath.SigFig(y)}), P = {myMath.SigFig(values[0, values.GetLength(1) - 1])}";
            marker.TextFont.Color = Color.White;
            marker.TextFont.Alignment = Alignment.LowerLeft;
            marker.TextFont.Size = 12;
            marker.TextFont.Name = "Comic Sans MS";
        }

        private void LaunchInteractiveWindow()
        {
            new ScottPlot.FormsPlotViewer(plot, windowWidth: 800, windowHeight: 800, windowTitle: "Simplex Problem").ShowDialog();
        }
        public void DisplayPlot()
        {
            plot.SetViewLimits(0, double.PositiveInfinity, 0, double.PositiveInfinity);
            Thread interactiveWindow = new Thread(LaunchInteractiveWindow);
            interactiveWindow.Start();
            Console.WriteLine("Would you like to save it as an image?");
            string response = new Menu(new List<string>() { "Yes", "No" }, '>').Select();

            if (response == "Yes")
            {
                Console.Write("Enter a name to save the file as: ");
                try
                {
                    plot.SaveFig($"images\\{Console.ReadLine()}.png");
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    Console.WriteLine("Unfortunately you couldn't name it that.");
                }
            }
            ConsoleHelper.WaitForKey();

            Console.WriteLine("Please wait for the interactive window to close (you can speed this up by closing it manually)..");
            while (interactiveWindow.IsAlive) interactiveWindow.Abort();
            ConsoleHelper.ClearLine(Console.CursorTop - 1);
        }
    }
}
