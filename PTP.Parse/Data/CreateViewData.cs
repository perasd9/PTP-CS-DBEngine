namespace PTP.Parse.Data
{
    public class CreateViewData
    {
        public CreateViewData(string viewName, QueryData queryData)
        {
            ViewName = viewName;
            QueryData = queryData;
        }

        public string ViewName { get; }
        public QueryData QueryData { get; }

        public string ViewDefinition()
        {
            return QueryData.ToString();
        }
    }
}
