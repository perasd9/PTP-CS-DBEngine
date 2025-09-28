using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.Query
{
    public class Term
    {
        private Expression _leftHandSide, _rightHandSide;

        public Term(Expression leftHandSide, Expression rightHandSide)
        {
            _leftHandSide = leftHandSide;
            _rightHandSide = rightHandSide;
        }

        public bool IsSatisfied(IScan scan)
        {
            Constant lhsValue = _leftHandSide.Evaluate(scan);
            Constant rhsValue = _rightHandSide.Evaluate(scan);

            return lhsValue.Equals(rhsValue);
        }

        public bool AppliesTo(Schema schema)
        {
            return _leftHandSide.AppliesTo(schema) && _rightHandSide.AppliesTo(schema);
        }

        public int ReductionFactor(IPlan plan)
        {
            string lhsName, rhsName;

            if (_leftHandSide.IsFieldName() && _rightHandSide.IsFieldName())
            {
                lhsName = _leftHandSide.AsFieldName;
                rhsName = _rightHandSide.AsFieldName;
                return Math.Max(plan.DistinctValues(lhsName), plan.DistinctValues(rhsName));
            }
            if (_leftHandSide.IsFieldName())
            {
                lhsName = _leftHandSide.AsFieldName;
                return plan.DistinctValues(lhsName);
            }
            if (_rightHandSide.IsFieldName())
            {
                rhsName = _rightHandSide.AsFieldName;
                return plan.DistinctValues(rhsName);
            }
            if (_leftHandSide.AsConstant.Equals(_rightHandSide.AsConstant))
                return 1;
            else
                return int.MaxValue;
        }

        public Constant EquatesWithConstant(string fieldName)
        {
            if(_leftHandSide.IsFieldName() && _leftHandSide.AsFieldName.Equals(fieldName) && !_rightHandSide.IsFieldName())
                return _rightHandSide.AsConstant;
            else if(_rightHandSide.IsFieldName() && _rightHandSide.AsFieldName.Equals(fieldName) && !_leftHandSide.IsFieldName())
                return _leftHandSide.AsConstant;
            else
                return null!;
        }

        public string EquatesWithField(string fieldName)
        {
            if (_leftHandSide.IsFieldName() && _leftHandSide.AsFieldName.Equals(fieldName) && _rightHandSide.IsFieldName())
                return _rightHandSide.AsFieldName;
            else if (_rightHandSide.IsFieldName() && _rightHandSide.AsFieldName.Equals(fieldName) && _leftHandSide.IsFieldName())
                return _leftHandSide.AsFieldName;
            else
                return null!;
        }

        public override string ToString()
        {
            return _leftHandSide.ToString() + " = " + _rightHandSide.ToString();
        }
    }
}
