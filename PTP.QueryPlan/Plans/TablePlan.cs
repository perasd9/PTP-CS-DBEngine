using PTP.Metadata;
using PTP.Query;
using PTP.Query.Interfaces;
using PTP.Shared;
using PTP.Transaction;

namespace PTP.QueryPlan.Plans
{
    public class TablePlan : IPlan
    {
        private Transaction.Transaction _transaction;
        private string _tableName;
        private Layout _layout;
        private StatisticInfo _statisticInfo;

        public TablePlan(Transaction.Transaction transaction, string tableName, MetadataManager manager)
        {
            _transaction = transaction;
            _tableName = tableName;
            _layout = manager.TableManager.GetLayout(tableName, transaction);
            _statisticInfo = manager.StatisticManager.GetStatisticInfo(tableName, _layout, transaction);
        }

        public int BlocksAccessed()
        {
            return _statisticInfo.BlocksAccessed;
        }

        public int DistinctValues(string fieldName)
        {
            return _statisticInfo.DistinctValues(fieldName);
        }

        public IScan Open()
        {
            return new TableScan(_transaction, _layout, _tableName);
        }

        public int RecordsOutput()
        {
            return _statisticInfo.RecordsOutput;
        }

        public Schema Schema()
        {
            return _layout.Schema;
        }
    }
}
