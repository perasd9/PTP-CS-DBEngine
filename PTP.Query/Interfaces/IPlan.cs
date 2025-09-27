using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.Query
{
    public interface IPlan
    {
        public IScan Open();
        public int BlocksAccessed();
        public int RecordsOutput();
        public int DistinctValues(string fieldName);
        public Schema Schema();
    }
}
