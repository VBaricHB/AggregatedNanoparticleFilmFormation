using System.Collections.Generic;

using ANPaX.UI.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI.ViewModels
{
    public class AggregateConfigViewModel : Screen, IConfigViewModel
    {


        private static string _defaultSizeDistribution = "FSP Standard";

        private string _selectedAggSizeDistribution = _defaultSizeDistribution;

        public List<string> AvailableAggSizeDistributions { get; set; } = new List<string>() { _defaultSizeDistribution };
        public AggregateFormationConfig Config { get; set; }

        public string Header => "Aggregate Configuration";

        public string SelectedAggSizeDistribution
        {
            get => _selectedAggSizeDistribution;
            set
            {
                _selectedAggSizeDistribution = value;
                NotifyOfPropertyChange(() => SelectedAggSizeDistribution);
            }
        }

        public AggregateConfigViewModel(AggregateFormationConfig config)
        {
            Config = config;
        }
    }
}
