using PTP.Query.Interfaces;
using PTP.Shared;

namespace PTP.Query
{
    public class Predicate
    {
        private ICollection<Term> _terms = new List<Term>();

        public Predicate()
        {
            
        }

        public Predicate(Term term)
        {
            _terms.Add(term);
        }

        public void ConjoinWith(Predicate pred)
        {
            foreach(var t in pred._terms)
                _terms.Add(t);
        }

        public bool IsSatisfied(IScan s)
        {
            foreach(var t in _terms)
            {
                if(!t.IsSatisfied(s))
                    return false;
            }
            return true;
        }

        public int ReductionFactor(IPlan p)
        {
            int factor = 1;
            foreach(var t in _terms)
            {
                factor *= t.ReductionFactor(p);
            }
            return factor;
        }

        public Predicate SelectSubPred(Schema schema)
        {
            Predicate result = new Predicate();

            foreach(var t in _terms)
            {
                if(t.AppliesTo(schema))
                    result._terms.Add(t);
            }

            if(result._terms.Count == 0)
                return null;
            else

                return result;
        }

        public Predicate JoinSubPred(Schema schema1, Schema schema2)
        {
            Predicate result = new Predicate();
            Schema combinedSchema = new Schema();

            combinedSchema.AddAll(schema1);
            combinedSchema.AddAll(schema2);

            foreach(var t in _terms)
            {
                if(t.AppliesTo(combinedSchema) && !t.AppliesTo(schema1) && !t.AppliesTo(schema2))
                    result._terms.Add(t);
            }

            if(result._terms.Count == 0)
                return null;
            else
                return result;
        }

        public Constant EquatesWithConstant(string fieldName)
        {
            foreach(var t in _terms)
            {
                Constant c = t.EquatesWithConstant(fieldName);
                if(c != null)
                    return c;
            }

            return null;
        }

        public string EquatesWithField(string fieldName)
        {
            foreach(var t in _terms)
            {
                string s = t.EquatesWithField(fieldName);
                if(s != null)
                    return s;
            }
            return null;
        }

        public string ToString()
        {
            string result = "";
            foreach(var t in _terms)
            {
                if(result.Length > 0)
                    result += " and ";
                result += t.ToString();
            }
            return result;
        }
    }
}