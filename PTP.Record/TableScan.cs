using PTP.Disk;
using PTP.Query;
using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.Shared
{
    public class TableScan : IUpdateScan
    {
        private Transaction.Transaction _transaction;
        private Layout _layout;
        private RecordPage _recordPage;
        private string _fileName;
        private int _currentSlot;

        public TableScan(Transaction.Transaction transaction, Layout layout, string fileName)
        {
            _transaction = transaction;
            _layout = layout;
            _fileName = fileName + ".tbl";

            if (_transaction.Size(_fileName) == 0)
                this.MoveToNewBlock();
            else
                this.MoveToBlock(0);
        }

        public void Close()
        {
            if(_recordPage != null)
                _transaction.Unpin(_recordPage.Block);
        }

        public RID GetRID()
        {
            return new RID(_recordPage.Block.BlockNumber, _currentSlot);
        }

        private void MoveToBlock(int blockNumber)
        {
            this.Close();

            Block block = new Block(_fileName, blockNumber);

            _recordPage = new RecordPage(_transaction, block, _layout);

            _currentSlot = -1;
        }

        private void MoveToNewBlock()
        {
            this.Close();

            Block block = _transaction.Append(_fileName);

            _recordPage = new RecordPage(_transaction, block, _layout);

            _recordPage.Format();
            _currentSlot = -1;
        }

        private bool IsAtLastBlock()
        {
            return _recordPage.Block.BlockNumber == _transaction.Size(_fileName) - 1;
        }

        public void SetInt(string fieldName, int value)
        {
            _recordPage.SetInt(_currentSlot, fieldName, value);
        }

        public void SetString(string fieldName, string value)
        {
            _recordPage.SetString(_currentSlot, fieldName, value);
        }

        public void SetValue(string fieldName, Constant value)
        {
            if(_layout.Schema.Type(fieldName) == (int)FieldType.INT)
            {
                this.SetInt(fieldName, value.AsInt());
            }
            else
            {
                this.SetString(fieldName, value.AsString());
            }
        }

        public void Insert()
        {
            _currentSlot = _recordPage.InsertAfter(_currentSlot);
            while(_currentSlot < 0)
            {
                if (this.IsAtLastBlock())
                    this.MoveToNewBlock();
                else
                    this.MoveToBlock(_recordPage.Block.BlockNumber + 1);
                _currentSlot = _recordPage.InsertAfter(_currentSlot);
            }
        }

        public void Delete()
        {
            _recordPage.Delete(_currentSlot);
        }

        public void MoveToRID(RID rid)
        {
            this.Close();

            Block block = new Block(_fileName, rid.BlockNumber);

            _recordPage = new RecordPage(_transaction, block, _layout);

            _currentSlot = rid.SlotNumber;
        }

        public void BeforeFirst()
        {
            this.MoveToBlock(0);
        }

        public bool Next()
        {
            _currentSlot = _recordPage.NextAfter(_currentSlot);

            while(_currentSlot < 0)
            {
                if (this.IsAtLastBlock()) return false;

                this.MoveToBlock(_recordPage.Block.BlockNumber + 1);

                _currentSlot = _recordPage.NextAfter(_currentSlot);
            }

            return true;
        }

        public int GetInt(string fieldName)
        {
            return _recordPage.GetInt(_currentSlot, fieldName);
        }

        public string GetString(string fieldName)
        {
            return _recordPage.GetString(_currentSlot, fieldName);
        }

        public Constant GetValue(string fieldName)
        {
            if(_layout.Schema.Type(fieldName) == (int)FieldType.INT)
            {
                return new Constant(this.GetInt(fieldName));
            }
            else
            {
                return new Constant(this.GetString(fieldName));
            }
        }

        public bool HasField(string fieldName)
        {
            return _layout.Schema.HasField(fieldName);
        }
    }
}
