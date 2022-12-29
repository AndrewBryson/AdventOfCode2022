using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace Day1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = File.ReadAllLines(@"inputs/day1-a.txt").ToList<string>();
            List<int> calories = new List<int>();

            int calorieCount = 0;
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    calorieCount += int.Parse(line);
                }
                else
                {
                    calories.Add(calorieCount);
                    calorieCount = 0;
                }
            }


            var sorted = calories.OrderByDescending(x => x).ToList();

            //1.a
            Console.WriteLine(sorted.First());

            //1.b
            Console.WriteLine(sorted.Take(3).Sum());
        }
    }
}
