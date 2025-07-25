using PTP.Disk;
using System.Collections;

namespace PTP.Log
{
    public class LogIterator : IEnumerator<byte[]>
    {
        private readonly FileManager _fileManager;
        private readonly Block _block;
        private readonly Page _page;
        private readonly int _currentPosition;
        private readonly int boundary;

        public byte[] Current => throw new NotImplementedException();

        public LogIterator(FileManager fileManager, Block block)
        {
            _fileManager = fileManager;
            _block = block;

        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
