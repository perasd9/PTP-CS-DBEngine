using PTP.Query;

namespace PTP.Parse.Data
{
    public class InsertData
    {
        public InsertData(string tableName, List<string> fields, List<Constant> values)
        {
            TableName = tableName;
            Fields = fields;
            Values = values;
        }

        public string TableName { get; }
        public List<string> Fields { get; }
        public List<Constant> Values { get; }
    }
}
