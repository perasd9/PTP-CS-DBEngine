using PTP.Disk;

namespace PTP.Log.Tests
{
    public class LogManagerTest : IDisposable
    {
        private LogManager _logManager;
        private string _testFolder = "\\PTP.Log.Tests\\testLog";

        public LogManagerTest()
        {
            if (Directory.Exists(_testFolder))
                Directory.Delete(_testFolder, recursive: true);

            Directory.CreateDirectory(_testFolder);

            FileManager fm = new FileManager(_testFolder, 400);
            _logManager = new LogManager(fm, "testLogFile");
        }

        [Fact]
        public void LogTestScenario()
        {
            CreateRecords(1, 35);
            PrintLogRecords("The log file now has these records:");

            CreateRecords(36, 70);
            _logManager.Flush(65);
            PrintLogRecords("The log file now has these records:");
        }

        private void CreateRecords(int start, int end)
        {
            Console.Write("Creating records: ");
            for (int i = start; i <= end; i++)
            {
                byte[] rec = CreateLogRecord("record" + i, i + 100);
                int lsn = _logManager.Append(rec);
                Console.Write(lsn + " ");
            }
            Console.WriteLine();
        }

        private byte[] CreateLogRecord(string data, int length)
        {
            int npos = Page.MaxLength(data.Length);
            byte[] b = new byte[npos + sizeof(int)];
            Page p = new Page(b);
            p.SetString(0, data);
            p.SetInt(npos, length);
            return b;
        }
        private void PrintLogRecords(string message)
        {
            Console.WriteLine(message);
            IEnumerator<byte[]> iter = _logManager.Enumerator();

            while (iter.MoveNext())
            {
                byte[] rec = iter.Current;
                Page p = new Page(rec);
                string s = p.GetString(0);
                int npos = Page.MaxLength(s.Length);
                int val = p.GetInt(npos);
                Console.WriteLine($"[{s}, {val}]");
            }

            Console.WriteLine();
        }

        public void Dispose()
        {
            if(Directory.Exists(_testFolder))
                Directory.Delete(_testFolder, recursive: true);
        }
    }
}
