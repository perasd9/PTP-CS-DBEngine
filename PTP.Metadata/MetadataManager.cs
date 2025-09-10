namespace PTP.Metadata
{
    public class MetadataManager
    {
        private static TableManager _tableManager;
        private static StatisticManager _statisticManager;
        private static ViewManager _viewManager;
        private static IndexManager _indexManager;

        public MetadataManager(bool isNew, Transaction.Transaction transaction)
        {
            _tableManager = new TableManager(isNew, transaction);
            _statisticManager = new StatisticManager(_tableManager, transaction);
            _viewManager = new ViewManager(isNew, _tableManager, transaction);
            _indexManager = new IndexManager(isNew, _tableManager, transaction);

        }
    }
}
