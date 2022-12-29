using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        const int CharacterBufferSize = 14;
        string content = File.ReadAllText(@"day6-input.txt");

        Queue<char> q = new Queue<char>(content.Take(CharacterBufferSize));
        int position = 0;

        // Read input until the end
        for (int i = CharacterBufferSize; i < content.Length; i++)
        {
            position = i;
            if (q.Distinct().Count() == CharacterBufferSize)
            {
                break;
            }

            q.Dequeue();
            q.Enqueue(content[i]);
        }

        Console.WriteLine("Marker position: {0}", position);
    }
}