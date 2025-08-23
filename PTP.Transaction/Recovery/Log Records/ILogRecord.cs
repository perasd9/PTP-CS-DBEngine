using PTP.Disk;

namespace PTP.Transaction.Recovery
{
    public interface ILogRecord
    {
        //Message types in log file.
        public const int CHECKPOINT = 0, START = 1, COMMIT = 2, ROLLBACK = 3, SETINT = 4, SETSTRING = 5;

        //Every log record(implementation of interface) will return one of message type.
        public int Operator();
        //From start transaction til the end for every message it has to be known to which transaction belongs.
        public int TransactionNumber();
        //In this db system decision for recovery is UNDO only approach, so this is compesantion like operation, back action on old value.
        public void Undo(Transaction transaction);

        /*
         * Based on first 4B(int) create conrete implementation instance of some Log Record.
         */
        static ILogRecord CreateLogRecord(byte[] bytes)
        {
            Page page = new Page(bytes);

            switch(page.GetInt(0))
            {
                case CHECKPOINT:
                    return new CheckpointRecord();
                case START:
                    return new StartRecord(page);
                case COMMIT:
                    return new CommitRecord(page);
                case ROLLBACK:
                    return new RollbackRecord(page);
                case SETINT:
                    return new SetIntRecord(page);
                case SETSTRING:
                    return new SetStringRecord(page);

                default:
                    return null;
            }
        }

    }
}
