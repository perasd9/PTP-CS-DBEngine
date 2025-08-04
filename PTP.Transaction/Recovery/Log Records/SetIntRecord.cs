using PTP.Disk;
using PTP.Log;

namespace PTP.Transaction.Recovery
{
    internal class SetIntRecord : ILogRecord
    {
        private int _transactionNumber, _offset;
        private int _value;
        private Block _block;

        public SetIntRecord(Page page)
        {
            int transactionPosition = sizeof(int);
            _transactionNumber = page.GetInt(transactionPosition);

            int filePosition = transactionPosition + sizeof(int);
            string fileName = page.GetString(filePosition);

            int blockPosition = filePosition + Page.MaxLength(fileName.Length);
            int blockNumber = page.GetInt(blockPosition);
            _block = new Block(fileName, blockNumber);

            int offsetPosition = blockPosition + sizeof(int);
            _offset = page.GetInt(offsetPosition);

            int valuePosition = offsetPosition + sizeof(int);
            _value = page.GetInt(valuePosition);
        }

        public int Operator() => ILogRecord.SETINT;

        public int TransactionNumber() => _transactionNumber;

        public void Undo(Transaction transaction)
        {
            transaction.Pin(_block);
            transaction.SetInt(_block, _offset, _value, false); // false is really important here due to duplicate log if true is left as default
            transaction.Unpin(_block);
        }

        public static int WriteToLog(LogManager logManager, int transactionNumber, Block block, int offset, int value)
        {
            int transactionPosition = sizeof(int);
            int filePosition = transactionPosition + sizeof(int);
            int blockPosition = filePosition + Page.MaxLength(block.FileName.Length);
            int offsetPosition = blockPosition + sizeof(int);
            int valuePosition = offsetPosition + sizeof(int);

            int recordLength = valuePosition + sizeof(int);

            byte[] record = new byte[recordLength];

            Page page = new Page(record);

            page.SetInt(0, ILogRecord.SETSTRING);
            page.SetInt(transactionPosition, transactionNumber);
            page.SetString(filePosition, block.FileName);
            page.SetInt(blockPosition, block.BlockNumber);
            page.SetInt(offsetPosition, offset);
            page.SetInt(valuePosition, value);

            return logManager.Append(record);
        }

        public override string ToString() => $"<SETINT {_transactionNumber} {_block} {_offset} {_value}>";
    }
}