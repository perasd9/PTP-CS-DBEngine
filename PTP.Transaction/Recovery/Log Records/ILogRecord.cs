using PTP.Disk;

namespace PTP.Transaction.Recovery
{
    public interface ILogRecord
    {
        public const int CHECKPOINT = 0, START = 1, COMMIT = 2, ROLLBACK = 3, SETINT = 4, SETSTRING = 5;

        public int Operator();
        public int TransactionNumber();
        public void Undo(Transaction transaction);

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
