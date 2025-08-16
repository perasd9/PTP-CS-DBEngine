using PTP.Disk;
using PTP.Shared.Exceptions;

namespace PTP.Transaction.Concurrency
{
    public class LockTable
    {
        private const int MAX_TIME = 10_000; // time in milliseconds

        private readonly Dictionary<Block, int> _locks = new();

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

        private bool HasOtherSLocks(Block block)
        {
            return GetLockValue(block) > 1;
        }

        private int GetLockValue(Block block)
        {
            if (_locks.TryGetValue(block, out int value))
            {
                return value;
            }

            return 0;
        }

        private bool WaitingTooLong(int startTime)
        {
            return DateTime.Now.Millisecond - startTime > MAX_TIME;
        }

        private bool HasXLock(Block block)
        {
            return GetLockValue(block) < 0;
        }
    }
}
