using PTP.Metadata;
using PTP.Parse;
using PTP.Parse.Data;
using PTP.Query;
using PTP.QueryPlan.Planners.Interfaces;
using PTP.QueryPlan.Plans;

namespace PTP.QueryPlan.Planners
{
    public class BasicQueryPlanner : IQueryPlanner
    {
        private MetadataManager _metadataManager;

        public BasicQueryPlanner(MetadataManager metadataManager)
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
                currentPlan = new ProductPlan(currentPlan, nextPlan);
            
            currentPlan = new SelectPlan(currentPlan, queryData.Pred);

            return new ProjectPlan(currentPlan, queryData.Fields);
        }
    }
}
