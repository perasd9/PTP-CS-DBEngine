using PTP.Metadata;
using PTP.Parse.Data;
using PTP.Query;
using PTP.Query.Interfaces;
using PTP.QueryPlan.Planners.Interfaces;
using PTP.QueryPlan.Plans;

namespace PTP.QueryPlan.Planners
{
    public class BasicUpdatePlanner : IUpdatePlanner
    {
        private MetadataManager _metadataManager;

        public BasicUpdatePlanner(MetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public int ExecuteCreateIndex(CreateIndexData data, Transaction.Transaction transaction)
        {
            _metadataManager.IndexManager.CreateIndex(data.IndexName, data.TableName, data.FieldName, transaction);

            return 0;
        }

        public int ExecuteCreateTable(CreateTableData data, Transaction.Transaction transaction)
        {
            _metadataManager.TableManager.CreateTable(data.TableName, data.Schema, transaction);

            return 0;
        }

        public int ExecuteCreateView(CreateViewData data, Transaction.Transaction transaction)
        {
            _metadataManager.ViewManager.CreateView(data.ViewName, data.ViewDefinition(), transaction);

            return 0;
        }

        public int ExecuteDelete(DeleteData data, Transaction.Transaction transaction)
        {
            IPlan plan = new TablePlan(transaction, data.TableName, _metadataManager);
            plan = new SelectPlan(plan, data.Pred);

            IUpdateScan scan = (IUpdateScan)plan.Open();

            int count = 0;

            while(scan.Next())
            {
                scan.Delete();
                count++;
            }

            scan.Close();

            return count;
        }

        public int ExecuteInsert(InsertData data, Transaction.Transaction transaction)
        {
            IPlan plan = new TablePlan(transaction, data.TableName, _metadataManager);
            IUpdateScan scan = (IUpdateScan)plan.Open();

            scan.Insert();

            IEnumerator<Constant> vals = data.Values.GetEnumerator();

            foreach (var field in data.Fields)
            {
                vals.MoveNext();
                scan.SetValue(field, vals.Current);
            }

            scan.Close();

            return 1;
        }

        public int ExecuteModify(ModifyData data, Transaction.Transaction transaction)
        {
            IPlan plan = new TablePlan(transaction, data.TableName, _metadataManager);
            plan = new SelectPlan(plan, data.Pred);

            IUpdateScan scan = (IUpdateScan)plan.Open();

            int count = 0;

            while (scan.Next())
            {
                Constant val = data.NewValue.Evaluate(scan);
                scan.SetValue(data.FieldName, val);
                count++;
            }

            scan.Close();

            return count;
        }
    }
}
