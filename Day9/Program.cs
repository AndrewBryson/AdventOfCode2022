using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

namespace Day9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region TestData
            string input = @"R 4
                                U 4
                                L 3
                                D 1
                                R 4
                                D 1
                                L 5
                                R 2";
            //string[] instructions = input.Split(Environment.NewLine).Select(x => x.Trim()).ToArray();
            #endregion

            string[] instructions = File.ReadAllLines(@"day9-input.txt").Select(x => x.Trim()).ToArray();

            Point start = new Point(0, 0);
            Rope rope = new Rope(start);

            foreach (string move in instructions)
            {
                if (!string.IsNullOrWhiteSpace(move))
                {
                    ApplyRopeMove(rope, move);
                }
            }

            Console.WriteLine();
            var tailDistinct = rope.TrailTail.Distinct();
            Console.WriteLine("Tail Distinct() = {0}", tailDistinct.Count());
        }

        private static void ApplyRopeMove(Rope rope, string move)
        {
            string direction = move.Split(' ')[0];
            int count = int.Parse(move.Split(' ')[1]);

            for (int i = 0; i < count; i++)
            {
                rope.MoveOnePosition(direction);
            }

            // Debugging methods to visualise the Rope and Knots state.
            //PrintChart(rope, move);
            //PrintPostions(rope, move);
        }

        private static void PrintPostions(Rope rope, string move)
        {
            Console.WriteLine(string.Format(" = {0} ", move));
            foreach (var item in rope.Knots)
            {
                Console.WriteLine("{0}: [{1},{2}]", item.Name, item.Point.X, item.Point.Y);
            }
        }

        private static void PrintChart(Rope rope, string move)
        {
            Console.WriteLine(string.Format(" = {0} ", move));
            int x = 5; int y = 4;
            for (int j = y; j >= 0; j--)
            {
                for (int i = 0; i <= x; i++)
                {
                    Knot any = rope.Knots.Where(x => x.Point.X == i && x.Point.Y == j).MaxBy(x => x.Name);
                    Console.Write(" {0}", any == null ? "." : any.Name);
                    //Console.Write(" {0},{1} ", i, j);
                }
                Console.WriteLine();
            }
        }
    }
    class Rope
    {
        public Knot[] Knots { get; set; }
        public Knot Head { get; set; }
        public Knot Tail { get; set; }
        public List<Point> TrailTail { get; set; }
        public Rope(Point start)
        {
            this.Knots = new Knot[10];
            this.TrailTail = new List<Point>();

            // Inititialise the Rope and its Knots
            string[] names = new string[] { "H", "1", "2", "3", "4", "5", "6", "7", "8", "T" };
            for (int i = 0; i < this.Knots.Count(); i++)
            {
                this.Knots[i] = new Knot()
                {
                    Point = new Point(start.X, start.Y),
                    Name = names[i]
                };
            }

            // Create the Previous and Next pointers to the other Knots
            for (int i = 0; i < this.Knots.Count(); i++)
            {
                if (i != 0)
                {
                    this.Knots[i].Previous = this.Knots[i - 1];
                }

                if (i != this.Knots.Count() - 1)
                {
                    this.Knots[i].Next = this.Knots[i + 1];
                }
            }

            this.Head = this.Knots.First();
            this.Tail = this.Knots.Last();

            this.TrailTail.Add(start);
        }

        internal void MoveOnePosition(string direction)
        {
            // Move the head
            switch (direction)
            {
                case "U":
                    this.Head.Point.Y++;
                    break;
                case "D":
                    this.Head.Point.Y--;
                    break;
                case "L":
                    this.Head.Point.X--;
                    break;
                case "R":
                    this.Head.Point.X++;
                    break;
            }

            // Move the other knots
            for (int i = 1; i < this.Knots.Length; i++)
            {
                this.MoveKnot(this.Knots[i]);
            }
        }

        internal void MoveKnot(Knot thisKnot)
        {
            // thisKnot moves relative to thisKnot.Previous, which has already moved.
            // calculate the new position of thisKnot and move it.
            // the 'direction' shouldn't matter because the moving-influence is the Previous Knot.
            Point thisPoint = thisKnot.Point;
            Point otherPoint = thisKnot.Previous.Point;

            bool sameRow = thisPoint.Y == otherPoint.Y;
            bool sameColumn = thisPoint.X == otherPoint.X;
            bool diagonal = !sameRow && !sameColumn;

            // May produce a negative number, so make it positive to determine path distance
            int xDistance = otherPoint.X - thisPoint.X;
            int yDistance = otherPoint.Y - thisPoint.Y;

            // Exit early if ...
            if (thisPoint.Equals(otherPoint)
                || (!diagonal && (Math.Abs(xDistance) == 1 || Math.Abs(yDistance) == 1))
                || (diagonal && (Math.Abs(xDistance) == 1 && Math.Abs(yDistance) == 1)))
            {
                // Overlapping Points.
                // Points with a distance of 1 on the same row or column.
                // Points on a diagonal with a distance of 1.
                return;
            }

            // ACTION: Performing the move
            double xCalc = (double)xDistance / 2;
            int xMoveDistance = xCalc < 0 ? (int)Math.Floor(xCalc) : (int)Math.Ceiling(xCalc);
            double yCalc = (double)yDistance / 2;
            int yMoveDistance = yCalc < 0 ? (int)Math.Floor(yCalc) : (int)Math.Ceiling(yCalc);

            thisPoint.X += xMoveDistance;
            thisPoint.Y += yMoveDistance;

            // Record the position of a new Point, if this is the tail
            if (thisKnot.Next == null)
            {
                this.TrailTail.Add(new Point(thisPoint.X, thisPoint.Y));
            }
        }
    }

    class Knot
    {
        public Point Point { get; set; }
        public Knot Previous { get; set; }
        public Knot Next { get; set; }
        public string Name { get; set; }
    }

    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Point other = obj as Point;
            bool same = (this.X == other.X) && (this.Y == other.Y);
            return same;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return (this.X ^ this.Y).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", this.X, this.Y);
        }
    }
}