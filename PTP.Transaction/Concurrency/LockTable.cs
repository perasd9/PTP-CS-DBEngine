using PTP.Disk;
using PTP.Shared.Exceptions;

namespace PTP.Transaction.Concurrency
{
    public class LockTable
    {
        //Time using to wait for resource to release lock, if this expires exception(BufferAbort) will be thrown.
        private const int MAX_TIME = 10_000; // time in milliseconds

        private readonly Dictionary<Block, int> _locks = new(); //Denote which block(this is specifed item of granularity here) is locked(X or S).

        /*
         * Acquire a shared lock on the specified block. If time expires exception is thrown. Possible deadlock.
         */
        public void SLock(Block block)
        {
            lock(this)
            {
                try
                {
                    int startTime = DateTime.Now.Millisecond;
                    while(HasXLock(block) && !WaitingTooLong(startTime))
                    {
                        Monitor.Wait(this, MAX_TIME);
                    }

                    if (HasXLock(block))
                        throw new LockAbortException();

                    int value = GetLockValue(block);

                    _locks.Add(block, value + 1);
                }
                catch (Exception)
                {
                    throw new LockAbortException();
                }
            }
        }

        /*
         * Acquire a exclusive lock on the specified block. If time expires exception is thrown. Possible deadlock.
         */
        public void XLock(Block block)
        {
            lock(this)
            {
                try
                {
                    int startTime = DateTime.Now.Millisecond;

                    while (HasOtherSLocks(block) && !WaitingTooLong(startTime))
                    {
                        Monitor.Wait(this, MAX_TIME);
                    }

                    if (HasOtherSLocks(block))
                        throw new LockAbortException();
                }
                catch (Exception)
                {
                    throw new LockAbortException();
                }
            }
        }

        /*
         * Remove lock on the specified block.
         */
        public void Unlock(Block block)
        {
            int value = GetLockValue(block);

            if(value > 0 )
            {
                _locks[block] = value - 1;
            } else
            {
                _locks.Remove(block);
                Monitor.PulseAll(this); // Notify all waiting threads
            }
        }

        /*
         * Check if there are other shared locks on the block.
         */
        private bool HasOtherSLocks(Block block)
        {
            return GetLockValue(block) > 1;
        }

        /*
         * Returns the lock value for the specified block, or 0 if no lock exists.
         */
        private int GetLockValue(Block block)
        {
            if (_locks.TryGetValue(block, out int value))
            {
                return value;
            }

            return 0;
        }

        /*
         * Check if waiting time exceeded MAX_TIME.
         */
        private bool WaitingTooLong(int startTime)
        {
            return DateTime.Now.Millisecond - startTime > MAX_TIME;
        }

        /*
         * Check if exclusive lock is acquired on the block.
         */
        private bool HasXLock(Block block)
        {
            return GetLockValue(block) < 0;
        }
    }
}
