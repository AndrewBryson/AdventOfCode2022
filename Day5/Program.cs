using System.Diagnostics;

internal class Program
{
    /*
     *                 [M]     [W] [M]    
                    [L] [Q] [S] [C] [R]    
                    [Q] [F] [F] [T] [N] [S]
            [N]     [V] [V] [H] [L] [J] [D]
            [D] [D] [W] [P] [G] [R] [D] [F]
        [T] [T] [M] [G] [G] [Q] [N] [W] [L]
        [Z] [H] [F] [J] [D] [Z] [S] [H] [Q]
        [B] [V] [B] [T] [W] [V] [Z] [Z] [M]
            1   2   3   4   5   6   7   8   9 

        move 1 from 7 to 4
        move 1 from 6 to 2
     */
    private static void Main(string[] args)
    {
        string[] lines = File.ReadAllLines(@"day5-input.txt");

        DoWork(lines);
    }

    private static void DoWork(string[] lines)
    {
        // Find the line breaks
        int lineBreak = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                lineBreak = (i - 1);
                break;
            }
        }

        Debug.Assert(lineBreak > 0);

        int maxStackNumber = GetStackCount(lines, lineBreak);

        // Read cargo arrangement back to front
        List<List<string>> cargoStacks = CreateStackLists(maxStackNumber);
        for (int i = (lineBreak - 1); i >= 0; i--)
        {
            string line = lines[i];
            // Parse manifest to each cargo stack
            char[] lineData = line.ToCharArray();
            AssignCargo(cargoStacks, lineData);
        }

        ApplyMovements(cargoStacks, lines.Skip(lineBreak+2).ToList());

        Console.WriteLine("Part 2 answer = ");
        foreach (var item in cargoStacks)
        {
            Console.Write("{0}", item.TakeLast(1).First());
        }
    }

    private static void ApplyMovements(List<List<string>> cargoLists, IEnumerable<string> enumerable)
    {
        foreach (string instruction in enumerable)
        {
            string[] parts = instruction.Split(' ');

            int moveCount = int.Parse(parts[1]);
            int moveFrom = int.Parse(parts[3]);
            int moveTo = int.Parse(parts[5]);

            // The source list
            var popList = cargoLists[moveFrom - 1];

            // Remove items from the head of the list, making a copy first
            var movers = popList.GetRange((popList.Count - moveCount), moveCount);
            popList.RemoveRange((popList.Count - moveCount), moveCount);

            // Add the moving items to the target list
            var pushList = cargoLists[moveTo - 1];
            pushList.AddRange(movers);
        }

        return;
    }

    private static void AssignCargo(List<List<string>> cargoLists, char[] lineData)
    {

        // Jump this number of characters to get each manifest item
        int jumpOffset = 4;
        // Cargo starts here
        int index = 1;

        while (index < lineData.Length)
        {

            char c = lineData[index];

            if (char.IsAsciiLetterUpper(c))
            {
                // Round down to the whole integer
                int listIndex = index / jumpOffset;
                cargoLists[listIndex].Add(c.ToString());
            }

            index += jumpOffset;
        }
    }

    private static int GetStackCount(string[] lines, int lineBreak)
    {
        char[] numbers = lines[lineBreak].ToCharArray();
        for (int i = numbers.Length - 1; i >= 0; i--)
        {
            string c = numbers[i].ToString();
            if (!string.IsNullOrWhiteSpace(c))
            {
                return int.Parse(c);
            }
        }

        return -1;
    }

    private static List<List<string>> CreateStackLists(int maxStackNumber)
    {
        List<List<string>> stacks = new List<List<string>>();
        for (int i = 0; i < maxStackNumber; i++)
        {
            stacks.Add(new List<string>());
        }

        return stacks;
    }
}