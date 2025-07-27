namespace PTP.Log.Tests
{
    internal class LogManagerTest : IDisposable
    {
        private LogManager _logManager;
        private string _testFile;

        public LogManagerTest()
        {
            
        }

        public void Dispose()
        {
            if(Directory.Exists(_testFile))
                Directory.Delete(_testFile, recursive: true);
        }
    }
}
