using PTP.Buffer;
using PTP.Disk;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    public class RecoveryManager
    {
        private readonly LogManager _logManager;
        private readonly BufferManager _bufferManager;
        private readonly Transaction _transaction;
        private int _transactionNumber;

        public RecoveryManager(LogManager logManager, BufferManager bufferManager, Transaction transaction, int transactionNumber)
        {
            _logManager = logManager;
            _bufferManager = bufferManager;
            _transaction = transaction;
            _transactionNumber = transactionNumber;

            StartRecord.WriteToLog(_logManager, transactionNumber);
        }

        public void Commit()
        {
            _bufferManager.FlushAll(this._transactionNumber);
            int LSN = CommitRecord.WriteToLog(_logManager, _transactionNumber);
            _logManager.Flush(LSN);
        }

        public void Rollback()
        {
            DoRollback();
            _bufferManager.FlushAll(this._transactionNumber);
            int LSN = RollbackRecord.WriteToLog(_logManager, _transactionNumber);
            _logManager.Flush(LSN);
        }

        public void Recover()
        {
            DoRecover();
            _bufferManager.FlushAll(this._transactionNumber);
            int LSN = CheckpointRecord.WriteToLog(_logManager);
            _logManager.Flush(LSN);
        }

        public int SetInt(Buffer.Buffer buffer, int offset, int newValue)
        {
            int oldValue = buffer.Contents.GetInt(offset);
            Block block  = buffer.Block;

            return SetIntRecord.WriteToLog(_logManager, _transactionNumber, block, offset, oldValue); 
        }

        public int SetString(Buffer.Buffer buffer, int offset, string newValue)
        {
            string oldValue = buffer.Contents.GetString(offset);
            Block block = buffer.Block;

            return SetStringRecord.WriteToLog(_logManager, _transactionNumber, block, offset, oldValue);
        }

        private void DoRollback()
        {

        }

        private void DoRecover()
        {

        }
    }
}
