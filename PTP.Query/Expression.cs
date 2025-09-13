using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.Query
{
    public class Expression
    {
        private Constant _value = null;
        private string _fieldName = null;

        public Expression(Constant value)
        {
            _value = value;
        }
        public Expression(string fieldName)
        {
            _fieldName = fieldName;
        }

        public bool IsFieldName()
        {
            return _fieldName != null;
        }

        public Constant AsConstant => _value;

        public string AsFieldName => _fieldName;

        public Constant Evaluate(IScan scan)
        {
            return _value != null ? _value : scan.GetValue(_fieldName);
        }

        public bool AppliesTo(Schema schema)
        {
            return _value != null ? true : schema.HasField(_fieldName);
        }

        public override string ToString()
        {
            return _value != null ? _value.ToString() : _fieldName;
        }
    }
}
