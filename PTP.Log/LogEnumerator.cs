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

        /*
         * Block in memory used by the LogManager, represents file which working with this block.
         * Initialize {_page} as block in memory and important one is that {_page} is EMPTY(just byte array with defaul value).
         * MoveToBlock will not literally move anything to block but actually read block from file and write into page.
         */
        public LogEnumerator(FileManager fileManager, Block block)
        {
            _fileManager = fileManager;
            _block = block;

            _page = new Page(new byte[_fileManager.BlockSize]);
            MoveToBlock(_block);
        }

        /*
         * Active item in collection.
         */
        object IEnumerator.Current => Current;
        public byte[] Current => _current;


        public void Dispose()
        {
            //nothing to clean up
        }

        /*
         * If {_currentPosition}(which is 0 by deault at the first calling constructor) is equals with the block size(which means that current is at the end of block)
         * and if block number is 0(this is important because 0 is actually last block, see log manager where we described that log enumerator starts from the end)
         * that means that there are no more blocks and return false, if block number is not 0 it can be continued with new block(one after currently which is currently - 1)
         * and MoveToBlock to actually load that block from disk into memory(Page), in both case if {_currentPosition} was equals to {_fileManager.BlockSize} or not
         * it's going on read next data with GetBytes method, that's {_current}, {_currentPosition} is also updated with new offset and it returns true(go to next item)
         */
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

        /*
         * This method will read block from disk and load that into {_page} variable.
         * {_boundary} will just be read as first 4B(int), that's boundary of 
         */
        private void MoveToBlock(Block block)
        {
            _fileManager.Read(block, _page);
            _boundary = _page.GetInt(0);
            _currentPosition = _boundary;
        }
    }
}
