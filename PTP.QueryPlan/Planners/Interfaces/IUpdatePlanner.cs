using PTP.Parse.Data;

namespace PTP.QueryPlan.Planners.Interfaces
{
    public interface IUpdatePlanner
    {
        public int ExecuteInsert(InsertData data, Transaction.Transaction tx);
        public int ExecuteDelete(DeleteData data, Transaction.Transaction tx);
        public int ExecuteModify(ModifyData data, Transaction.Transaction tx);
        public int ExecuteCreateTable(CreateTableData data, Transaction.Transaction tx);
        public int ExecuteCreateView(CreateViewData data, Transaction.Transaction tx);
        public int ExecuteCreateIndex(CreateIndexData data, Transaction.Transaction tx);

    }
}
