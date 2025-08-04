using PTP.Disk;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    internal class RollbackRecord : ILogRecord
    {
        private int _transactionNumber;

        public RollbackRecord(Page page)
        {
            int transactionPosition = sizeof(int);
            _transactionNumber = page.GetInt(transactionPosition);
        }

        public int Operator() => ILogRecord.ROLLBACK;

        public int TransactionNumber() => _transactionNumber;

        public void Undo(Transaction transaction)
        {

        }

        public static int WriteToLog(LogManager logManager, int transactionNumber)
        {
            byte[] record = new byte[2 * sizeof(int)];

            Page page = new Page(record);

            page.SetInt(0, ILogRecord.ROLLBACK);
            page.SetInt(sizeof(int), transactionNumber);

            return logManager.Append(record);
        }

        public override string ToString() => $"<ROLLBACK {_transactionNumber}>";
    }
}