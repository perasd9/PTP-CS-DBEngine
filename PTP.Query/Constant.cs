namespace PTP.Query
{
    public class Constant : IComparable
    {
        private int _intValue;
        private string? _stringValue = null;

        public Constant(int intValue)
        {
            this._intValue = intValue;
        }

        public Constant(string stringValue)
        {
            this._stringValue = stringValue;
        }

        public int AsInt()
        {
            return _intValue;
        }

        public string? AsString()
        {
            return _stringValue;
        }

        public int CompareTo(object? obj)
        {
            Constant constant = (Constant)obj!;

            return _stringValue != null ? _stringValue.CompareTo(constant.AsString()) : _intValue.CompareTo(constant.AsInt());
        }

        public override bool Equals(object? obj)
        {
            Constant constant = (Constant)obj!;

            return _stringValue != null ? _stringValue.Equals(constant.AsString()) : _intValue.Equals(constant.AsInt());
        }

        public override int GetHashCode()
        {
            return _stringValue != null ? _stringValue.GetHashCode() : _intValue.GetHashCode();
        }

        public override string ToString()
        {
            return _stringValue != null ? _stringValue : _intValue.ToString();
        }
    }
}