using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Day3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int sum = 0;
            string[] lines = File.ReadAllLines(@"input/day3-input.txt");

            //sum = Part1(lines);

            sum = Part2(lines);
            Console.WriteLine("Input length: {0}", lines.Length);
            Console.WriteLine("sum = {0}", sum);
        }

        private static int Part2(string[] lines)
        {
            int sum = 0;
            List<char> part1 = new List<char>();
            List<char> part2 = new List<char>();
            List<char> part3 = new List<char>();

            for (int i = 0; i < lines.Length; i = i + 3)
            {
                Console.WriteLine("i: {0}", i);
                part1 = lines[i].ToList<char>();
                part2 = lines[i+1].ToList<char>();
                part3 = lines[i+2].ToList<char>();

                var commonChar = from p1 in part1
                                 join p2 in part2 on p1 equals p2
                                 join p3 in part3 on p1 equals p3
                                 select p1;

                commonChar = commonChar.Distinct().ToList<char>();

                int commonCount = commonChar.Count();
                Console.WriteLine("Common #: {0}", commonCount);
                sum += GetCharScore(commonChar.First());
            }

            return sum;
        }

        private static int Part1(string[] lines)
        {
            int sum = 0;
            foreach (string line in lines)
            {
                int length = line.Length;
                int half = length / 2;

                List<char> left = line.Substring(0, half).ToList<char>();
                List<char> right = line.Substring(half).ToList<char>();

                var common = from l in left
                             join r in right on l equals r
                             select l;

                Console.WriteLine(string.Join(", ", common));

                foreach (char item in common.Distinct())
                {
                    sum += GetCharScore(item);
                }
            }

            Console.WriteLine("Sum: {0}", sum);
            return sum;
        }

        private static int GetCharScore(char item)
        {
            int itemScore = -1;

            int lowerCaseOffset = -96;
            int upperCaseOffset = -38;

            itemScore = (int)item + (Char.IsLower(item) ? lowerCaseOffset : upperCaseOffset);

            return itemScore;
        }
    }
}
