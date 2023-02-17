using System.Diagnostics;

namespace Day12
{
    internal class Program
    {
        static int[,] graph = new int[,] {
        { 0, 4, 0, 0, 0, 0, 0, 8, 0 },
        { 4, 0, 8, 0, 0, 0, 0, 11, 0 },
        { 0, 8, 0, 7, 0, 4, 0, 0, 2 },
        { 0, 0, 7, 0, 9, 14, 0, 0, 0 },
        { 0, 0, 0, 9, 0, 10, 0, 0, 0 },
        { 0, 0, 4, 14, 10, 0, 2, 0, 0 },
        { 0, 0, 0, 0, 0, 2, 0, 1, 6 },
        { 8, 11, 0, 0, 0, 0, 1, 0, 7 },
        { 0, 0, 2, 0, 0, 0, 6, 7, 0 }
    };

        private static char END = 'E';
        static void Main(string[] args)
        {
            DijkstraImpl.Dijkstra(graph, 0, 9);
            Environment.Exit(0);

            string input = @"Sabqponm
                             abcryxxl
                             accszExk
                             acctuvwj
                             abdefghi";

            string[] split = input.Split(Environment.NewLine);
            int sideLength = split[0].Length;
            int depth = split.Length;
            char[,] map = new char[sideLength, depth];
            List<Node> nodes = new List<Node>();

            // Build an array of nodes with coordinates
            for (int y = 0; y < split.Length; y++)
            {
                string line = split[y].Trim();

                for (int x = 0; x < line.Length; x++)
                {
                    char c = line[x];
                    map[x, y] = c;
                    nodes.Add(new Node(c) { X = x, Y = y });
                }
            }

            for (int y = 0; y < split.Length; y++)
            {
                string line = split[y].Trim();
                for (int x = 0; x < line.Length; x++)
                {
                    Node currentNode = nodes.Where(n => n.X == x && n.Y == y).First();

                    // Left
                    if (x > 0)
                    {
                        currentNode.Neighbours.Add(nodes.Where(n => n.X == x - 1 && n.Y == y && (n.Elevation - currentNode.Elevation <= 1)).FirstOrDefault());
                    }

                    // Right
                    if (x < sideLength - 1)
                    {
                        currentNode.Neighbours.Add(nodes.Where(n => n.X == x + 1 && n.Y == y && (n.Elevation - currentNode.Elevation <= 1)).FirstOrDefault());
                    }

                    // Up
                    if (y > 0)
                    {
                        currentNode.Neighbours.Add(nodes.Where(n => n.X == x && n.Y == y - 1 && (n.Elevation - currentNode.Elevation <= 1)).FirstOrDefault());
                    }

                    // Down
                    if (y < depth - 1)
                    {
                        currentNode.Neighbours.Add(nodes.Where(n => n.X == x && n.Y == y + 1 && (n.Elevation - currentNode.Elevation <= 1)).FirstOrDefault());
                    }
                }
            }

            DijkstraAlgorithm(0, nodes);

            // Figure out how to walk the tree
            List<Node> visited = new List<Node>();
            FindPath(nodes, nodes[0], visited);
            Console.WriteLine("visited count: {0}", visited.Count);
        }

        static int MinDistance(bool[] visited, int[] dist)
        {
            int min = int.MaxValue, minIndex = -1;

            for (int v = 0; v < visited.Length; v++)
                if (visited[v] == false && dist[v] <= min)
                {
                    min = dist[v];
                    minIndex = v;
                }

            return minIndex;
        }

        static void DijkstraAlgorithm(int src, List<Node> nodes)
        {
            int N = nodes.Count;
            int[] dist = new int[N];
            bool[] visited = new bool[N];

            for (int i = 0; i < N; i++)
            {
                dist[i] = int.MaxValue;
                visited[i] = false;
            }

            dist[src] = 0;

            for (int count = 0; count < N - 1; count++)
            {
                int u = MinDistance(visited, dist);

                visited[u] = true;

                for (int v = 0; v < N; v++)
                    if (!visited[v] && graph[u, v] != 0 && dist[u] != int.MaxValue && dist[u] + graph[u, v] < dist[v])
                        dist[v] = dist[u] + graph[u, v];
            }
        }

        private static void FindPath(List<Node> nodes, Node current, List<Node> visited)
        {
            if (current.IsEnd)
            {
                // Reached the end.
                Console.WriteLine("Found the end!");
                Environment.Exit(0);
                //return;
            }

            foreach (var neighbour in current.Neighbours)
            {
                if (visited.Contains(neighbour)) continue; // Don't consider any previously visited Node

                if (CanMove(current, neighbour))
                {
                    Console.WriteLine("{0}:{1}[{2},{3}] -> {4}:{5}[{6},{7}]",
                        current.Character, current.Elevation, current.X, current.Y,
                        neighbour.Character, neighbour.Elevation, neighbour.X, neighbour.Y);
                    visited.Add(current);
                    FindPath(nodes, neighbour, visited);
                }
            }
        }

        private static bool CanMove(Node current, Node neighbour)
        {
            int difference = neighbour.Elevation - current.Elevation;
            return difference >= 0 && difference <= 1;
        }
    }

    class Node
    {
        public bool IsStart { get; }
        public bool IsEnd { get; }
        public int X { get; set; }
        public int Y { get; set; }
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
                this.Elevation = 0;
                return;
            }

            if (character == 'E')
            {
                this.IsEnd = true;
                this.Elevation = this.GetElevation('z') + 1;
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