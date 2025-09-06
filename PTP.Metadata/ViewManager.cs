using PTP.Shared;

namespace PTP.Metadata
{
    public class ViewManager
    {
        private static int MAX_VIEW_LENGTH_SQL = 200; //maximum length of view definition SQL string
        private TableManager _tableManager;

        public ViewManager(bool isNew, TableManager tableManager, Transaction.Transaction transaction)
        {
            this._tableManager = tableManager;

            if(isNew)
            {
                Schema schema = new Schema();
                schema.AddStringField("viewname", TableManager.MAX_LENGTH_NAME);
                schema.AddStringField("viewdef", MAX_VIEW_LENGTH_SQL);
                tableManager.CreateTable("viewcat", schema, transaction);
            }
        }

        public void CreateView(string viewName, string viewDefiniton, Transaction.Transaction transaction)
        {
            Layout layout = _tableManager.GetLayout("viewcat", transaction);
            TableScan tableScan = new TableScan(transaction, layout, "viewcat");

            tableScan.SetString("viewname", viewName);
            tableScan.SetString("viewdef", viewDefiniton);

            tableScan.Close();
        }

        public string GetViewDefinition(string viewName, Transaction.Transaction transaction)
        {
            string result = null;

            Layout layout = _tableManager.GetLayout("viewcat", transaction);

            TableScan tableScan = new TableScan(transaction, layout, "viewcat");

            while (tableScan.Next())
            {
                if (tableScan.GetString("viewname") == viewName)
                {
                    result = tableScan.GetString("viewdef");
                    break;
                }
            }

            tableScan.Close();

            return result;
        }
    }
}
