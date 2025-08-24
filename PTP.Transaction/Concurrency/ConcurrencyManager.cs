using PTP.Disk;

namespace PTP.Transaction.Concurrency
{
    public class ConcurrencyManager
    {
        private static LockTable _lockTable = new(); //this field is used for delegate responsibility to just lock and release 
        private Dictionary<Block, string> _locks = new(); //map of locked blocks(block is chosen as level of granularity)

        /*
         * Shared lock is acquired only if no exclusive lock is held by another transaction on the same block.
         */
        public void SLock(Block block)
        {
            if(!_locks.ContainsKey(block))
            {
                _lockTable.SLock(block);
                _locks.Add(block, "S");
            }
        }

        /*
         * Exclusive lock is acquired only if no other locks (shared or exclusive) are held by another transaction on the same block.
         */
        public void XLock(Block block)
        {
            if(!HasXLock(block))
            {
                SLock(block);
                _lockTable.XLock(block);
                _locks.Add(block, "X");
            }
        }

        /*
         * Release all locks held by the current transaction.
         */
        public void Release()
        {
            foreach(var block in _locks.Keys)
                _lockTable.Unlock(block);

            _locks.Clear();
        }

        /*
         * Check if exclusive lock is acquired on the block.
         */
        private bool HasXLock(Block block)
        {
            _locks.TryGetValue(block, out string lockType);

            return lockType != null && lockType == "X";
        }
    }
}
