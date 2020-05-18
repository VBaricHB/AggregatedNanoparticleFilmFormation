using System.Collections.Generic;

using ANPaX.UI.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI.ViewModels
{
    public class ClusterConfigViewModel : Screen, IConfigViewModel
    {
        private static string _defaultAggFormationFactory = "Cluster Cluster Aggregation";
        private string _selectedAggFormationFactory = _defaultAggFormationFactory;

        public string Header => "Cluster Configuration";
        public AggregateFormationConfig Config { get; set; }
        public List<string> AvailableAggFormationFactories { get; set; } = new List<string> { _defaultAggFormationFactory };

        public string SelectedAggFormationFactory
        {
            get => _selectedAggFormationFactory;
            set
            {
                _selectedAggFormationFactory = value;
                NotifyOfPropertyChange(() => SelectedAggFormationFactory);
            }
        }

        public ClusterConfigViewModel(AggregateFormationConfig config)
        {
            Config = config;
        }
    }
}
