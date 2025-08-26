using PTP.Disk;
using PTP.Log;
using System;
using System.Data.SqlTypes;

namespace PTP.Transaction.Recovery
{
    internal class SetStringRecord : ILogRecord
    {
        /*
         * After first 4B(int) for operator, second 4B(int) is transaction number, third is fileName of string length,
         * fourth is blockPosition, fifth is blockNumber, sixth is offsetPosition, seventh is valuePosition so it's read like this.
         */
        private int _transactionNumber, _offset;
        private string _value;
        private Block _block;

        public SetStringRecord(Page page)
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
            _value = page.GetString(valuePosition);
        }

        public int Operator() => ILogRecord.SETSTRING;

        public int TransactionNumber() => _transactionNumber;

        /*
         * Undo operation for setString is to back string value on the old one. Fristly pin page(pin is done by buffer manager to make page available in pool).
         * SetInt is next where setInt method of transaction does properly set of old value within transaction.
         * At the end unpin method will probably flush all in some moment and release page in pool for other pages.
         */
        public void Undo(Transaction transaction)
        {
            transaction.Pin(_block);
            transaction.SetString(_block, _offset, _value, false); // false is really important here due to duplicate log if true is left as default
            transaction.Unpin(_block);
        }

        /*
         * Like above, offset for every value is calculated and at the end {logManager} will write that in log file and return latest LSN.
         */
        public static int WriteToLog(LogManager logManager, int transactionNumber, Block block, int offset, string value)
        {
            int transactionPosition = sizeof(int);
            int filePosition = transactionPosition + sizeof(int);
            int blockPosition = filePosition + Page.MaxLength(block.FileName.Length);
            int offsetPosition = blockPosition + sizeof(int);
            int valuePosition = offsetPosition + sizeof(int);

            int recordLength = valuePosition + Page.MaxLength(value.Length);

            byte[] record = new byte[recordLength];

            Page page = new Page(record);

            page.SetInt(0, ILogRecord.SETSTRING);
            page.SetInt(transactionPosition, transactionNumber);
            page.SetString(filePosition, block.FileName);
            page.SetInt(blockPosition, block.BlockNumber);
            page.SetInt(offsetPosition, offset);
            page.SetString(valuePosition, value);

            return logManager.Append(record);
        }

        public override string ToString() => $"<SETSTRING {_transactionNumber} {_block} {_offset} {_value}>";
    }
}