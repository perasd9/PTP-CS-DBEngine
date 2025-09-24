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
            _viewManager = new ViewManager(isNew, _tableManager, transaction);
            _statisticManager = new StatisticManager(_tableManager, transaction);
            _indexManager = new IndexManager(isNew, _tableManager, _statisticManager, transaction);

        }

        public TableManager TableManager { get => _tableManager; set => _tableManager = value; }
        public StatisticManager StatisticManager { get => _statisticManager; set => _statisticManager = value; }
        public ViewManager ViewManager { get => _viewManager; set => _viewManager = value; }
        public IndexManager IndexManager { get => _indexManager; set => _indexManager = value; }
    }
}
