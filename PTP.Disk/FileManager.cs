using System.Collections.Concurrent;
using System.IO;

namespace PTP.Disk
{
    public class FileManager
    {
        private readonly string dbDiretory;
        private readonly int blockSize;
        private readonly bool isNew;
        private readonly ConcurrentDictionary<string, FileStream> openFiles = new ConcurrentDictionary<string, FileStream>();

        public FileManager(string dbDiretory, int blockSize)
        {
            this.dbDiretory = dbDiretory;
            this.blockSize = blockSize;

            if (!Directory.Exists(dbDiretory))
            {
                Directory.CreateDirectory(dbDiretory);
                isNew = true;
            }

            foreach (var file in Directory.GetFiles(dbDiretory, "temp."))
            {
                File.Delete(file);
            }
        }

        private FileStream GetFile(string filename)
        {
            return openFiles.GetOrAdd(filename, fn =>
            {
                string fullPath = Path.Combine(dbDiretory, fn);
                return new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            });
        }

        public void Read(Block block, Page page)
        {

            FileStream fileStream = GetFile(block.FileName);

            lock (fileStream)
            {
                try
                {
                    fileStream.Seek(block.BlockNumber * blockSize, SeekOrigin.Begin);

                    fileStream.Read(page.Contents(), 0, blockSize);
                }
                catch (IOException)
                {
                    throw new Exception("Error reading block: " + block);
                }
            }
        }

        public void Write(Block block, Page page)
        {
            FileStream fileStream = GetFile(block.FileName);

            lock (fileStream) {
                try
                {
                    fileStream.Seek(block.BlockNumber * blockSize, SeekOrigin.Begin);

                    fileStream.Write(page.Contents(), 0, blockSize);
                }
                catch (IOException)
                {
                    throw new Exception("Error reading block: " + block);
                }
            }
        }

        public Block Append(string fileName)
        {
            int newBlockNumber = Length(fileName);

            Block block = new Block(fileName, newBlockNumber);

            byte[] bytes = new byte[blockSize];

            FileStream fileStream = GetFile(fileName);

            lock(fileStream)
            {
                fileStream.Seek(newBlockNumber * blockSize, SeekOrigin.Begin);
                fileStream.Write(bytes, 0, blockSize);
            }

            return block;
        }

        private int Length(string fileName)
        {
            FileStream fileStream = GetFile(fileName);

            return (int)(fileStream.Length / blockSize);
        }

    }
}
