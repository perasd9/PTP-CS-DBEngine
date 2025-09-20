using PTP.Shared;

namespace PTP.Parse.Data
{
    public class CreateTableData
    {
        public CreateTableData(string tableName, Schema schema)
        {
            TableName = tableName;
            Schema = schema;
        }

        public string TableName { get; }
        public Schema Schema { get; }
    }
}
