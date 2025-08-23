using PTP.Disk;
using PTP.Log;
using PTP.Shared.Exceptions;
using System.Runtime.ConstrainedExecution;

namespace PTP.Buffer
{
    public class BufferManager
    {
        /*
         * Buffer pool is pool of pages which are cached as pool, number of available denotes if there is some page which is free to be used.
         * Time is used to prevent deadlock potentially, some clients code can pin some page which is pinned by someone else who waits for same page
         * and if MAX_TIME expires exception will be thrown.
         */
        private List<Buffer> _bufferPool;
        private int _numAvailable;
        private static long MAX_TIME = 10_000; //time in milliseconds

        public BufferManager(FileManager fileManager, LogManager logManager, int numBuffs)
        {
            this._bufferPool = new List<Buffer>(numBuffs);
            this._numAvailable = numBuffs;

            for (int i = 0; i < numBuffs; i++)
                this._bufferPool[i] = new Buffer(fileManager, logManager);
        }

        /*
         * This method just returns how much pages are free to be used.
         */
        public int AvailableBuffers()
        {
            lock (this)
            {
                return _numAvailable;
            }
        }

        /*
         * This method iterates through all buffers in pool and check if buffer is dirty(see buffer's SetModified method to understand what is dirty page).
         * If page is dirty its content is flushing into disk.
         */
        public void FlushAll(int transcationNumber)
        {
            lock(this)
            {
                foreach (var buffer in this._bufferPool)
                    if(buffer.ModifyingTransactionNumber == transcationNumber)
                        buffer.Flush();
            }
        }

        /*
         * This method unpins buffer in pool which means that makes free space for some client in waiting state to use this buffer now.
         */
        public void Unpin(Buffer buffer)
        {
            lock(this)
            {
                buffer.Unpin();

                if (!buffer.IsPinned)
                {
                    this._numAvailable++;
                    Monitor.PulseAll(this);
                }
            }
        }

        /*
         * This method evident current time, call local method TryToPin on block, if buffer is not pinned loop is executing, wait MAX_TIME in miliseconds,
         * if some buffer is freed another thread will notify that released buffer(unpin actually) which will trigger buffer here to try to pin again.
         * If time expires and buffer cannot be pinned exception will be thrown, opposite buffer will be returned.
         */
        public Buffer Pin(Block block)
        {
            try
            {
                long currentTimestamp = DateTime.Now.Millisecond;

                Buffer buff = this.TryToPin(block);

                while (buff is null && !IsWaitedTooLong(currentTimestamp))
                {
                    Monitor.Wait(this, (int)MAX_TIME);

                    buff = this.TryToPin(block);
                }

                if (buff is null)
                    throw new BufferAbortException();

                return buff;
            }
            catch (Exception)
            {
                throw new BufferAbortException();
            }
        }

        /*
         * This method tries to pin block in buffer, firstly tries to find forwarded lock into buffer pool, if it is not successfull tries to find
         * in unpinned buffers if and that's not successfull returns null. Opposite load block into page calling AssignToBlock method and in both paths
         * if buffer exists in existing buffer or it didn't exist buffer is pinning calling Pin method on {buff}.
         */
        private Buffer TryToPin(Block block)
        {
            Buffer buff = this.FindExistingBuffer(block);

            if(buff is null)
            {
                buff = this.FindUnpinnedBuffer();

                if(buff is null)
                    return null;

                buff.AssignToBlock(block);
            }

            if (!buff.IsPinned)
                this._numAvailable--;

            buff.Pin();

            return buff;
        }

        /*
         * This method just returns whether did time for waiting expired.
         */
        private bool IsWaitedTooLong(long startTime) => DateTime.Now.Millisecond - startTime > MAX_TIME;

        /*
         * This method iterates through buffers in pool and find first one which is not pinned(not in usage of some page).
         */
        private Buffer FindUnpinnedBuffer()
        {
            foreach (var buffer in this._bufferPool)
                if (!buffer.IsPinned)
                    return buffer;

            return null;
        }

        /*
         * This method iterates through buffers in pool and check if buffer is in cache currently, where it's pinned or not.
         */
        private Buffer FindExistingBuffer(Block block)
        {
            foreach (var buffer in this._bufferPool)
            {
                Block b = buffer.Block;

                if (b != null && b.Equals(block))
                    return buffer;
            }

            return null;
        }
    }
}
