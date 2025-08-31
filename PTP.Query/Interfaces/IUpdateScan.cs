using PTP.Shared;

namespace PTP.Query.Interfaces
{
    public interface IUpdateScan : IScan
    {
        public void SetInt(string fieldName, int value);
        public void SetString(string fieldName, string value);
        public void SetValue(string fieldName, Constant value);
        public void Insert();
        public void Delete();
        public RID GetRID();
        public void MoveToRID(RID rid);
    }
}
