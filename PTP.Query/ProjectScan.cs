using PTP.Query.Interfaces;

namespace PTP.Query
{
    public class ProjectScan : IScan
    {
        private IScan _scan;
        private ICollection<string> _fields;

        public ProjectScan(IScan scan, List<string> fields)
        {
            _scan = scan;
            _fields = fields;
        }

        public void BeforeFirst()
        {
            _scan.BeforeFirst();
        }

        public void Close()
        {
            _scan.Close();
        }

        public int GetInt(string fieldName)
        {
            if(this.HasField(fieldName))
                return _scan.GetInt(fieldName);
            else
                throw new ArgumentException($"Field {fieldName} not found.");
        }

        public string GetString(string fieldName)
        {
            if (this.HasField(fieldName))
                return _scan.GetString(fieldName);
            else
                throw new ArgumentException($"Field {fieldName} not found.");
        }

        public Constant GetValue(string fieldName)
        {
            if (this.HasField(fieldName))
                return _scan.GetValue(fieldName);
            else
                throw new ArgumentException($"Field {fieldName} not found.");
        }

        public bool HasField(string fieldName)
        {
            return _fields.Contains(fieldName);
        }

        public bool Next()
        {
            return _scan.Next();
        }
    }
}
