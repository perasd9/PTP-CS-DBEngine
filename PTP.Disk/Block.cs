namespace PTP.Disk
{
    /*
     * Example of block is file "database.db" and in that file serial number of block.
     * One file "database.db" has 10 blocks od 500MB. Identify data on concrete byte is like
     * know in which file data is stored and in which block, in that block later offset will be used
     * for accessing specific value.
     */
    public class Block
    {
        private readonly string _fileName;
        private readonly int _blockNumber;

        public string FileName => _fileName;
        public int BlockNumber => _blockNumber;

        public Block(string fileName, int blockNumber)
        {
            _fileName = fileName;
            _blockNumber = blockNumber;
        }

        public override string ToString()
        {
            return $"Block: FileName={_fileName}, BlockNumber={_blockNumber}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Block other)
            {
                return _fileName == other._fileName && _blockNumber == other._blockNumber;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_fileName, _blockNumber);
        }
    }
}
