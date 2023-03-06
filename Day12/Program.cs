using System.Diagnostics;

namespace Day12
{
    internal class Program
    {
        static void HelperPrintRawGraph(int[,] graph)
        {
            int sideLength = graph.GetLength(0);
            for (int x = 0; x < sideLength; x++)
            {
                for (int y = 0; y < sideLength; y++)
                {
                    System.Console.Write("{0}\t", graph[x,y]);
                }
                System.Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            // string input = @"Sabqponm
            //                  abcryxxl
            //                  accszExk
            //                  acctuvwj
            //                  abdefghi";

            string input = File.ReadAllText(@"day12-input.txt");

            string[] split = input.Split(Environment.NewLine);
            int sideLength = split[0].Length;
            int depth = split.Length;
            int vertexCount = sideLength * depth;

            List<Node> nodes = new List<Node>();

            // Build an array of nodes with coordinates
            int count = 0;
            for (int y = 0; y < split.Length; y++)
            {
                string line = split[y].Trim();

                for (int x = 0; x < line.Length; x++)
                {
                    char c = line[x];
                    var n = new Node(c) 
                    { 
                        X = x, Y = y, Sequence = count++ 
                    };

                    nodes.Add(n);
                }
            }

            // Add all the neighbours of each node using coordinates
            for (int y = 0; y < split.Length; y++)
            {
                string line = split[y].Trim();
                for (int x = 0; x < line.Length; x++)
                {
                    Node currentNode = nodes.Where(n => n.X == x && n.Y == y).First();

                    // PART1 logic for all IF below: (n.Elevation - currentNode.Elevation <= 1)
                    // PART2 logic for all IF below: (n.Elevation - currentNode.Elevation >= -1)

                    // Left
                    if (x > 0)
                    {
                        currentNode.Neighbours.Add(nodes.Where(n => n.X == x - 1 && n.Y == y && (n.Elevation - currentNode.Elevation >= -1)).FirstOrDefault());
                    }

                    // Right
                    if (x < sideLength - 1)
                    {
                        currentNode.Neighbours.Add(nodes.Where(n => n.X == x + 1 && n.Y == y && (n.Elevation - currentNode.Elevation >= -1)).FirstOrDefault());
                    }

                    // Up
                    if (y > 0)
                    {
                        currentNode.Neighbours.Add(nodes.Where(n => n.X == x && n.Y == y - 1 && (n.Elevation - currentNode.Elevation >= -1)).FirstOrDefault());
                    }

                    // Down
                    if (y < depth - 1)
                    {
                        currentNode.Neighbours.Add(nodes.Where(n => n.X == x && n.Y == y + 1 && (n.Elevation - currentNode.Elevation >= -1)).FirstOrDefault());
                    }
                }
            }

            int[,] expanded = ExpandToArray(nodes);
            // HelperPrintRawGraph(expanded);

            // Part 1
            // var endNode = nodes.Where(x => x.IsEnd == true).First();
            // var startNode = nodes.Where(x => x.IsStart == true).First();
            // int[] results = DijkstraImpl.Dijkstra(expanded, startNode.Sequence, vertexCount);
            // System.Console.WriteLine("START is: {0}", startNode.Sequence);
            // System.Console.WriteLine("END is: {0}", endNode.Sequence);
            // System.Console.WriteLine("Value at {0} is {1}", endNode.Sequence, results[endNode.Sequence]);

            // Part 2
            var startNode = nodes.Where(x => x.IsEnd == true).First();
            int[] results = DijkstraImpl.Dijkstra(expanded, startNode.Sequence, vertexCount);
            // All 'a' nodes
            var aNodeSequences = nodes.Where(x => x.Elevation == 1).Select(x => x.Sequence).ToList();
            List<int> distances = new List<int>();
            // Get the distance values to all 'a' nodes and report the shortest.
            for (int i = 0; i < results.Length; i++)
            {
                if (aNodeSequences.Contains(i)) 
                {
                    distances.Add(results[i]);
                }
            }

            System.Console.WriteLine("Shortest path: {0}", distances.Order().ToList()[0]);
        }

        static int[,] ExpandToArray(List<Node> nodes)
        {
            System.Console.WriteLine("Count to expand: {0}", nodes.Count);
            int[,] graph = new int[nodes.Count, nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                var thisNode = nodes[i];
                for (int j = 0; j < thisNode.Neighbours.Count; j++)
                {
                    if (thisNode.Neighbours[j] == null)
                    {
                        continue;
                    }

                    var neighbour = thisNode.Neighbours[j];
                    graph[thisNode.Sequence, neighbour.Sequence] = 1;
                }
            }

            return graph;
        }
    }

    class Node
    {
        public bool IsStart { get; }
        public bool IsEnd { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Sequence { get; set; }
        public List<Node> Neighbours { get; set; }
        public char Character { get; set; }
        public int Elevation { get; set; }
        public Node(char character)
        {
            this.Character = character;
            this.Neighbours = new List<Node>();

            if (character == 'S')
            {
                this.IsStart = true;
                this.Elevation = this.GetElevation('a');
                return;
            }

            if (character == 'E')
            {
                this.IsEnd = true;
                this.Elevation = this.GetElevation('z');
                return;
            }

            this.Elevation = this.GetElevation(character);
        }

        private int GetElevation(char c)
        {
            const int lowerCaseOffset = -96;
            return (int)c + lowerCaseOffset;
        }
    }
}