using PTP.Shared;

namespace PTP.Metadata
{
    public class TableManager
    {
        public static int MAX_LENGTH_NAME = 20; //maximun length of table or field name
        private Layout _tableCatalogLayout, _fieldCatalogLayout;

        public TableManager(bool isNew, Transaction.Transaction transaction)
        {
            Schema tableCatalogSchema = new Schema();
            tableCatalogSchema.AddStringField("tblname", MAX_LENGTH_NAME);
            tableCatalogSchema.AddIntField("slotsize");

            _tableCatalogLayout = new Layout(tableCatalogSchema);

            Schema fieldCatalogSchema = new Schema();
            fieldCatalogSchema.AddStringField("tblname", MAX_LENGTH_NAME);
            fieldCatalogSchema.AddStringField("fldname", MAX_LENGTH_NAME);
            fieldCatalogSchema.AddIntField("type");
            fieldCatalogSchema.AddIntField("length");
            fieldCatalogSchema.AddIntField("offset");

            _fieldCatalogLayout = new Layout(fieldCatalogSchema);

            if (isNew)
            {
                this.CreateTable("tblcat", tableCatalogSchema, transaction);
                this.CreateTable("fldcat", fieldCatalogSchema, transaction);
            }
        }

        public void CreateTable(string name, Schema schema, Transaction.Transaction transaction)
        {
            Layout layout = new Layout(schema);

            TableScan tableCatalog = new TableScan(transaction, _tableCatalogLayout, "tblcat");

            tableCatalog.Insert();
            tableCatalog.SetString("tblname", name);
            tableCatalog.SetInt("slotsize", layout.SlotSize);
            tableCatalog.Close();

            TableScan fileCatalog = new TableScan(transaction, _fieldCatalogLayout, "fldcat");

            foreach (var field in schema.Fields())
            {
                fileCatalog.Insert();
                fileCatalog.SetString("tblname", name);
                fileCatalog.SetString("fldname", field);
                fileCatalog.SetInt("type", (int)schema.Type(field));
                fileCatalog.SetInt("length", schema.Length(field));
                fileCatalog.SetInt("offset", layout.Offset(field));
            }

            fileCatalog.Close();
        }

        public Layout GetLayout(string tableName, Transaction.Transaction transaction)
        {
            int size = -1;

            TableScan tableCatalog = new TableScan(transaction, _tableCatalogLayout, "tblcat");

            while (tableCatalog.Next())
            {
                if (tableCatalog.GetString("tblname") == tableName)
                {
                    size = tableCatalog.GetInt("slotsize");
                    break;
                }
            }

            tableCatalog.Close();

            Schema schema = new Schema();

            IDictionary<string, int> offsets = new Dictionary<string, int>();

            TableScan fieldCatalog = new TableScan(transaction, _fieldCatalogLayout, "fldcat");

            while (fieldCatalog.Next())
            {
                if(fieldCatalog.GetString("tblname") == tableName)
                {
                    string fieldName = fieldCatalog.GetString("fldname");
                    int fieldType = fieldCatalog.GetInt("type");
                    int fieldLength = fieldCatalog.GetInt("length");
                    int fieldOffset = fieldCatalog.GetInt("offset");
                    schema.AddField(fieldName, fieldType, fieldLength);
                    offsets[fieldName] = fieldOffset;
                }
            }

            fieldCatalog.Close();


            return new Layout(schema, offsets, size);
        }
    }
}
