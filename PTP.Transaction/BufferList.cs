using PTP.Buffer;
using PTP.Disk;

namespace PTP.Transaction
{
    internal class BufferList
    {
        private BufferManager _bufferManager;

        public BufferList(BufferManager bufferManager)
        {
            this._bufferManager = bufferManager;
        }

        internal Buffer.Buffer GetBuffer(Block block)
        {
            throw new NotImplementedException();
        }

        internal void Pin(Block block)
        {
            throw new NotImplementedException();
        }

        internal void Unpin(Block block)
        {
            throw new NotImplementedException();
        }

        internal void UnpinAll()
        {
            throw new NotImplementedException();
        }
    }
}
