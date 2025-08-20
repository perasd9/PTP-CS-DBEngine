using PTP.Shared;
using System.Collections.Concurrent;

namespace PTP.Disk
{
    public class FileManager
    {
        private readonly string _dbDirectory; //folder of files
        private readonly int _blockSize;
        private readonly bool _isNew;
        private readonly ConcurrentDictionary<string, FileStream> _openFiles = new(); //files in directory


        public bool IsNew => _isNew;

        /*
         * Define size of blocks used in files with this instance of FileManager works with.
         * It's used to know structure of blocks and in that way read and write data.
         */
        public int BlockSize => _blockSize;

        /*
         * Making directory(work with more files) and set convention for block structure {_blockSize}.
         * If directory doesn't exist it's making new, if directory exists it deletes everything(cleanup) and prepare for other action.
         */
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

        /*
         * From all files into directory return that {fileName} needed, if file doesn't exist it will make.
         */
        private FileStream GetFile(string fileName)
        {
            return _openFiles.GetOrAdd(fileName, val =>
            {
                string fullPath = Path.Combine(_dbDirectory, val);
                return new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            });
        }

        /*
         * Read method will get file with concrete name(file which contains block => block.FileName), go to offset of that block(with blockNumber * _blockSize)
         * and with FileStream API will read {_blockSize} bytes from file on SEEKED position in memory(p.Contents() which represents our PAGE as memory representation of block)
         */
        public void Read(Block block, Page p)
        {
            var file = GetFile(block.FileName);
            lock (file)
            {
                file.Seek(block.BlockNumber * _blockSize, SeekOrigin.Begin);
                file.Read(p.Contents(), 0, _blockSize);
            }
        }

        /*
         * Write method will get file from all files, seek into position of that block(with blockNumber * _blockSize) and write byte array from Page into file on SEEKED position
         * Flush will guarantee that no additional bytes are left in memory of buffer as cached.
         */
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

        /*
         * {newBlockNum} will be gotten with local Length method by for example file size of 1000MB and convetional BlockSize of 500MB which consequently
         * denote that there is block 0 and block 1 so newBlockNum will be 2 clearly and it will be used as {block} variable and file.Write will write
         * empty(default) byte array of {_blockSize} for making that block ready for usage, again Flush will guarantee no data in buffer stream under the hood.
         */
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

        /*
         * Simply returns number of block(somehow called length) in file.
         * Ex. file of 1000MB({fileName} => file) divided with {_blockSize} for example 500MB will return 2 as output of this method.
         */
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
