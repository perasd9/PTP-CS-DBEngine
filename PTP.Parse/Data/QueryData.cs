using PTP.Query;
using System;

namespace PTP.Parse.Data
{
    public class QueryData
    {
        public QueryData(List<string> fields, ICollection<string> tables, Predicate pred)
        {
            Fields = fields;
            Tables = tables;
            Pred = pred;
        }

        public List<string> Fields { get; }
        public ICollection<string> Tables { get; }
        public Predicate Pred { get; }

        public override string ToString()
        {
            string result = "select ";
            foreach(string fldname in Fields)
                result += fldname + ", ";
            result = result.Substring(0, result.Length - 2);
            result += " from ";
            foreach (string tblname in Tables)
                result += tblname + ", ";
            result = result.Substring(0, result.Length - 2);
            string predstring = Pred.ToString();
            if (!predstring.Equals(""))
                result += " where " + predstring;
            return result;
        }
    }
}
