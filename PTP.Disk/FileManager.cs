using System.Collections.Concurrent;
using System.IO;

namespace PTP.Disk
{
    public class FileManager
    {
        private readonly string _dbDirectory;
        private readonly int _blockSize;
        private readonly bool _isNew;
        private readonly ConcurrentDictionary<string, FileStream> _openFiles = new();

        public bool IsNew() => _isNew;
        public int BlockSize() => _blockSize;

        public FileManager(string dbDirectory, int blockSize)
        {
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

        private FileStream GetFile(string FileName)
        {
            return _openFiles.GetOrAdd(FileName, fn =>
            {
                string fullPath = Path.Combine(_dbDirectory, fn);
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

        public Block Append(string FileName)
        {
            int newblockNum = Length(FileName);
            var block = new Block(FileName, newblockNum);
            var file = GetFile(FileName);
            lock (file)
            {
                file.Seek(block.BlockNumber * _blockSize, SeekOrigin.Begin);
                file.Write(new byte[_blockSize], 0, _blockSize);
                file.Flush(true);
            }
            return block;
        }

        public int Length(string FileName)
        {
            var file = GetFile(FileName);
            lock (file)
            {
                return (int)(file.Length / _blockSize);
            }
        }
    }
}
