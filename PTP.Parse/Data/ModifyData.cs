using PTP.Query;

namespace PTP.Parse.Data
{
    public class ModifyData
    {
        public ModifyData(string tableName, string fieldName, Expression newValue, Predicate pred)
        {
            TableName = tableName;
            FieldName = fieldName;
            NewValue = newValue;
            Pred = pred;
        }

        public string TableName { get; }
        public string FieldName { get; }
        public Expression NewValue { get; }
        public Predicate Pred { get; }
    }
}
