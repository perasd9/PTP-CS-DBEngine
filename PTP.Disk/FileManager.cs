using PTP.Shared;
using System.Collections.Concurrent;

namespace PTP.Disk
{
    public class FileManager
    {
        private readonly string _dbDirectory;
        private readonly int _blockSize;
        private readonly bool _isNew;
        private readonly ConcurrentDictionary<string, FileStream> _openFiles = new();

        public bool IsNew => _isNew;
        public int BlockSize => _blockSize;

        public FileManager(string dbDirectory, int blockSize)
        {
            dbDirectory = EnvData.EnvDirectory + dbDirectory;

            _dbDirectory = dbDirectory;
            _blockSize = blockSize;

            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
                _isNew = true;
            }

            foreach (var file in Directory.GetFiles(dbDirectory, "temp*"))
            {
                File.Delete(file);
            }
        }

        private FileStream GetFile(string fileName)
        {
            return _openFiles.GetOrAdd(fileName, val =>
            {
                string fullPath = Path.Combine(_dbDirectory, val);
                return new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            });
        }

        public void Read(Block block, Page p)
        {
            var file = GetFile(block.FileName);
            lock (file)
            {
                file.Seek(block.BlockNumber * _blockSize, SeekOrigin.Begin);
                file.Read(p.Contents(), 0, _blockSize);
            }
        }

        public void Write(Block block, Page p)
        {
            var file = GetFile(block.FileName);
            lock (file)
            {
                file.Seek(block.BlockNumber * _blockSize, SeekOrigin.Begin);
                file.Write(p.Contents(), 0, _blockSize);
                file.Flush(true);
            }
        }

        public Block Append(string fileName)
        {
            int newblockNum = Length(fileName);
            var block = new Block(fileName, newblockNum);
            var file = GetFile(fileName);
            lock (file)
            {
                file.Seek(block.BlockNumber * _blockSize, SeekOrigin.Begin);
                file.Write(new byte[_blockSize], 0, _blockSize);
                file.Flush(true);
            }
            return block;
        }

        public int Length(string fileName)
        {
            var file = GetFile(fileName);
            lock (file)
            {
                return (int)(file.Length / _blockSize);
            }
        }
    }
}
