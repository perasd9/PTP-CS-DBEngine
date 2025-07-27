using PTP.Disk;
using System.Collections;
using System.Reflection.Metadata.Ecma335;

namespace PTP.Log
{
    public class LogEnumerator : IEnumerator<byte[]>
    {
        private readonly FileManager _fileManager;
        private Block _block;
        private readonly Page _page;
        private int _currentPosition;
        private int _boundary;
        private byte[] _current;

        public LogEnumerator(FileManager fileManager, Block block)
        {
            _fileManager = fileManager;
            _block = block;

            _page = new Page(new byte[_fileManager.BlockSize]);
            MoveToBlock(_block);
        }

        object IEnumerator.Current => Current;
        public byte[] Current => _current;


        public void Dispose()
        {
            //nothing to clean up
        }

        public bool MoveNext()
        {
            if(_currentPosition == _fileManager.BlockSize)
            {
                if (_block.BlockNumber == 0)
                    return false;

                _block = new Block(_block.FileName, _block.BlockNumber - 1);

                MoveToBlock(_block);
            }

            _current = _page.GetBytes(_currentPosition);
            _currentPosition += sizeof(int) + _current.Length;

            return true;
        }

        public void Reset()
        {
            //we go in reverse order, so we have nothing to reset
            throw new NotSupportedException();
        }
        private void MoveToBlock(Block block)
        {
            _fileManager.Read(block, _page);
            _boundary = _page.GetInt(0);
            _currentPosition = _boundary;
        }
    }
}
