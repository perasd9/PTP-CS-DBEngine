using PTP.Disk;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    internal class CheckpointRecord : ILogRecord
    {
        public int Operator() => ILogRecord.CHECKPOINT;

        //There is no transaction for checkpoint log record, only purpose of this record is for recovery, to mark til this point is everything right.
        public int TransactionNumber()
        {
            return -1;
        }

        /*
         * There is no undo action for checkpoint log record.
         */
        public void Undo(Transaction transaction)
        {

        }

        /*
         * Checkpoint record is made only from the 4B for operator so page with sizeof(int) is made, return value is returned latest LSN by append method.
         */
        public static int WriteToLog(LogManager logManager)
        {
            byte[] record = new byte[sizeof(int)];

            Page page = new Page(record);

            page.SetInt(0, ILogRecord.CHECKPOINT);

            return logManager.Append(record);
        }

        public override string ToString() => $"<CHECKPOINT>";
    }
}