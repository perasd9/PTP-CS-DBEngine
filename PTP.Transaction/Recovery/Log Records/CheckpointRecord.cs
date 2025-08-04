using PTP.Disk;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    internal class CheckpointRecord : ILogRecord
    {
        public int Operator() => ILogRecord.CHECKPOINT;

        public int TransactionNumber()
        {
            return -1;
        }

        public void Undo(Transaction transaction)
        {

        }

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