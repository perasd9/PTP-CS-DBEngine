using PTP.Buffer;
using PTP.Disk;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    public class RecoveryManager
    {
        /*
         * Recovery manager deals only with logs in log file and based on that logs execute actions, 
         */
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

            StartRecord.WriteToLog(_logManager, transactionNumber); //on constructor it's logical that first log is START
        }

        /*
         * This method will flush transaction's buffer, it seems like it crash rule of WAL but in FlushAll method firsly logs are written and then content.
         * After that Commit record is written to log and LSN which is last will be flushed.
         */
        public void Commit()
        {
            _bufferManager.FlushAll(this._transactionNumber);
            int LSN = CommitRecord.WriteToLog(_logManager, _transactionNumber);
            _logManager.Flush(LSN);
        }

        /*
         * This method will firstly does undo action for all records(see DoRollBack), after that is same as above, flush everything of transaction,
         * write Rollback recrod in log and flush that also.
         */
        public void Rollback()
        {
            DoRollback();
            _bufferManager.FlushAll(this._transactionNumber);
            int LSN = RollbackRecord.WriteToLog(_logManager, _transactionNumber);
            _logManager.Flush(LSN);
        }

        /*
         * This method will as in rollback do the recover with DoRecover method and flush all, write Dheckpoint record and flush that also.
         */
        public void Recover()
        {
            DoRecover();
            _bufferManager.FlushAll(this._transactionNumber);
            int LSN = CheckpointRecord.WriteToLog(_logManager);
            _logManager.Flush(LSN);
        }

        /*
         * This method is used from transaction, just delegating obligations, it's writing record of SetInt record 
         */
        public int SetInt(Buffer.Buffer buffer, int offset, int newValue)
        {
            int oldValue = buffer.Contents.GetInt(offset);
            Block block  = buffer.Block;

            return SetIntRecord.WriteToLog(_logManager, _transactionNumber, block, offset, oldValue); 
        }

        /*
         * This method is used from transaction, just delegating obligations, it's writing record of SetString record 
         */
        public int SetString(Buffer.Buffer buffer, int offset, string newValue)
        {
            string oldValue = buffer.Contents.GetString(offset);
            Block block = buffer.Block;

            return SetStringRecord.WriteToLog(_logManager, _transactionNumber, block, offset, oldValue);
        }

        /*
         * With enumerator(which starts from backward this is really important) iterating through logs based on that logs 
         * ILogRecord(of course implementation class of interface) creates concrete record, and really important part is that if 
         * that record belongs to this transaction number and if operator is not START it should undo action of record.
         */
        private void DoRollback()
        {
            IEnumerator<byte[]> enumerator = _logManager.Enumerator();

            while (enumerator.MoveNext())
            {
                byte[] bytes = enumerator.Current;

                ILogRecord logRecord = ILogRecord.CreateLogRecord(bytes);

                if (logRecord.TransactionNumber() == this._transactionNumber)
                {
                    if (logRecord.Operator() == ILogRecord.START)
                        return;

                    logRecord.Undo(_transaction);
                }
            }
        }

        /*
         * This method will do concrete recover actions, firslt is made empty list of finished transactions(that is important because finished transactions
         * don't need to be recovered or whatever else). As in DoRollback enumerator will iterate through logs from backward, when hit Commit or Rollback record
         * it adds transaction into finished and goes to another records whose transactions are not finished and undo them until next transaction.
         */
        private void DoRecover()
        {
            IList<int> finishedTransactions = new List<int>();
            IEnumerator<byte[]> enumerator = _logManager.Enumerator();

            while(enumerator.MoveNext())
            {
                ILogRecord logRecord = ILogRecord.CreateLogRecord(enumerator.Current);

                if (logRecord.Operator() == ILogRecord.CHECKPOINT)
                    return;
                if(logRecord.Operator() == ILogRecord.COMMIT || logRecord.Operator() == ILogRecord.ROLLBACK)
                {
                    finishedTransactions.Add(logRecord.TransactionNumber());
                } 
                else if(!finishedTransactions.Contains(logRecord.TransactionNumber()))
                {
                    logRecord.Undo(_transaction);
                }
            }

        }
    }
}
