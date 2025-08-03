namespace PTP.Transaction.Recovery
{
    internal class CheckpointRecord : ILogRecord
    {
        public int Operator()
        {
            throw new NotImplementedException();
        }

        public int TransactionNumber()
        {
            throw new NotImplementedException();
        }

        public void Undo(int transactionNumber)
        {
            throw new NotImplementedException();
        }
    }
}