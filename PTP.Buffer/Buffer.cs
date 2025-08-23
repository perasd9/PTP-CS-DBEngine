using PTP.Disk;
using PTP.Log;

namespace PTP.Buffer
{
    public class Buffer
    {
        /*
         * Buffer is made for pin and unpin page, buffer is internal representation of page actually, it serves as a layer above file manager
         * which works directly with the disk. Buffer has page as field {_contents} and {_fileManager} to work with that page(load from block and write into block).
         * One buffer can have more pins, consequently logical because same page can be used by more clients at the same time, concurrency is solved in concurrency manager.
         */
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

        /*
         * This method denote buffer as a dirty which means some transaction modified content, that means also that LSN is changed so it has to be updated.
         * This is important for Flush method because this is the only way how page can be denoted as dirty.
         */
        public void SetModified(int transactionNumber, int LSN)
        {
            this._transactionNumber = transactionNumber;

            if (this._LSN >= 0) this._LSN = LSN;
        }

        /*
         * Increment number of pins on this buffer.
         */
        internal void Pin()
        {
            this._pins++;
        }

        /*
         * Decrement number of pins on this buffer.
         */
        internal void Unpin()
        {
            if (this._pins <= 0)
                throw new InvalidOperationException("Buffer is not pinned.");
            this._pins--;
        }

        /*
         * This method checks if buffer is dirty which means if content is changed in memory by some transaction page needs to be written on disk.
         * This is very important becaues by this disk acceses can be decreased the way that content is flushed into disk only if page is modified.
         */
        internal void Flush()
        {
            if(this._transactionNumber > 0)
            {
                _logManager.Flush(this._LSN);
                _fileManager.Write(_block, _contents);
                this._transactionNumber = -1;
            }
        }

        /*
         * This method actually connect block from disk and load intp {_contents} page, before that it can be ensured that content is written on disk
         * so it calls Flush method, after that just load block into page and reset pins on zero.
         */
        internal void AssignToBlock(Block block)
        {
            this.Flush();
            this._block = block;
            this._fileManager.Read(block, _contents);
            this._pins = 0;
        }

    }
}
