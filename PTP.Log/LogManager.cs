using PTP.Disk;

namespace PTP.Log
{
    public class LogManager
    {
        private readonly FileManager _fileManager;
        private readonly string _logFileName;
        private readonly Page _logPage;
        private Block _currentBlock;
        private int latestLSN = 0;
        private int lastSavedLSN = 0;

        public FileManager FileManager => _fileManager;

        public string LogFileName => _logFileName;

        public Page Page => _logPage;

        public Block CurrentBlock => _currentBlock;

        public int LatestLSN => latestLSN;

        public int LastSavedLSN => lastSavedLSN;

        public LogManager(FileManager fileManager, string logFileName)
        {
            _fileManager = fileManager;
            _logFileName = logFileName;

            byte[] contents = new byte[_fileManager.BlockSize];

            _logPage = new Page(contents);

            int logSize = _fileManager.Length(logFileName);

            if (logSize == 0)
            {
                _currentBlock = AppendNewBlock();
            }
            else
            {
                _currentBlock = new Block(logFileName, logSize - 1);
                _fileManager.Read(_currentBlock, _logPage);
            }
        }

        public int Append(byte[] logrecord)
        {
            lock(this)
            {
                int boundary = _logPage.GetInt(0);
                int recordSize = logrecord.Length;
                int bytesNeeded = recordSize + sizeof(int);

                if(boundary - bytesNeeded < sizeof(int))
                {
                    Flush();
                    _currentBlock = AppendNewBlock();
                    boundary = _logPage.GetInt(0);
                }

                int recordPosition = boundary - bytesNeeded;
                _logPage.SetBytes(recordPosition, logrecord);
                _logPage.SetInt(0, recordPosition);

                latestLSN++;

                return latestLSN;
            }
        }

        public void Flush(int LSN)
        {
            if (LSN > lastSavedLSN)
                this.Flush();
        }

        public IEnumerator<byte[]> Enumerator()
        {
            Flush();

            return new LogEnumerator(_fileManager, _currentBlock);
        }

        private void Flush()
        {
            if (_currentBlock == null || _logPage == null)
                return;

            _fileManager.Write(_currentBlock, _logPage);
            lastSavedLSN = latestLSN;
        }

        private Block AppendNewBlock()
        {
            Block block = _fileManager.Append(_logFileName);
            _logPage.SetInt(0, _fileManager.BlockSize);
            
            _fileManager.Write(block, _logPage);

            return block;
        }
    }
}
