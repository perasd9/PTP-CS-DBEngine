using PTP.Buffer;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    public class RecoveryManager
    {
        private LogManager _logManager;
        private BufferManager _bufferManager;
        private Transaction _transaction;
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

            return 0;
        }

        public int SetString(Buffer.Buffer, int offset, string newValue)
        {

            return 0;
        }

        private void DoRollback()
        {

        }

        private void DoRecover()
        {

        }
    }
}
