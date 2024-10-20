using System;
using System.Collections.Generic;
using System.Linq;
using static Travelling_Salesman_Problem.Map;

namespace Travelling_Salesman_Problem
{
	class TravellingSalesmanProblem
	{
		public Map map;
		string[] text;
		public Solution solution;





		public TravellingSalesmanProblem() { }

		public class Solution
		{
			public enum SolvingMethods
			{
				NearestNeightbor_Solver,
				BruteForce_Solver
			}


			public string[] path;
			public int weight;

			public Solution(string[] path, int weight)
			{
				this.path = path;
				this.weight = weight;
			}
			public void Print()
			{
				// capitalize first letter
				for (int i = 0; i < path.Length; i++)
				{
					path[i] = path[i].ToLower();
					path[i] = char.ToUpper(path[i][0]) + path[i].Substring(1);
				}

				Console.Write(
					"\n" +
					"\n" +
					"\n" +
					"\n" +
					"Best route found:" +
					"\n" +
					"\n" +
					"└──>  ");
				for (int i = 0; i < path.Length - 1; i++)
				{
					Console.Write(path[i] + " -> ");
				}
				Console.WriteLine(path[path.Length - 1]);
				Console.WriteLine("Weight: " + weight);
			}
		}


		public Solution Solve(Solution.SolvingMethods method)
		{
			Solution solution = null;

			switch (method)
			{
				case Solution.SolvingMethods.NearestNeightbor_Solver:
					solution = NearestNeightbor_Solver(map);
					break;

				case Solution.SolvingMethods.BruteForce_Solver:
					solution = BruteForce_Solver(map);
					break;
			}

			if (solution != null)
			{
				solution.weight = map.routeWeight(map.nodes, solution.path);
			}
			return solution;
		}


		public void setMap(string path)
		{
			text = CSVParser.readMapGraph(path);

			string[] nodesLine = CSVParser.parseLine(text[0]);
			string[] nodes = nodesLine.Skip(1).ToArray();

			// 
			int[][] weightMatrix = new int[text.Length - 1][];
			for (int i = 0; i < text.Length - 1; i++)
			{
				string[] row = CSVParser.parseLine(text[i + 1]);
				int[] matrixRow = new int[row.Length - 1];
				for (int j = 0; j < nodes.Length; j++)
				{
					try
					{
						matrixRow[j] = Int32.Parse(row[j + 1]);
					}
					catch (FormatException)
					{
						Console.WriteLine($"Format error on CSV, line: {i}");
						return;
					}

				}
				weightMatrix[i] = matrixRow;
			}
			map = new Map(nodes, weightMatrix);
		}
		public bool setMainNode(string nodeName)
		{
			nodeName = nodeName.ToLower();
			if (map == null ||
				map.nodes == null ||
				map.nodes.Count == 0 ||
				map.nodes.ContainsKey(nodeName) == false)
			{
				return false;
			}

			map.mainNode = map.nodes[nodeName];

			return true;
		}


		/*
		 *  method based on nearest neighbord
		 */
		public Solution NearestNeightbor_Solver(Map map)
		{
			if (map == null || map.nodes == null || map.nodes.Count == 0 || map.mainNode == null)
			{
				Console.WriteLine("Error. Be sure to configure parameters before solving.");
				return null;
			}

			// Inicializar variables
			List<string> visitedNodes = new List<string>(); // Para rastrear los nodos visitados
			Node currentNode = map.mainNode; // Comenzamos desde el nodo principal
			int totalWeight = 0;

			// Añadir el nodo principal a los visitados
			visitedNodes.Add(currentNode.name);

			// Mientras queden nodos sin visitar
			while (visitedNodes.Count < map.nodes.Count)
			{
				Vertex nearestVertex = null;
				int minWeight = int.MaxValue;

				// Buscar el nodo vecino no visitado más cercano
				foreach (var vertexEntry in currentNode.vertices)
				{
					Vertex vertex = vertexEntry.Value;
					if (!visitedNodes.Contains(vertex.node2.name) && vertex.weight < minWeight && vertex.weight > 0)
					{
						nearestVertex = vertex;
						minWeight = vertex.weight;
					}
				}

				// Si encontramos un nodo cercano, lo visitamos
				if (nearestVertex != null)
				{
					currentNode = nearestVertex.node2;
					visitedNodes.Add(currentNode.name);
					totalWeight += nearestVertex.weight;
				}
				else
				{
					// No hay más nodos que visitar desde aquí
					break;
				}
			}

			// Volver al nodo inicial para cerrar el ciclo
			if (visitedNodes.Count == map.nodes.Count)
			{
				if (currentNode.vertices.ContainsKey(map.mainNode.name))
				{
					totalWeight += currentNode.vertices[map.mainNode.name].weight;
					visitedNodes.Add(map.mainNode.name); // Añadir nodo inicial al final
				}
			}

			// Guardar y mostrar la solución
			solution = new Solution(visitedNodes.ToArray(), totalWeight);
			return solution;
		}








		/////////////////////////////////////////////////////////////////////////////////////////////////

		/*
		 *  brute force aproach
		 */
		public Solution BruteForce_Solver(Map map)
		{
			CombinationTree tree = new(map);
			CombinationTree.Bubble endBubble = null;

			do
			{
				tree.NextIteration();
				endBubble = tree.checkEnd();
			} while (endBubble != null);

			// endBubble is the first bubble with no nodes left
			// to visit and is currently at main node
			//
			// now i backtrack it to the first bubble to see the path to follow

			LinkedList<Node> path = new();



			return null;
		}

		private class CombinationTree
		{
			Map map;
			LinkedList<LinkedList<Bubble>> columns = new();

			public CombinationTree(Map map)
			{
				this.map = map;

				// add main node
				LinkedList<Bubble> firstColumn = new LinkedList<Bubble>();	// create first column
				columns.AddFirst(firstColumn);
				Bubble mainBubble = new();									// create main node's bubble
				mainBubble.nodesLeftToVisit = new();						// create its nodesLeftToVisit
				IEnumerator<String> ite = map.nodes.Keys.GetEnumerator();	//
				
				while (ite.MoveNext())
				{
					mainBubble.nodesLeftToVisit.Add(ite.Current);
				}

				firstColumn.AddFirst(mainBubble);
			}

			public void NextIteration()
			{
				LinkedList<Bubble> lastColumn = columns.Last.Value;
				columns.AddLast(new LinkedList<Bubble>());

				foreach (Bubble bubble in lastColumn)
				{
					foreach (Vertex vertice in map.nodes[bubble.node.name].vertices.Values)
					{
						Node node2 = vertice.node2;

						// "bubble2" is the conected bubble to "bubble" from next iteration
						Bubble bubble2 = new();     // create bubble2
						bubble2.node = node2;       // insert its node
						bubble2.nodesLeftToVisit = bubble.DuplicateNodesLeftToVisit();
						bubble2.nodesLeftToVisit.Remove(bubble2.node.name);         // set nodes left to visit and remove itself
						bubble2.previousBubble = bubble;
						bubble.nextBubbles.Add(bubble.nextBubbles.Count, bubble2);  // conect bubble and bubble2

						columns.Last.Value.AddLast(bubble2);
					}
				}
			}

			public class Bubble
			{
				public Node node;
				public HashSet<string> nodesLeftToVisit;
				public Dictionary<int, Bubble> nextBubbles;
				public Bubble previousBubble;

				public HashSet<string> DuplicateNodesLeftToVisit()
				{
					return new HashSet<string>(nodesLeftToVisit);
				}
			}

			public Bubble checkEnd()
			{
				Node mainNode = columns.First.Value.First.Value.node; // first (and only) node of first column (iteration)
				LinkedList<Bubble> lastColumn = columns.Last.Value;
				foreach (Bubble bubble in lastColumn)
				{
					if (bubble.nodesLeftToVisit.Count == 0 &&
						bubble.node.name == mainNode.name)
					{
						return bubble;
					}
				}
				return null;
			}
		}
	}
}

