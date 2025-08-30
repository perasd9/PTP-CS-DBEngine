using PTP.Disk;
using PTP.Transaction;

namespace PTP.Shared
{
    public class RecordPage
    {
        public static int EMPTY = 0, USED = 1;
        private Transaction.Transaction _transaction;
        private Block _block;
        private Layout _layout;

        public RecordPage(Transaction.Transaction transaction, Block block, Layout layout)
        {
            _transaction = transaction;
            _block = block;
            _layout = layout;

            _transaction.Pin(_block);
        }

        public Block Block => _block;

        public int GetInt(int slot, string fieldName)
        {
            int fieldPosition = this.Offset(slot) + _layout.Offset(fieldName);

            return _transaction.GetInt(_block, fieldPosition);
        }

        public string GetString(int slot, string fieldName)
        {
            int fieldPosition = this.Offset(slot) + _layout.Offset(fieldName);  

            return _transaction.GetString(_block, fieldPosition);
        }

        public void SetInt(int slot, string fieldName, int value)
        {
            int fieldPosition = this.Offset(slot) + _layout.Offset(fieldName);

            _transaction.SetInt(_block, fieldPosition, value, true);
        }

        public void SetString(int slot, string fieldName, string value)
        {
            int fieldPosition = this.Offset(slot) + _layout.Offset(fieldName);

            _transaction.SetString(_block, fieldPosition, value, true);
        }

        public void Delete(int slot)
        {
            this.SetFlag(slot, EMPTY);
        }

        public int NextAfter(int slot)
        {
            return this.SearchAfter(slot, USED);
        }

        public void Format()
        {
            int slot = 0;
            while (this.IsValidSlot(slot))
            {
                _transaction.SetInt(_block, this.Offset(slot), EMPTY, false);
                Schema schema = _layout.Schema;

                foreach(var fieldName in schema.Fields())
                {
                    int fieldPosition = this.Offset(slot) + _layout.Offset(fieldName);

                    if(schema.Type(fieldName) == (int)FieldType.INT) 
                        _transaction.SetInt(_block, fieldPosition, 0, false);
                    else
                        _transaction.SetString(_block, fieldPosition, "", false);
                }
                slot++;
            }
        }

        public int InsertAfter(int slot)
        {
            int newSlot = this.SearchAfter(slot, EMPTY);

            if(newSlot >= 0)
            {
                this.SetFlag(newSlot, USED);
            }

            return newSlot;
        }

        private int SearchAfter(int slot, int flag)
        {
            slot++;
            while(IsValidSlot(slot))
            {
                if(_transaction.GetInt(_block, this.Offset(slot)) == flag)
                {
                    return slot;
                }
                slot++;
            }

            return -1;
        }

        private void SetFlag(int slot, int flag)
        {
            _transaction.SetInt(_block, this.Offset(slot), flag, true);
        }

        private bool IsValidSlot(int slot)
        {
            return this.Offset(slot + 1) <= _transaction.BlockSize;
        }

        private int Offset(int slot)
        {
            return slot * _layout.SlotSize;
        }
    }
}
