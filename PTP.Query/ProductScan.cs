using PTP.Query.Interfaces;

namespace PTP.Query
{
    public class ProductScan : IScan
    {
        private IScan _s1, _s2;

        public ProductScan(IScan s1, IScan s2)
        {
            _s1 = s1;
            _s2 = s2;

            this.BeforeFirst();
        }

        public void BeforeFirst()
        {
            _s1.BeforeFirst();
            _s1.Next();
            _s2.BeforeFirst();
        }

        public void Close()
        {
            _s1.Close();
            _s2.Close();
        }

        public int GetInt(string fieldName)
        {
            if(_s1.HasField(fieldName))
                return _s1.GetInt(fieldName);
            else
                return _s2.GetInt(fieldName);
        }

        public string GetString(string fieldName)
        {
            if(_s1.HasField(fieldName))
                return _s1.GetString(fieldName);
            else
                return _s2.GetString(fieldName);
        }

        public Constant GetValue(string fieldName)
        {
            if(_s1.HasField(fieldName))
                return _s1.GetValue(fieldName);
            else
                return _s2.GetValue(fieldName);
        }

        public bool HasField(string fieldName)
        {
            return _s1.HasField(fieldName) || _s2.HasField(fieldName);
        }

        public bool Next()
        {
            if (_s2.Next())
                return true;
            else
            {
                _s2.BeforeFirst();
                
                return _s2.Next() && _s1.Next();
            }
        }
    }
}
