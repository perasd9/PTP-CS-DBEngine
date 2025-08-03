using PTP.Disk;

namespace PTP.Transaction.Recovery
{
    internal class SetStringRecord : ILogRecord
    {
        private Page page;

        public SetStringRecord(Page page)
        {
            this.page = page;
        }

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