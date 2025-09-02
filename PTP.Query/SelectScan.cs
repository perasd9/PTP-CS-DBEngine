using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.Query
{
    public class SelectScan : IUpdateScan
    {
        private IScan _s;
        private Predicate _pred;

        public SelectScan(IScan s, Predicate pred)
        {
            _s = s;
            _pred = pred;
        }

        public void BeforeFirst()
        {
            _s.BeforeFirst();
        }

        public void Close()
        {
            _s.Close();
        }


        public int GetInt(string fieldName)
        {
            return _s.GetInt(fieldName);
        }

        public string GetString(string fieldName)
        {
            return _s.GetString(fieldName);
        }

        public Constant GetValue(string fieldName)
        {
            return _s.GetValue(fieldName);
        }

        public bool HasField(string fieldName)
        {
            return _s.HasField(fieldName);
        }


        public bool Next()
        {
            while(_s.Next())
            {
                if (_pred.IsSatisfied(_s))
                    return true;
            }
            return false;
        }

        //Update methods

        public void Insert()
        {
            IUpdateScan updateScan = (IUpdateScan)_s;

            updateScan.Insert();
        }

        public void Delete()
        {
            IUpdateScan updateScan = (IUpdateScan)_s;

            updateScan.Delete();
        }

        public void MoveToRID(RID rid)
        {
            IUpdateScan updateScan = (IUpdateScan)_s;

            updateScan.MoveToRID(rid);
        }

        public RID GetRID()
        {
            IUpdateScan updateScan = (IUpdateScan)_s;

            return updateScan.GetRID();
        }

        public void SetInt(string fieldName, int value)
        {
            IUpdateScan updateScan = (IUpdateScan)_s;

            updateScan.SetInt(fieldName, value);
        }

        public void SetString(string fieldName, string value)
        {
            IUpdateScan updateScan = (IUpdateScan)_s;

            updateScan.SetString(fieldName, value);
        }

        public void SetValue(string fieldName, Constant value)
        {
            IUpdateScan updateScan = (IUpdateScan)_s;

            updateScan.SetValue(fieldName, value);
        }
    }
}
