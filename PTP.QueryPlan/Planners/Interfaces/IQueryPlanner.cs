using PTP.Parse.Data;
using PTP.Query;

namespace PTP.QueryPlan.Planners.Interfaces
{
    public interface IQueryPlanner
    {
        public IPlan CreatePlan(QueryData queryData, Transaction.Transaction transaction);
    }
}
