using PTP.Query;

namespace PTP.Parse.Data
{
    public class DeleteData
    {
        public DeleteData(string tableName, Predicate pred)
        {
            TableName = tableName;
            Pred = pred;
        }

        public string TableName { get; }
        public Predicate Pred { get; }
    }
}
