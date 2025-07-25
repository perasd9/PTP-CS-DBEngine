using PTP.Disk.Tests;

namespace PTP.DB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            FileManagerTest fileManager = new FileManagerTest();

            fileManager.CanReadAndWriteStringAndInt();
        }
    }
}
