internal class Program
{
    private static void Main(string[] args)
    {
        int overlapCount = 0;
        string[] lines = File.ReadAllLines(@"input/day4-input.txt");

        /*
         *  30-31,2-31
            6-92,4-5
            35-39,42-44
         */
        foreach (string line in lines)
        {
            string[] sectionParts = line.Split(',');
            string[] part1 = sectionParts[0].Split('-');
            string[] part2 = sectionParts[1].Split('-');

            // Figure out the size of the number range
            int part1Min = int.Parse(part1[0]);
            int part1Max = int.Parse(part1[1]);
            int part1Range = part1Max - part1Min + 1;

            int part2Min = int.Parse(part2[0]);
            int part2Max = int.Parse(part2[1]);
            int part2Range = part2Max- part2Min + 1;

            // Expand range to a list of ints
            List<int> part1List = Enumerable.Range(part1Min, part1Range).ToList();
            List<int> part2List = Enumerable.Range(part2Min, part2Range).ToList();

            // Find complete overlap
            //if (part1List.TrueForAll(x => part2List.Contains(x)) || part2List.TrueForAll(x => part1List.Contains(x)))
            //{
            //    Console.WriteLine(" + {0}", line);
            //    overlapCount++;
            //}

            // Find any overlap
            if (part1List.Any(x => part2List.Contains(x)) || part2List.Any(x => part1List.Contains(x)))
            {
                Console.WriteLine(" + {0}", line);
                overlapCount++;
            }
        }

        Console.WriteLine("Overlap count: {0}", overlapCount);
    }
}