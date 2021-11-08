using System;
using System.IO;
using System.Threading.Tasks;

namespace FileWatcher
{
  class Program
  {
    static void Main(string[] args)
    {
      using var watcher = new FileSystemWatcher(@"/home/danilo/Documents/zi_citanje");

      watcher.NotifyFilter = NotifyFilters.Attributes
                           | NotifyFilters.CreationTime
                           | NotifyFilters.DirectoryName
                           | NotifyFilters.FileName
                           | NotifyFilters.LastAccess
                           | NotifyFilters.LastWrite
                           | NotifyFilters.Security
                           | NotifyFilters.Size;

      watcher.Changed += OnChanged;
      watcher.Created += OnCreated;
      watcher.Deleted += OnDeleted;
      watcher.Renamed += OnRenamed;
      watcher.Error += OnError;

      watcher.Filter = "*.txt";
      watcher.IncludeSubdirectories = true;
      watcher.EnableRaisingEvents = true;

      Console.WriteLine("Press enter to exit.");
      Console.ReadLine();
    }
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
      if (e.ChangeType != WatcherChangeTypes.Changed)
      {
        return;
      }
      Console.WriteLine($"Changed: {e.FullPath}");
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
      string plainText = System.IO.File.ReadAllText(@e.FullPath);
      string cipherText = OneTimePadEncription(plainText);
      string inputPath = e.Name;

      WriteText(cipherText, inputPath);

    }

    private static string OneTimePadEncription(string plainText)
    {
      string key = "ThisIsAHardCodedKey";
      string cipherText = "";

      int[] cipher = new int[key.Length];

      for (int i = 0; i < key.Length; i++)
      {
        cipher[i] = plainText[i] - 'A' + key[i] - 'A';
      }

      for (int i = 0; i < key.Length; i++)
      {
        if (cipher[i] > 25)
        {
          cipher[i] = cipher[i] - 26;
        }
      }

      for (int i = 0; i < key.Length; i++)
      {
        int x = cipher[i] + 'A';
        cipherText += (char)x;
      }

      return cipherText;
    }

    private static string OneTimePadDecription(string cipherText)
    {
      string key = "ThisIsAHardCodedKey";
      string plainText = "";

      int[] plain = new int[key.Length];

      for (int i = 0; i < key.Length; i++)
      {
        plain[i] = cipherText[i] - 'A' - key[i] - 'A';
      }

      for (int i = 0; i < key.Length; i++)
      {
        if (plain[i] < 0)
        {
          plain[i] = plain[i] + 26;
        }
      }

      for (int i = 0; i < key.Length; i++)
      {
        int x = plain[i] + 'A';
        plainText += (char)x;
      }

      return plainText;
    }

    public static async Task WriteText(string criptedText, string path)
    {
      await File.WriteAllTextAsync(Path.Combine("/home/danilo/Documents/zi_upis/", path), criptedText);
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e) =>
        Console.WriteLine($"Deleted: {e.FullPath}");

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
      Console.WriteLine($"Renamed:");
      Console.WriteLine($"    Old: {e.OldFullPath}");
      Console.WriteLine($"    New: {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private static void PrintException(Exception ex)
    {
      if (ex != null)
      {
        Console.WriteLine($"Message: {ex.Message}");
        Console.WriteLine("Stacktrace:");
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine();
        PrintException(ex.InnerException);
      }
    }
  }
}
