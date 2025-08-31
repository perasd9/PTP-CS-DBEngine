using PTP.Buffer;
using PTP.Disk;

namespace PTP.Transaction
{
    public class BufferList
    {
        private Dictionary<Block, Buffer.Buffer> _bufferPool = new Dictionary<Block, Buffer.Buffer>();
        private ICollection<Block> _pinnedBlocks = new List<Block>();
        private BufferManager _bufferManager;

        public BufferList(BufferManager bufferManager)
        {
            this._bufferManager = bufferManager;
        }

        public Buffer.Buffer GetBuffer(Block block)
        {
            return _bufferPool.GetValueOrDefault(block);
        }

        public void Pin(Block block)
        {
            Buffer.Buffer buffer = _bufferManager.Pin(block);

            _bufferPool[block] = buffer;

            _pinnedBlocks.Add(block);
        }

        public void Unpin(Block block)
        {
            var doesExist = _bufferPool.TryGetValue(block, out var buffer);

            _bufferManager.Unpin(buffer);
            _pinnedBlocks.Remove(block);

            if(!_pinnedBlocks.Contains(block))
                _bufferPool.Remove(block);
        }

        public void UnpinAll()
        {
            foreach(var block in _pinnedBlocks.ToList())
            {
                Buffer.Buffer buffer = _bufferPool[block];
                _bufferManager.Unpin(buffer);
            }

            _bufferPool.Clear();
            _pinnedBlocks.Clear();
        }
    }
}
