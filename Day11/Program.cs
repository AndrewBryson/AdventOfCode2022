using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Day11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const long maxRound = 10000;

            // Initialise the game
            Round round = new Round();

            // Read all input
            //string[] input = File.ReadAllLines(@"testdata1.txt"); // test data
            string[] input = File.ReadAllLines(@"day11-input.txt");
            int offset = 0;
            while (true)
            {
                string[] block = input.Skip(offset).Take(7).ToArray();

                if (block.Length == 0)
                    break;

                // Parse inputs to objects
                long player = long.Parse(Regex.Matches(block[0], @"\d+")[0].Value);

                List<long> startingItems = new List<long>();
                foreach (Match m in Regex.Matches(block[1], @"\d+"))
                {
                    startingItems.Add(long.Parse(m.Value));
                }

                string[] ops = block[2].Split("=")[1].Trim().Split(" ");

                int divisibleBy = int.Parse(Regex.Matches(block[3], @"\d+")[0].Value);

                int throwToIfTrue = int.Parse(Regex.Matches(block[4], @"\d+")[0].Value);
                int throwToIfFalse = int.Parse(Regex.Matches(block[5], @"\d+")[0].Value);

                Console.WriteLine("p:{0} o{1} i:{2} d:{3} t:{4} f:{5}",
                    player,
                    string.Join("|", startingItems),
                    string.Join("|", ops),
                    divisibleBy,
                    throwToIfTrue,
                    throwToIfFalse);

                Turn turn = new Turn();
                turn.Operations = ops.ToList();
                turn.DivisibleBy = divisibleBy;
                turn.throwToIfTrue = throwToIfTrue;
                turn.throwToIfFalse = throwToIfFalse;

                Player newPlayer = new Player(player.ToString());
                newPlayer.CurrentItems = startingItems;
                newPlayer.NextTurn = turn;

                round.Players.Add(newPlayer);

                offset += 7;
            }
            Console.WriteLine();

            // Part 2:
            long mod = 1;
            foreach (var item in round.Players)
            {
                mod *= item.NextTurn.DivisibleBy;
            }

            // Play the game
            while (round.RoundsPlayed < maxRound)
            {
                foreach (var player in round.Players)
                {
                    var copy = new List<long>(player.CurrentItems);
                    foreach (var item in copy)
                    {
                        // Inspection: Do the calculation
                        long newlevel = player.Inspect(item);

                        // Part 1:
                        //newlevel = newlevel / 3;
                        // Part 2: keep the worry level (newlevel) to a small number instead of growing massively
                        newlevel %= mod;

                        // Test the result
                        if (OpCalc.IsDivisibleBy(newlevel, player.NextTurn.DivisibleBy))
                        {
                            // True condition: remove and pass to the other player
                            round.Players[player.NextTurn.throwToIfTrue].CurrentItems.Add(newlevel);
                        }
                        else
                        {
                            // False condition
                            round.Players[player.NextTurn.throwToIfFalse].CurrentItems.Add(newlevel);
                        }

                        player.CurrentItems.Remove(item);
                    }
                }

                Console.WriteLine("End of round: {0}", ++round.RoundsPlayed);
            }

            Console.WriteLine("FINAL data:");
            ShowPlayerItems(round);

            var top2 = round.Players.Select(x => x.InspectionCount).OrderDescending().Take(2).ToList();
            Console.WriteLine("Top 2: {0}", string.Join(", ", top2));
            long monkeyBusiness = top2[0] * top2[1];

            Console.WriteLine("Monkey Business = {0}", monkeyBusiness);
            Console.WriteLine("Done");
        }

        private static void ShowPlayerItems(Round round)
        {
            foreach (var player in round.Players)
            {
                Console.WriteLine("player:{0}, count: {1}, items: {2}", player.Name, player.InspectionCount, string.Join(",", player.CurrentItems));
            }
        }
    }

    class OpCalc
    {
        public static bool IsDivisibleBy(long firstNumber, long secondNumber)
        {
            return firstNumber % secondNumber == 0;
        }
        public static long Calculate(long firstNumber, string opcode, long operand)
        {
            switch (opcode)
            {
                case "*":
                    return firstNumber * operand;
                case "+":
                    return firstNumber + operand;
                case "%":
                    return firstNumber % operand;
                default:
                    throw new Exception("Shouldn't happen");
            }
        }
    }

    class Round
    {
        public List<Player> Players { get; set; }
        public long RoundsPlayed { get; set; }
        public Round()
        {
            this.Players = new List<Player>();
        }
    }

    class Turn
    {
        public List<string> Operations { get; set; }
        public int DivisibleBy { get; set; }
        public int throwToIfTrue { get; set; }
        public int throwToIfFalse { get; set; }
        public Turn()
        {
            this.Operations = new List<string>();
        }
    }

    class Player
    {
        public string Name { get; set; }
        public List<long> CurrentItems { get; set; }
        public long InspectionCount { get; set; }
        public Turn NextTurn { get; set; }
        public Player(string name)
        {
            this.Name = name;
            this.CurrentItems = new List<long>();
        }

        internal long Inspect(long item)
        {
            var ops = this.NextTurn.Operations;
            long operand = ops[2] == "old" ? item : long.Parse(ops[2]);
            long newlevel = OpCalc.Calculate(
                item,
                ops[1],
                operand);

            this.InspectionCount++;

            return newlevel;
        }
    }
}