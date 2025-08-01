using PTP.Disk;
using PTP.Log;

namespace PTP.Buffer
{
    public class Buffer
    {
        private readonly FileManager _fileManager;
        private readonly LogManager _logManager;
        private readonly Page _contents;
        private Block _block;
        private int _pins;
        private int _transactionNumber = -1;
        private int _LSN = -1;

        public Buffer(FileManager fileManager, LogManager logManager)
        {
            this._fileManager = fileManager;
            this._logManager = logManager;
            this._contents = new Page(new byte[fileManager.BlockSize]);
        }

        public Page Contents => _contents;
        public Block Block => _block;
        public bool IsPinned => _pins > 0;
        public int ModifyingTransactionNumber => _transactionNumber;

        public void SetModified(int transactionNumber, int LSN)
        {
            this._transactionNumber = transactionNumber;

            if (this._LSN >= 0) this._LSN = LSN;
        }

        internal void Pin()
        {
            this._pins++;
        }

        internal void Unpin()
        {
            if (this._pins <= 0)
                throw new InvalidOperationException("Buffer is not pinned.");
            this._pins--;
        }

        internal void Flush()
        {
            if(this._transactionNumber > 0)
            {
                _logManager.Flush(this._LSN);
                _fileManager.Write(_block, _contents);
                this._transactionNumber = -1;
            }
        }

        internal void AssignToBlock(Block block)
        {
            this.Flush();
            this._block = block;
            this._fileManager.Read(block, _contents);
            this._pins = 0;
        }

    }
}
