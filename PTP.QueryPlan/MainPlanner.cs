using PTP.Parse;
using PTP.Parse.Data;
using PTP.Query;
using PTP.QueryPlan.Planners.Interfaces;

namespace PTP.QueryPlan
{
    public class MainPlanner
    {
        private IQueryPlanner _queryPlanner;
        private IUpdatePlanner _updatePlanner;

        public MainPlanner(IQueryPlanner queryPlanner, IUpdatePlanner updatePlanner)
        {
            _queryPlanner = queryPlanner;
            _updatePlanner = updatePlanner;
        }


        public IPlan CreatePlan(string qry, Transaction.Transaction transaction)
        {
            Parser parser = new Parser(qry);
            QueryData data = parser.Query();

            //this.VerifyQuery(data);

            return _queryPlanner.CreatePlan(data, transaction);
        }

        public int ExecuteUpdate(string cmd, Transaction.Transaction transaction)
        {
            Parser parser = new Parser(cmd);
            object data = parser.UpdateCommand();
            //this.VerifyUpdate(data);

            if (data is InsertData insertData)
                return _updatePlanner.ExecuteInsert(insertData, transaction);
            else if (data is DeleteData deleteData)
                return _updatePlanner.ExecuteDelete(deleteData, transaction);
            else if (data is ModifyData modifyData)
                return _updatePlanner.ExecuteModify(modifyData, transaction);
            else if (data is CreateTableData createTableData)
                return _updatePlanner.ExecuteCreateTable(createTableData, transaction);
            else if (data is CreateViewData createViewData)
                return _updatePlanner.ExecuteCreateView(createViewData, transaction);
            else if (data is CreateIndexData createIndexData)
                return _updatePlanner.ExecuteCreateIndex(createIndexData, transaction);
            else
                return 0;
        }
    }
}
