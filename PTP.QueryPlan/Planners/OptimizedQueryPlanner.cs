using PTP.Metadata;
using PTP.Parse;
using PTP.Parse.Data;
using PTP.Query;
using PTP.QueryPlan.Planners.Interfaces;
using PTP.QueryPlan.Plans;

namespace PTP.QueryPlan.Planners
{
    public class OptimizedQueryPlanner : IQueryPlanner
    {
        private MetadataManager _metadataManager;

        public OptimizedQueryPlanner(MetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IPlan CreatePlan(QueryData queryData, Transaction.Transaction transaction)
        {
            List<IPlan> plans = new List<IPlan>();

            foreach (var tbl in queryData.Tables)
            {
                string viewDefinition = _metadataManager.ViewManager.GetViewDefinition(tbl, transaction);
                if (viewDefinition != null)
                {
                    var parser = new Parser(viewDefinition);
                    QueryData viewData = parser.Query();

                    plans.Add(this.CreatePlan(viewData, transaction));
                }
                else
                {
                    plans.Add(new TablePlan(transaction, tbl, _metadataManager));
                }
            }

            IPlan currentPlan = plans[0];
            plans.Remove(currentPlan);

            foreach (var nextPlan in plans)
            {
                IPlan plan1 = new ProductPlan(nextPlan, currentPlan);
                IPlan plan2 = new ProductPlan(currentPlan, nextPlan);

                currentPlan = (plan1.BlocksAccessed() < plan2.BlocksAccessed()) ? plan1 : plan2;
            }

            currentPlan = new SelectPlan(currentPlan, queryData.Pred);

            return new ProjectPlan(currentPlan, queryData.Fields);
        }
    }
}
