using PTP.IndexData;
using PTP.Shared;

namespace PTP.Metadata
{
    public class IndexManager
    {
        private Layout _layout;
        private TableManager _tableManager;
        private StatisticManager _statisticManager;

        public IndexManager(bool isNew, TableManager tableManager, StatisticManager statisticManager, Transaction.Transaction transaction)
        {
            if(isNew)
            {
                Schema schema = new Schema();
                schema.AddStringField("indexName", TableManager.MAX_LENGTH_NAME);
                schema.AddStringField("tableName", TableManager.MAX_LENGTH_NAME);
                schema.AddStringField("fieldName", TableManager.MAX_LENGTH_NAME);

                tableManager.CreateTable("idxcat", schema, transaction);
            }

            _tableManager = tableManager;
            _statisticManager = statisticManager;

            _layout = tableManager.GetLayout("idxcat", transaction);
        }

        public void CreateIndex(string indexName, string tableName, string fieldName, Transaction.Transaction transaction)
        {
            TableScan tableScan = new TableScan(transaction, _layout, "idxcat");

            tableScan.Insert();
            tableScan.SetString("indexname", indexName);
            tableScan.SetString("tablename", tableName);
            tableScan.SetString("fieldname", fieldName);

            tableScan.Close();
        }

 //       public Map<String, IndexInfo> getIndexInfo(String tblname,
 //Transaction tx)
 //       {
 //           Map<String, IndexInfo> result = new HashMap<String, IndexInfo>();
 //           TableScan ts = new TableScan(tx, "idxcat", layout);
 //           while (ts.next())
 //               if (ts.getString("tablename").equals(tblname))
 //               {
 //                   String idxname = ts.getString("indexname");
 //                   String fldname = ts.getString("fieldname");
 //                   Layout tblLayout = tblmgr.getLayout(tblname, tx);
 //                   StatInfo tblsi = statmgr.getStatInfo(tblname, tbllayout, tx);
 //                   IndexInfo ii = new IndexInfo(idxname, fldname,
 //                   tblLayout.schema(), tx, tblsi);
 //                   result.put(fldname, ii);
 //               }
 //           ts.close();
 //           return result;
 //       }
    }

    public class IndexInfo
    {
        private string _indexName, _fieldName;
        private Transaction.Transaction _transaction;
        private Schema _tableSchema;
        private Layout _indexLayout;
        private StatisticInfo _statisticInfo;

        public IndexInfo(string indexName, string fieldName, Schema tableSchema, Transaction.Transaction transaction, StatisticInfo statisticInfo)
        {
            _indexName = indexName;
            _fieldName = fieldName;
            _tableSchema = tableSchema;
            _indexLayout = this.CreateIndexLayout();
            _transaction = transaction;
            _statisticInfo = statisticInfo;
        }

        public PTPIndex Open()
        {
            Schema schema = new Schema();

            //return new HashIndex(_transaction, _indexName, _indexLayout);
            //return new BTreeIndex(_transaction, _indexName, _indexLayout);

            return null;
        }

        public int BlocksAccessed()
        {
            int requestsPerBlock = _transaction.BlockSize / _indexLayout.SlotSize;
            int numberOfBlocks = _statisticInfo.RecordsOutput / requestsPerBlock;

            //return HashIndex.SearchCost(numberOfBlocks, requestsPerBlock);

            return 0;
        }

        public int RecordsOutput()
        {
            return _statisticInfo.RecordsOutput / _statisticInfo.DistinctValues(_fieldName);
        }

        public int DistinctValues(string fieldName)
        {
            return _fieldName == fieldName ? 1 : _statisticInfo.DistinctValues(fieldName);
        }

        private Layout CreateIndexLayout()
        {
            Schema schema = new Schema();

            schema.AddIntField("block");
            schema.AddIntField("id");

            if(_tableSchema.Type(_fieldName) == (int)FieldType.INT)
            {
                schema.AddIntField("dataval");
            }
            else
            {
                int length = _tableSchema.Length(_fieldName);
                schema.AddStringField("dataval", length);
            }

            return new Layout(schema);
        }
    }
}
