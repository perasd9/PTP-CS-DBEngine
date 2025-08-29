using PTP.Disk;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    internal class StartRecord : ILogRecord
    {
        //Describe which transaction this record belongs.
        private int _transactionNumber;

        /*
         * After first 4B(int) for operator, second 4B(int) is transaction number so it's read like this.
         */
        public StartRecord(Page page)
        {
            int transactionPosition = sizeof(int);
            _transactionNumber = page.GetInt(transactionPosition);
        }

        public int Operator() => ILogRecord.START;

        public int TransactionNumber() => _transactionNumber;

        /*
         * There is no undo action for commit log record.
         */
        public void Undo(Transaction transaction)
        {

        }

        /*
         * Start record is made by operator on first 4B and transaction number on second 4B so page of 2 * sizeof(int) is created by that idea.
         * At the end this method returns returned value from logManager.Append method and this is important because returend value is latest LSN.
         */
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