namespace PTP.Parse.Data
{
    public class CreateIndexData
    {
        public CreateIndexData(string indexName, string tableName, string fieldName)
        {
            IndexName = indexName;
            TableName = tableName;
            FieldName = fieldName;
        }

        public string IndexName { get; }
        public string TableName { get; }
        public string FieldName { get; }
    }
}
