using System.Runtime.CompilerServices;

namespace Day10
{
    internal class Program
    {
        static int register = 1;
        static void Main(string[] args)
        {
            //string input = @"noop
            //                addx 3
            //                addx -5";

            //string[] instructions = input.Split(Environment.NewLine).Select(x => x.Trim()).ToArray();
            //string[] instructions = File.ReadAllLines(@"testdata.txt");
            string[] instructions = File.ReadAllLines(@"day10-input.txt");

            bool inProgress = false;
            string instruction = string.Empty;
            int ticks = 1;
            int sum = 0;
            const int DELAY_ADDX = 2;

            List<int> keyCycles = new List<int>() { 20, 60, 100, 140, 180, 220 };
            Dictionary<int, Action<int>> callbackActions = new Dictionary<int, Action<int>>();
            Queue<string> queue = new Queue<string>();
            foreach (var item in instructions)
            {
                queue.Enqueue(item);
            }

            // While there are instructions to process
            while (true)
            {
                // Add to the sum when we hit key cycle markers
                if (keyCycles.Contains(ticks))
                {
                    sum += (ticks * register);
                }

                if (callbackActions.Count == 0)
                {
                    if (queue.Count > 0)
                    {
                        instruction = queue.Dequeue();
                    }
                    else
                    {
                        break;
                    }

                    switch (instruction)
                    {
                        case "noop":
                            break;
                        case string s when s.StartsWith("addx"):
                            int val = int.Parse(instruction.Split(' ')[1]);
                            int when = ticks + DELAY_ADDX - 1; // -1 because instruction exec at end of cycle

                            callbackActions.Add(when, (x) =>
                            {
                                register += val;
                            });

                            break;
                    }
                }

                // Wait until the current instruction has processed
                if (callbackActions.Count > 0)
                {
                    // Execute any callback due during this tick, e.g. a noop starting and completing in 1 tick
                    if (callbackActions.ContainsKey(ticks))
                    {
                        var callback = callbackActions[ticks];
                        callback(ticks);
                        callbackActions.Remove(ticks);
                    }
                }


                int mod = ticks % 40;
                if (mod >= (register -1) && mod <= (register +1))
                {
                    PrintCRT(ticks, "#");
                }
                else
                {
                    PrintCRT(ticks, ".");
                }

                ticks++;
            }

            Console.WriteLine();
            Console.WriteLine("Sum: {0}", sum);
        }

        private static void PrintCRT(int tick, string s)
        {
            if (tick % 40 == 0)
            {
                Console.WriteLine();
            }

            Console.Write(s);
        }
    }
}