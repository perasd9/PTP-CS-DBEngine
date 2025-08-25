using PTP.Buffer;
using PTP.Disk;
using PTP.Log;
using PTP.Transaction.Concurrency;
using PTP.Transaction.Recovery;

namespace PTP.Transaction
{
    public class Transaction
    {
        private static int _nextTransactionNumber = 0;
        private static int END_OF_FILE = -1;
        private RecoveryManager _recoveryManager;
        private ConcurrencyManager _concurrencyManager;
        private BufferManager _bufferManager;
        private FileManager _fileManager;
        private int _transactionNumber;
        private BufferList _bufferList;

        public Transaction(FileManager fileManager, BufferManager bufferManager, LogManager logManager)
        {
            _fileManager = fileManager;
            _bufferManager = bufferManager;
            _recoveryManager = new RecoveryManager(logManager, bufferManager, this, _nextTransactionNumber);
            _concurrencyManager = new ConcurrencyManager();
            _transactionNumber = NextTransactionNumber();
            _bufferList = new BufferList(bufferManager);
        }

        private int NextTransactionNumber()
        {
            lock(this)
            {
                return _nextTransactionNumber++;
            }
        }

        public void Commit()
        {
            _recoveryManager.Commit();
            _concurrencyManager.Release();
            _bufferList.UnpinAll();
        }

        public void Rollback()
        {
            _recoveryManager.Rollback();
            _concurrencyManager.Release();
            _bufferList.UnpinAll();
        }

        public void Recover()
        {
            _bufferManager.FlushAll(this._transactionNumber);
            _recoveryManager.Recover();
        }

        public int Size(string filename)
        {
            Block dummy = new Block(filename, END_OF_FILE);
            _concurrencyManager.SLock(dummy);

            return _fileManager.Length(filename);
        }

        public Block Append(string filename)
        {
            Block dummy = new Block(filename, END_OF_FILE);
            _concurrencyManager.SLock(dummy);

            return _fileManager.Append(filename);
        }

        public int BlockSize => _fileManager.BlockSize;

        public int AvailableBuffers => _bufferManager.AvailableBuffers();

        public int GetInt(Block block, int offset)
        {
            _concurrencyManager.SLock(block);

            return _bufferList.GetBuffer(block).Contents.GetInt(offset);
        }


        public string GetString(Block block, int offset)
        {
            _concurrencyManager.SLock(block);

            return _bufferList.GetBuffer(block).Contents.GetString(offset);
        }

        public void SetInt(Block block, int offset, int value, bool okToLog)
        {
            _concurrencyManager.XLock(block);
            Buffer.Buffer buffer = _bufferList.GetBuffer(block);
            int LSN = -1;

            if(okToLog)
                LSN = _recoveryManager.SetInt(buffer, offset, value);
            Page page = buffer.Contents;

            page.SetInt(offset, value);
            buffer.SetModified(_transactionNumber, LSN);
        }

        public void SetString(Block block, int offset, string value, bool okToLog)
        {
            _concurrencyManager.XLock(block);
            Buffer.Buffer buffer = _bufferList.GetBuffer(block);
            int LSN = -1;

            if (okToLog)
                LSN = _recoveryManager.SetString(buffer, offset, value);
            Page page = buffer.Contents;

            page.SetString(offset, value);
            buffer.SetModified(_transactionNumber, LSN);
        }
        public void Pin(Block block)
        {
            _bufferList.Pin(block);
        }

        public void Unpin(Block block)
        {
            _bufferList.Unpin(block);
        }
    }
}
