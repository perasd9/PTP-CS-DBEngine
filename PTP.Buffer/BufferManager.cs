using PTP.Disk;
using PTP.Log;
using PTP.Shared.Exceptions;
using System.Runtime.ConstrainedExecution;

namespace PTP.Buffer
{
    public class BufferManager
    {

        private List<Buffer> _bufferPool;
        private int _numAvailable;
        private static long MAX_TIME = 10_000;

        public BufferManager(FileManager fileManager, LogManager logManager, int numBuffs)
        {
            this._bufferPool = new List<Buffer>(numBuffs);
            this._numAvailable = numBuffs;

            for (int i = 0; i < numBuffs; i++)
                this._bufferPool[i] = new Buffer(fileManager, logManager);
        }

        public int AvailableBuffers()
        {
            lock (this)
            {
                return _numAvailable;
            }
        }

        public void FlushAll(int transcationNumber)
        {
            lock(this)
            {
                foreach (var buffer in this._bufferPool)
                    if(buffer.ModifyingTransactionNumber == transcationNumber)
                        buffer.Flush();
            }
        }

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

        private bool IsWaitedTooLong(long startTime) => DateTime.Now.Millisecond - startTime > MAX_TIME;

        private Buffer FindUnpinnedBuffer()
        {
            foreach (var buffer in this._bufferPool)
                if (!buffer.IsPinned)
                    return buffer;

            return null;
        }

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
