using PTP.Disk;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    internal class StartRecord : ILogRecord
    {
        private int _transactionNumber;

        public StartRecord(Page page)
        {
            int transactionPosition = sizeof(int);
            _transactionNumber = page.GetInt(transactionPosition);
        }

        public int Operator() => ILogRecord.START;

        public int TransactionNumber() => _transactionNumber;

        public void Undo(Transaction transaction)
        {

        }

        public static int WriteToLog(LogManager logManager, int transactionNumber)
        {
            byte[] record = new byte[2 * sizeof(int)];

            Page page = new Page(record);
            
            page.SetInt(0, ILogRecord.START);
            page.SetInt(sizeof(int), transactionNumber);

            return logManager.Append(record);
        }

        public override string ToString() => $"<START {_transactionNumber}>";
    }
}