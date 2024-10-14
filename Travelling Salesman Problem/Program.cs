using System;
using System.Diagnostics;
using static Travelling_Salesman_Problem.Map;
using static Travelling_Salesman_Problem.TravellingSalesmanProblem;

namespace Travelling_Salesman_Problem
{
	class Program
	{
		static void Main(string[] args)
		{
			
			Stopwatch sw = new Stopwatch();
			int numOfTries = 10;

			TravellingSalesmanProblem tsp = new TravellingSalesmanProblem();

			tsp.setMap(@"G:\Portfolio\Algorithms\Travelling Salesman Problem\CSV\map.csv");
			tsp.setMainNode("Ampuero");

			tsp.map.Print();

			Solution solution = null;

			/*
			 * method 1 test 
			 */

			Console.WriteLine("Testing [NearestNeightbor_Solver]");
			long totalTime = 0;
			for (int i = 0; i < numOfTries; i++)
			{
				sw.Start();
				solution = tsp.Solve(Solution.SolvingMethods.NearestNeightbor_Solver);
				sw.Stop();
				totalTime += sw.ElapsedMilliseconds;
			}

			if (solution != null)
			{
				solution.Print();
				Console.WriteLine($"Time elapsed: {totalTime / numOfTries}ms on ({numOfTries} tries) average");
			}
			Console.WriteLine("\n\n\n");
			



			////////////////////////////////////////////////////////////////////////////////////////////




			/*
			 * method 2 test
			 */


			//Console.WriteLine("Tests of brute force:");


			
			Console.WriteLine("Testing [BruteForce_Solver]");
			solution = null;
			totalTime = 0;

			sw.Start();
			solution = tsp.Solve(Solution.SolvingMethods.BruteForce_Solver);
			sw.Stop();

			totalTime += sw.ElapsedMilliseconds;
			Console.WriteLine("Solution: " + solution);
			if (solution != null)
			{
				solution.Print();
				Console.WriteLine($"Time elapsed: {totalTime / numOfTries}ms on ({numOfTries} tries) average");
			}
			Console.WriteLine("\n\n\n");
			

			/*
			Console.WriteLine("tests of brute force:\n");

			TravellingSalesmanProblem tsp = new TravellingSalesmanProblem();

			Node node1 = new Node("Ampuero");
			Node node12 = new Node("Limpias");
			Node node13 = new Node("Colindres");

			
			node1.connectTo(node12, 5);
			node12.connectTo(node13, 1);
			*/
		}
	}
}
