using PTP.Query;
using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.QueryPlan.Plans
{
    public class SelectPlan : IPlan
    {
        private IPlan _plan;
        private Predicate _pred;

        public SelectPlan(IPlan plan, Predicate pred)
        {
            _plan = plan;
            _pred = pred;
        }
        public int BlocksAccessed()
        {
            return _plan.BlocksAccessed();
        }

        public int DistinctValues(string fieldName)
        {
            if(_pred.EquatesWithConstant(fieldName) != null)
                return 1;
            else
            {
                string fieldName2 = _pred.EquatesWithField(fieldName);
                if (fieldName2 != null)
                    return Math.Min(_plan.DistinctValues(fieldName), _plan.DistinctValues(fieldName2));
                else
                    return _plan.DistinctValues(fieldName);
            }
        }

        public IScan Open()
        {
            IScan s = _plan.Open();
            return new SelectScan(s, _pred);
        }

        public int RecordsOutput()
        {
            return _plan.RecordsOutput() / _pred.ReductionFactor(_plan);
        }

        public Schema Schema()
        {
            return _plan.Schema();
        }
    }
}
