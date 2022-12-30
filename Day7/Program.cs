// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");

string input = File.ReadAllText(@"input.txt");
List<string> parts = input.Split('\n').ToList();

int fileMaxLimit = 100000;
Directory tree = new Directory();

// Parse the input to a Directory tree
for (int i = 0; i < parts.Count; i++)
{
    tree = ParseInput(parts[i], 0, tree);
}

// Move pointer to the root
tree = tree.Parent;

Dictionary<string, int> results = new Dictionary<string, int>();
GetDirectorySizes(tree, results);

// Find dirs < 100000 bytes
int sum = results.Where((x, y) => x.Value <= fileMaxLimit).Select(x => x.Value).Sum();

Console.WriteLine("Part 1: Sum with > {0} = {1} {2}", fileMaxLimit, sum, Environment.NewLine);

// PART 2
const int SizeFileSystem = 70000000;
const int SizeRequired = 30000000;
int totalInUse = GetDirectorySize(tree);

int currentFreeSpace = SizeFileSystem - totalInUse;
int sizeToDelete = SizeRequired - currentFreeSpace;
Console.WriteLine("Free space needed: {0}", sizeToDelete);

// Find an element in 'results' closest to 'sizeToDelete'
var candidates = results.Where(x => x.Value >= sizeToDelete).Select(x => x.Value);
var sorted = candidates.Order();
int candidate = sorted.First();

Console.WriteLine("Part 2: Size to delete: {0}", candidate);

int GetDirectorySize(Directory currentPath)
{
    int fileSizes = currentPath.Files.Sum(x => x.Value);
    int childDirectorySizes = 0;
    foreach (var childDir in currentPath.Directories)
    {
        childDirectorySizes += GetDirectorySize(childDir);
    }

    int total = fileSizes + childDirectorySizes;
    return total;
}

void GetDirectorySizes(Directory currentPath, Dictionary<string, int> results)
{
    Console.WriteLine("Result count: {0}", results.Count);
    
    string? key = currentPath.Parent != null ? 
        string.Concat(currentPath.Parent.Name, "-", currentPath.Name) : 
        currentPath.Name;

    int size = GetDirectorySize(currentPath);
    results.Add(key, size);

    if (currentPath.Directories.Count > 0)
    {
        foreach (var item in currentPath.Directories)
        {
            GetDirectorySizes(item, results);
        }
    }

    currentPath = currentPath.Parent;
}

Directory ParseInput(string input, int i, Directory current)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        return current;
    }

    Console.WriteLine(" * {0}", input);

    if (input.StartsWith("$ cd "))
    {
        string target = input.Split(' ').Last();
        if (target == "/")
        {
            current.Name = target;
            current.Parent = null;
        }
        else if (target == "..")
        {
            current = current.Parent;
        }
        else // e.g. 'cd dirName'
        {
            current = current.Directories.Find(x => x.Name == target);
        }
    }
    else if (input.StartsWith("$ ls"))
    {
        // do nothing
    }
    else if (input.StartsWith("dir"))
    {
        string target = input.Split(' ').Last();
        // lists a directory
        Directory d = new Directory() { Name = target, Parent = current };
        current.Directories.Add(d);
    }
    else
    {
        // its a file
        string[] fileParts = input.Split(' ');
        int fileSize = int.Parse(fileParts[0]);
        string fileName = fileParts[1];
        current.Files.Add(fileName, fileSize);
    }

    return current;
}

class Directory
{
    public Directory? Parent { get; set; }
    public string? Name { get; set; }
    public Dictionary<string,int>? Files { get; set; }
    public List<Directory>? Directories { get; set; }
    public Directory ()
    {
        this.Directories = new List<Directory> ();
        this.Files= new Dictionary<string,int> ();
    }
}