using PTP.Query;
using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.QueryPlan.Plans
{
    public class ProjectPlan : IPlan
    {
        private IPlan _plan;
        private Schema _schema;
        public ProjectPlan(IPlan plan, ICollection<string> fieldNames)
        {
            _plan = plan;
            _schema = new Schema();
            foreach(var fldname in fieldNames)
            {
                _schema.Add(fldname, _plan.Schema());
            }
        }
        public int BlocksAccessed()
        {
            return _plan.BlocksAccessed();
        }

        public int DistinctValues(string fieldName)
        {
            return _plan.DistinctValues(fieldName);
        }

        public IScan Open()
        {
            IScan s = _plan.Open();
            return new ProjectScan(s, _schema.Fields());
        }

        public int RecordsOutput()
        {
            return _plan.RecordsOutput();
        }

        public Schema Schema()
        {
            return _schema;
        }
    }
}
