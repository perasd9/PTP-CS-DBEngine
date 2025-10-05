using PTP.Query;
using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.QueryPlan.Plans
{
    public class ProductPlan : IPlan
    {
        private IPlan _plan1, _plan2;
        private Schema _schema = new Schema();

        public ProductPlan(IPlan plan1, IPlan plan2)
        {
            _plan1 = plan1;
            _plan2 = plan2;
            _schema.AddAll(_plan1.Schema());
            _schema.AddAll(_plan2.Schema());
        }

        public int BlocksAccessed()
        {
            return _plan1.BlocksAccessed() + (_plan1.RecordsOutput() * _plan2.BlocksAccessed());
        }

        public int DistinctValues(string fieldName)
        {
            if (_plan1.Schema().HasField(fieldName))
                return _plan1.DistinctValues(fieldName);
            else
                return _plan2.DistinctValues(fieldName);
        }

        public IScan Open()
        {
            IScan s1 = _plan1.Open();
            IScan s2 = _plan2.Open();
            return new ProductScan(s1, s2);
        }

        public int RecordsOutput()
        {
            return _plan1.RecordsOutput() * _plan2.RecordsOutput();
        }

        public Schema Schema()
        {
            return _schema;
        }
    }
}
