using PTP.Disk;

namespace PTP.Transaction.Concurrency
{
    public class ConcurrencyManager
    {
        private static LockTable _lockTable = new();
        private Dictionary<Block, string> _locks = new();

        public void SLock(Block block)
        {
            if(!_locks.ContainsKey(block))
            {
                _lockTable.SLock(block);
                _locks.Add(block, "S");
            }
        }

        public void XLock(Block block)
        {
            if(!HasXLock(block))
            {
                SLock(block);
                _lockTable.XLock(block);
                _locks.Add(block, "X");
            }
        }

        public void Release()
        {
            foreach(var block in _locks.Keys)
                _lockTable.Unlock(block);

            _locks.Clear();
        }
        private bool HasXLock(Block block)
        {
            _locks.TryGetValue(block, out string lockType);

            return lockType != null && lockType == "X";
        }

    }
}
