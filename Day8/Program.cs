using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Transactions;

internal class Program
{
    class Tree
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
    }

    private static void Main(string[] args)
    {
        #region TestData
        //string input = @"30373
        //                25512
        //                65332
        //                33549
        //                35390";
        //string[] parts = input.Split(Environment.NewLine);
        //int sideLength = parts[0].Length;
        //int[,] trees = new int[5, 5];
        //Tree[,] treeData = new Tree[5, 5];
        #endregion

        string[] parts = File.ReadAllLines(@"day8-input.txt");
        int sideLength = parts[0].Length;
        Tree[,] treeData = new Tree[sideLength, sideLength];

        // Build a 2D array of the tree structure
        for (int i = 0; i < parts.Length; i++)
        {
            string line = parts[i].Trim();
            for (int c = 0; c < line.Length ; c++)
            {
                int number = int.Parse(line[c].ToString());
                //trees[i, c] = number;

                Tree tree = new Tree() { Height = number, X = i, Y = c };
                treeData[i, c] = tree;
            }
        }

        Console.WriteLine("First element = {0}, Last element = {1}", treeData[0, 0].Height, treeData[sideLength-1, sideLength-1].Height);
        
        List<Tree> treeResults = new List<Tree>();

        // Sides
        Tree[] treeSlice;
        for (int x = 0; x < sideLength; x++)
        {
            // Left to right
            treeSlice = GetSliceTreeData(treeData, new List<int>() { x }, Enumerable.Range(0, sideLength).ToList());
            CalculateTreeVisibility(treeSlice, treeResults);
            //Console.WriteLine("x: {0}, result l-r: {1}", x, treeResults.Count);

            // Now right to left on the same slice
            treeSlice = GetSliceTreeData(treeData, new List<int>() { x }, Enumerable.Range(0, sideLength).OrderDescending().ToList());
            CalculateTreeVisibility(treeSlice, treeResults);
            //Console.WriteLine("x: {0}, result r-l: {1}", x, treeResults.Count);
        }

        // Top and bottom
        for (int y = 0; y < sideLength; y++)
        {
            // Top to bottom
            treeSlice = GetSliceTreeData(treeData, Enumerable.Range(0, sideLength).ToList(), new List<int>() { y });
            CalculateTreeVisibility(treeSlice, treeResults);
            //Console.WriteLine("y: {0}, result top-bottom: {1}", y, treeResults.Count);

            // Now bottom to top on the same slice
            treeSlice = GetSliceTreeData(treeData, Enumerable.Range(0, sideLength).OrderDescending().ToList(), new List<int>() { y });
            CalculateTreeVisibility(treeSlice, treeResults);
            //Console.WriteLine("y: {0}, result bottom-top: {1}", y, treeResults.Count);
        }

        var distinct = treeResults.Distinct();
        Console.WriteLine("With Distinct() = {0}", distinct.Count());

        // Part 2
        Console.WriteLine();
        // Iterate the whole forest, calculate a viewing distance for each tree, higest wins
        Dictionary<Tree, int> distanceResults = new Dictionary<Tree, int>();
        for (int x = 0; x < sideLength; x++)
        {
            for (int y = 0; y < sideLength; y++)
            {
                Tree tree = treeData[x, y];
                Console.WriteLine("{0}: X:{1}, Y:{2}, height: {3}", distanceResults.Count, tree.X, tree.Y, tree.Height);
                int result = CalculateTreeDistanceView(tree, treeData);
                distanceResults.Add(tree, result);
            }
        }

        // Sort to find the best tree
        var sorted = distanceResults.Select(x => x.Value).OrderDescending().ToList();
        var first = sorted.First();
        Console.WriteLine(first);
    }

    private static int CalculateTreeDistanceView(Tree point, Tree[,] forest)
    {
        int top = 0, down = 0, left = 0, right = 0;
        int sideLength = forest.GetLength(0);
        int distanceToEdge = 0;
        int soFar = 0;

        // Look Up: forest[?-, y]
        distanceToEdge = point.X != 0 ? (point.X - 1) : 0;
        for (int i = distanceToEdge; i >= 0; i--)
        {
            Tree next = forest[i, point.Y];
            if (point == next)
            {
                continue;
            }

            if (next.Height >= point.Height)
            {
                top++;
                break;
            }

            top++;
        }

        // Look Down: forest[?+, y]
        distanceToEdge = point.X != (sideLength-1) ? (point.X + 1) : (sideLength-1);
        for (int i = distanceToEdge; i < sideLength ; i++)
        {
            Tree next = forest[i, point.Y];
            if (point == next)
            {
                continue;
            }

            if (next.Height >= point.Height)
            {
                down++;
                break;
            }

            down++;
        }

        // Look Left: forest[x, y-]
        distanceToEdge = point.Y != 0 ? (point.Y - 1) : 0;
        for (int i = distanceToEdge; i >= 0; i--)
        {
            Tree next = forest[point.X, i];
            if (point == next)
            {
                continue;
            }

            if (next.Height >= point.Height)
            {
                left++;
                break;
            }

            left++;
        }

        // Look right: forest[x, y+]
        distanceToEdge = point.Y != (sideLength-1) ? (point.Y + 1) : (sideLength-1);
        for (int i = distanceToEdge; i < sideLength; i++)
        {
            Tree next = forest[point.X, i];
            if (point == next)
            {
                continue;
            }

            if (next.Height >= point.Height)
            {
                right++;
                break;
            }

            right++;
        }

        int result = top * down * left * right;
        return result;
    }

    private static Tree[] GetSliceTreeData(Tree[,] trees, List<int> x, List<int> y)
    {
        List<Tree> slice = new List<Tree>();

        foreach (var i in x)
        {
            foreach (var item in y)
            {
                slice.Add(trees[i, item]);
            }
        }

        return slice.ToArray();
    }

    private static void CalculateTreeVisibility(Tree[] slice, List<Tree> results)
    {
        // Add the outer/edge/first tree
        results.Add(slice.First());

        Tree second;
        Tree highestSoFar = slice.First();

        // Doesn't count the edge/side-most element
        // And don't count the tree on the far edge
        for (int i = 1; i < slice.Length; i++)
        {
            second = slice[i];
            if (second.Height > highestSoFar.Height)
            {
                highestSoFar = second;
                results.Add(second);
            }
        }
    }
}