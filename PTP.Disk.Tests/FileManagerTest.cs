namespace PTP.Disk.Tests
{
    public class FileManagerTest
    {
        [Fact]
        public void CanReadAndWriteStringAndInt()
        {
            string dbName = "testFolder";
            if(Directory.Exists(dbName))
                Directory.Delete(dbName, true);

            int blockSize = 4096;

            FileManager fileManager = new FileManager(dbName, blockSize);

            Block block = new Block("testFile", 2);
            
            Page page = new Page(fileManager.BlockSize);

            int pos1 = 88;
            string testString = "Hello, World!";

            page.SetString(pos1, testString);

            int size = Page.MaxLength(testString.Length);

            int pos2 = pos1 + size;

            page.SetInt(pos2, 345);

            fileManager.Write(block, page);

            Page readPage = new Page(fileManager.BlockSize);

            fileManager.Read(block, readPage);

            int readInt = readPage.GetInt(pos2);
            string readString = readPage.GetString(pos1);

            Assert.Equal(345, readInt);
            Assert.Equal(testString, readString);

            Console.WriteLine($"offset {pos1} contains {readString}");
            Console.WriteLine($"offset {pos2} contains {readInt}");
        }

    }
}
