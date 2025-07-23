namespace PTP.Disk
{
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
