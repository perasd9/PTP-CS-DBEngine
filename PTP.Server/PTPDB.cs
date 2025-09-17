using PTP.Buffer;
using PTP.Disk;
using PTP.Log;
using PTP.Metadata;
using PTP.QueryPlan;
using PTP.QueryPlan.Planners;
using PTP.QueryPlan.Planners.Interfaces;
using PTP.Transaction;
using System.Numerics;

namespace PTP.Server
{
    public class PTPDB
    {
        public static int BLOCK_SIZE = 400;
        public static int BUFFER_SIZE = 8;
        public static string LOG_FILE = "ptpdb.log";

        private FileManager fm;
        private BufferManager bm;
        private LogManager lm;
        private MetadataManager mdm;
        private MainPlanner mainPlanner;

        public PTPDB(string dirname, int blocksize, int buffsize)
        {
            fm = new FileManager(dirname, blocksize);
            lm = new LogManager(fm, LOG_FILE);
            bm = new BufferManager(fm, lm, buffsize);
        }

        private void PrintLogRecords(string message)
        {
            Console.WriteLine(message);
            IEnumerator<byte[]> iter = lm.Enumerator();

            while (iter.MoveNext())
            {
                byte[] rec = iter.Current;
                Page p = new Page(rec);
                string data = p.GetString(0);
                int npos = Page.MaxLength(data.Length);
                int val = p.GetInt(npos);
                Console.WriteLine($"[{data}, {val}]");
            }

            Console.WriteLine();
        }

        public PTPDB(string dirname) : this(dirname, BLOCK_SIZE, BUFFER_SIZE)
        {
            Transaction.Transaction tx = NewTx();
            bool isnew = fm.IsNew;
            if (isnew)
                Console.WriteLine("creating new database");
            else
            {
                Console.WriteLine("recovering existing database");
                tx.Recover();
            }

            mdm = new MetadataManager(isnew, tx);
            IQueryPlanner qp = new BasicQueryPlanner(mdm);
            //UpdatePlanner up = new BasicUpdatePlanner(mdm);
            //    QueryPlanner qp = new HeuristicQueryPlanner(mdm);
            //    UpdatePlanner up = new IndexUpdatePlanner(mdm);
            this.mainPlanner = new MainPlanner(qp);

            tx.Commit();
        }

        public Transaction.Transaction NewTx()
        {
            return new Transaction.Transaction(fm, bm, lm);
        }

        public MetadataManager mdMgr()
        {
            return mdm;
        }

        public MainPlanner planner()
        {
            return mainPlanner;
        }

        public FileManager fileMgr()
        {
            return fm;
        }
        public LogManager logMgr()
        {
            return lm;
        }
        public BufferManager bufferMgr()
        {
            return bm;
        }
    }
}
