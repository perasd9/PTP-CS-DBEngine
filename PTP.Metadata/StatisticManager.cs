using PTP.Shared;

namespace PTP.Metadata
{
    public class StatisticManager
    {
        private TableManager _tableManager;
        private IDictionary<string, StatisticInfo> _stats;
        private int _numberOfCalls;

        public StatisticManager(TableManager tableManager, Transaction.Transaction transaction)
        {
            _tableManager = tableManager;
            this.RefreshStatistics(transaction);
        }

        public StatisticInfo GetStatisticInfo(string tableName, Layout layout, Transaction.Transaction transaction)
        {
            lock (this)
            {
                _numberOfCalls++;

                if (_numberOfCalls > 100)
                    this.RefreshStatistics(transaction);

                var exist = _stats.TryGetValue(tableName, out var info);

                if (!exist)
                {
                    info = this.CalculateTableStatistics(tableName, layout, transaction);
                    _stats.Add(tableName, info);
                }

                return info!; 
            }
        }

        private void RefreshStatistics(Transaction.Transaction transaction)
        {
            lock(this)
            {
                _stats = new Dictionary<string, StatisticInfo>();
                _numberOfCalls = 0;

                Layout tableCatalogLayout = _tableManager.GetLayout("tblcat", transaction);
                TableScan tableCatalog = new TableScan(transaction, tableCatalogLayout, "tblcat");

                while(tableCatalog.Next())
                {
                    string tableName = tableCatalog.GetString("tblname");
                    Layout layout = _tableManager.GetLayout(tableName, transaction);

                    StatisticInfo info = this.CalculateTableStatistics(tableName, layout, transaction);

                    _stats.Add(tableName, info);
                }

                tableCatalog.Close();
            }
        }

        private StatisticInfo CalculateTableStatistics(string tableName, Layout layout, Transaction.Transaction transaction)
        {
            int numberOfRecords = 0;
            int numberOfBlocks = 0;

            TableScan tableScan = new TableScan(transaction, layout, tableName);

            while(tableScan.Next())
            {
                numberOfRecords++;
                numberOfBlocks = tableScan.GetRID().BlockNumber + 1;
            }

            tableScan.Close();

            return new StatisticInfo(numberOfBlocks, numberOfRecords);
        }
    }

    public class StatisticInfo
    {
        private int _numberOfBlocks;
        private int _numberOfRecords;

        public StatisticInfo(int numberOfBlocks, int numberOfRecords)
        {
            _numberOfBlocks = numberOfBlocks;
            _numberOfRecords = numberOfRecords;
        }

        public int BlocksAccessed => _numberOfBlocks;
        public int RecordsOutput => _numberOfRecords;

        public int DistinctValues(string fieldName)
        {
            // Temporary implementation; real implementation would require more metadata
            return 1 + (_numberOfRecords / 3);
        }
    }
}
