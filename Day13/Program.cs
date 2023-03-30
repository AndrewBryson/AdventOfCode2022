using System.Collections.Generic;

namespace Day13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int sum = 0;
            //string[] lines = File.ReadAllLines(@"day13-testdata.txt");
            string[] lines = File.ReadAllLines(@"day13-input.txt");

            // Remove blank lines
            lines = lines.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            for (int i = 0; i < lines.Length; i += 2)
            {
                string leftRaw = lines[i];
                string rightRaw = lines[i + 1];

                Packet left = ParseToPacket(leftRaw);
                Packet right = ParseToPacket(rightRaw);

                int packetIndex = ((i + 2) / 2);
                Console.WriteLine("Packet {2}: {0} v {1}", leftRaw, rightRaw, packetIndex);
                int result = WalkPackets(left.List, right.List, packetIndex);
                Console.WriteLine(" -> {0}", result);
                if (result > 0) { sum += result; }

                /*
                dynamic left = GetListOfNumbers(leftRaw);
                List<string> leftNumbers = left.Numbers;
                leftPointer = left.Pointer;

                dynamic right = GetListOfNumbers(rightRaw);
                List<string> rightNumbers = right.Numbers;
                rightPointer = right.Pointer;

                if (leftNumbers.Count == 0 && rightNumbers.Count == 0)
                {
                    sum++;
                }
                else if (leftNumbers.Count == 0)
                {
                    continue;
                }

                var leftList = leftNumbers[0].Split(',');
                Console.WriteLine("l: {0} => {1}", leftRaw, string.Join(",", leftList));
                var rightList = rightNumbers[0].Split(',');
                Console.WriteLine("r: {0} => {1}", rightRaw, string.Join(",", rightList));

                for (int n = 0; n < leftNumbers.Count; n++)
                {
                    int order = CheckNumberOrder(int.Parse(leftNumbers[n]), int.Parse(rightNumbers[n]));
                    if (order == 1) sum += (i + 1);
                    if (order == -1) continue;
                }
                */
                //Console.WriteLine();
            }

            Console.WriteLine("Final sum: {0}", sum);
        }

        private static int WalkPackets(List left, List right, int packetIndex)
        {
            int loopCount = 0;

            while (true)
            {
                bool leftAvailable = (loopCount < left.Contents.Count);
                bool rightAvailable = (loopCount < right.Contents.Count);

                // If the left list runs out of items first, the inputs are in the right order
                if (!leftAvailable && rightAvailable) { return packetIndex; }

                // If the right list runs out of items first, the inputs are not in the right order
                if (leftAvailable && !rightAvailable) { return -1; }

                // If the lists are the same length and no comparison makes a decision about the order, continue checking the next part of the input.
                if (!leftAvailable || !rightAvailable) { return 0; }

                var leftItem = left.Contents[loopCount];
                var rightItem = right.Contents[loopCount];
                Type leftType = leftItem.GetType();
                Type rightType = rightItem.GetType();

                if (leftType == rightType)
                {
                    if (leftType == typeof(int))
                    {
                        int result = CheckNumberOrder((int)leftItem, (int)rightItem);
                        if (result == 1) { return packetIndex; }
                        if (result == -1) { return 0; }
                    }
                    else // typeof(List)
                    {
                        int result = WalkPackets((List)leftItem, (List)rightItem, packetIndex);
                        if (result == packetIndex) { return packetIndex; }
                        if (result == -1) { return 0; };
                        //if (result == 0) { return 0; }
                    }
                }
                else // mixed types, ensure both are lists and compare
                {
                    List leftList = new List();
                    List rightList = new List();

                    int result;
                    if (leftType == typeof(int))
                    {
                        leftList.Contents.Add(leftItem);
                        rightList = (List)rightItem;
                    }
                    else
                    {
                        leftList = (List)leftItem;
                        rightList.Contents.Add(rightItem);
                    }

                    result = WalkPackets(leftList, rightList, packetIndex);
                    if (result == packetIndex) { return packetIndex; }
                    else if (result == -1) { return 0; }
                    else { throw new Exception("Erk!"); }
                }

                loopCount++;
            }
        }

        private static Packet ParseToPacket(string raw)
        {
            char[] terminators = new char[] { ']', ',' };

            Packet p = new Packet() { Raw = raw };
            bool started = false;
            List current = p.List;

            for (int i = 0; i < raw.Length;)
            {
                int increment = 1;
                char token = raw[i];

                switch (token)
                {
                    case '[':
                        if (!started)
                        {
                            started = true;
                        }
                        else
                        {
                            //previous = current;
                            List inner = new List() { Previous = current };
                            current.Contents.Add(inner);
                            current = inner;
                        }

                        break;

                    case ']':
                        // end the current list
                        current = current.Previous;
                        break;

                    case ',':
                        // no operation
                        break;

                    default: // a number and not any of these: '[', ']', ','
                        int distance = (raw.IndexOfAny(terminators, i) - i);
                        //Console.WriteLine("IDX: {0}, distance: {1}", i, distance);

                        int n = int.Parse(raw.Substring(i, distance));
                        current.Contents.Add(n);
                        //Console.WriteLine(n);
                        increment = distance;
                        break;
                }

                i = i + increment;
            }

            return p;
        }

        private static int CheckNumberOrder(int l, int r)
        {
            if (l < r) return 1;
            else if (l > r) return -1;
            else return 0;
        }

        //    public static dynamic GetListOfNumbers(string raw)
        //    {
        //        var nextIndex = raw.IndexOfAny(new char[] { ']', '[' });

        //        var nums = raw.Split(new char[] { ']', '[' })
        //            .ToList<string>()
        //            .Where(x => !string.IsNullOrEmpty(x))
        //            .ToList();

        //        return new { Numbers = nums, Pointer = nextIndex };
        //    }

        //    private static List<object> GetListFromPacket(string packet)
        //    {
        //        // [1,[2,[3,[4,[5,6,7]]]],8,9]
        //        List<object> list = new List<object>();
        //        var current = list;

        //        for (int i = 1; i < packet.Length; i++)
        //        {
        //            string c = packet[i].ToString();

        //            switch (c)
        //            {
        //                case "[":
        //                    var newList = new List<object>();
        //                    list.Add(newList);
        //                    current = newList;
        //                    break;

        //                default:
        //                    break;
        //            }
        //        }

        //        return list;
        //    }

    }

    class Packet
    {
        public string Raw { get; set; }
        public List List { get; set; }
        public Packet()
        {
            this.List = new List();
        }
    }

    class List
    {
        public List Previous { get; set; }
        public List<object> Contents { get; set; }
        public List()
        {
            this.Contents = new List<object>();
        }
    }
}