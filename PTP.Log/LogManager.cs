using PTP.Disk;

namespace PTP.Log
{
    public class LogManager
    {
        private readonly FileManager _fileManager;
        private readonly string _logFileName;
        private readonly Page _logPage;
        private Block _currentBlock;
        private int latestLSN = 0; //latest in memory
        private int lastSavedLSN = 0; //last saved actually written in disk

        public FileManager FileManager => _fileManager;

        public string LogFileName => _logFileName;

        public Page Page => _logPage;

        public Block CurrentBlock => _currentBlock;

        public int LatestLSN => latestLSN;

        public int LastSavedLSN => lastSavedLSN;

        /*
         * Tell log manager which fileManager to use to disk access also specify logFile where want data to be stored.
         * {_logPage} will be initialized with empty byte array(default) and if logFile is empty({logSize} == 0) empty block will be added.
         * If it's not empty {_currentBlock} will be assigned with last block of log(here {logSize} helps, this is size in blocks).
         * Endly into active {_logPage} will be loaded {_currentBlock}
         */
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

        /*
         * The most important method of this class firsly get the {boundary} as first 4B(int) in page, {bytesNedded} is sum of record length and
         * size of int for this first 4B, if ex. boundary 350MB - bytesNeeded 300MB is greater than sizeof(int) which is same as zero just that 4B
         * are used for length value, if greater is case you have enough space in {_logPage} for record and you get {recordPosition}, set bytes
         * for that and update(really important) {boundary} the way you set {recordPosition} on first 4B, also important, increment latestLSN.
         * If case of less than sizeof(int) Flush call is mandatory and you have to make new page as {_currentBlock}.
         */
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

        /*
         * If Log Sequence Number(LSN) is greater than {lastSavedLSN} which is number of log lastly written into block then
         * {_logPage} will be written into block to guarantee eveything is on the disk(WAL - write ahead logging)
         */
        public void Flush(int LSN)
        {
            if (LSN > lastSavedLSN)
                this.Flush();
        }

        /*
         * Returns first block of log to make possible iterate through all blocks in log file but before that Flush method is called
         * to ensure everything is on the disk before iterating.
         */
        public IEnumerator<byte[]> Enumerator()
        {
            Flush();

            return new LogEnumerator(_fileManager, _currentBlock);
        }

        /*
         * Helper method to actually execute writing of {_logPage} into {_currentBlock}
         * Important is that when flush is done {lastSavedLSN} has to be updated with {latestLSN}
         */
        private void Flush()
        {
            if (_currentBlock == null || _logPage == null)
                return;

            _fileManager.Write(_currentBlock, _logPage);
            lastSavedLSN = latestLSN;
        }

        /*
         * Append new empty block to prepare for writing, {_logPage} will write first 4B(int) for block size and return that block. 
         */
        private Block AppendNewBlock()
        {
            Block block = _fileManager.Append(_logFileName);
            _logPage.SetInt(0, _fileManager.BlockSize);
            
            _fileManager.Write(block, _logPage);

            return block;
        }
    }
}
