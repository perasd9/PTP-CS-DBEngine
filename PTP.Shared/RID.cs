namespace PTP.Shared
{
    public class RID
    {
        private int _blockNumber;
        private int _slotNumber;

        public RID(int blockNumber, int slotNumber)
        {
            _blockNumber = blockNumber;
            _slotNumber = slotNumber;
        }

        public int BlockNumber => _blockNumber;
        public int SlotNumber => _slotNumber;

        public override bool Equals(object? obj)
        {
            if (obj is RID rid)
            {
                return rid.BlockNumber == this.BlockNumber && rid.SlotNumber == this.SlotNumber;
            }
            return false;
        }

        public override string ToString()
        {
            return $"RID[Block: {_blockNumber}, Slot: {_slotNumber}]";
        }
    }
}
