using System;
using System.IO;

namespace Day2
{

    /*
     * The second column, you reason, must be what you should play in response: 
     * X for Rock, 
     * Y for Paper, and 
     * Z for Scissors. Winning every time would be suspicious, so the responses must have been carefully chosen.
     * The winner of the whole tournament is the player with the highest score. 
     * Your total score is the sum of your scores for each round. 
     * The score for a single round is the score for the shape you selected 
     * (1 for Rock, 2 for Paper, and 3 for Scissors) 
     * plus the score for the outcome of the round (0 if you lost, 3 if the round was a draw, and 6 if you won).
     */
    internal class Program
    {
        //public enum RequiredGameOutcome
        //{
        //    Win,
        //    Lose,
        //    Draw
        //}

        public enum Options
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }

        public enum Result
        {
            Win = 6,
            Lose = 0,
            Draw = 3
        }

        private class Game
        {
            public Options Draw { get; set; }
            public Game(string input)
            {
                switch (input)
                {
                    case "A":
                    case "X":
                        this.Draw = Options.Rock;
                        break;
                    case "B":
                    case "Y":
                        this.Draw = Options.Paper;
                        break;
                    case "C":
                    case "Z":
                        this.Draw = Options.Scissors;
                        break;
                    default:
                        throw new Exception(input);
                }
            }
        }
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"input/input.txt");

            int myScore = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                string them = parts[0];
                string me = parts[1];

                Game theirGame = new Game(them);

                // Should I win, lose or draw?
                Result requiredOutcome = GetOutcome(me);
                string requiredDraw = GetRequiredDraw(theirGame.Draw, requiredOutcome);
                Game myGame = new Game(requiredDraw);
                //Console.WriteLine("them: {0}, required: {1}, draw: {2}", theirGame.Draw, outcome, myGame.Draw);

                //myScore += GetDrawScore(myGame.Draw);

                //var result = GetWinner(theirGame.Draw, myGame.Draw);

                myScore += GetDrawScore(myGame.Draw);
                myScore += GetResultScore(requiredOutcome);
            }

            Console.WriteLine("myScore: {0}", myScore);
        }

        private static string GetRequiredDraw(Options theirDraw, Result outcome)
        {
            if (theirDraw == Options.Rock)
            {
                switch (outcome)
                {
                    case Result.Win:
                        return "B";
                    case Result.Lose:
                        return "C";
                    case Result.Draw:
                        return "A";
                }
            }

            if (theirDraw == Options.Paper)
            {
                switch (outcome)
                {
                    case Result.Win:
                        return "C";
                    case Result.Lose:
                        return "A";
                    case Result.Draw:
                        return "B";
                }
            }

            if (theirDraw == Options.Scissors)
            {
                switch (outcome)
                {
                    case Result.Win:
                        return "A";
                    case Result.Lose:
                        return "B";
                    case Result.Draw:
                        return "C";
                }
            }

            return null;
        }

        private static Result GetOutcome(string me)
        {
            if (me == "X")
            {
                return Result.Lose;
            }
            else if (me == "Y")
            {
                return Result.Draw;
            }
            else
            {
                return Result.Win;
            }
        }

        private static int GetResultScore(Result result)
        {
            int score = (int)result;
            return score;
        }

        private static int GetDrawScore(Options draw)
        {
            int score = (int)draw;
            //Console.WriteLine("{0} -> {1}", draw.ToString(), score);

            return score;
        }

        public static Result GetWinner(Options them, Options me)
        {
            // Draw conditions
            if (them == me)
            {
                return Result.Draw;
            }

            // They-win conditions
            if (them == Options.Rock && me == Options.Scissors)
            {
                return Result.Lose;
            }    

            if (them == Options.Paper && me == Options.Rock)
            {
                return Result.Lose;
            }

            if (them == Options.Scissors && me == Options.Paper)
            {
                return Result.Lose;
            }

            // Otherwise I win
            return Result.Win;
        }

    }
}
