using System.Collections.Generic;

using ANPaX.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class AggregateFormationConfigViewModel : Screen, ITabViewModel
    {
        private static string _defaultSizeDistribution = "FSP Standard";
        private static string _defaultAggFormationFactory = "Cluster Cluster Aggregation";
        private string _selectedAggSizeDistribution = _defaultSizeDistribution;
        private string _selectedPPSizeDistribution = _defaultSizeDistribution;
        private string _selectedAggFormationFactory = _defaultAggFormationFactory;
        private string _selectedPSDMeanMode = "Geometric Mean";
        private string _selectedASDMeanMode = "Geometric Mean";

        public AggregateFormationConfig Config { get; set; } = new AggregateFormationConfig();

        public double MeanPPRadius { get; set; }

        public List<string> AvailablePPSizeDistributions { get; set; } = new List<string>() { _defaultSizeDistribution, "Monodisperse" };

        public List<string> AvailableAggSizeDistributions { get; set; } = new List<string>() { _defaultSizeDistribution };

        public List<string> AvailableAggFormationFactories { get; set; } = new List<string> { _defaultAggFormationFactory };

        public List<string> AvailablePSDMeanModes { get; set; } = new List<string> { "Arithmetic Mean", "Geometric Mean", "Sauter radius" };
        public List<string> AvailableASDMeanModes { get; set; } = new List<string> { "Arithmetic Mean", "Geometric Mean" };

        public string SelectedAggFormationFactory
        {
            get => _selectedAggFormationFactory;
            set
            {
                _selectedAggFormationFactory = value;
                NotifyOfPropertyChange(() => SelectedAggFormationFactory);
            }
        }

        public string SelectedPSDMeanMode
        {
            get => _selectedPSDMeanMode;
            set
            {
                _selectedPSDMeanMode = value;
                NotifyOfPropertyChange(() => SelectedPSDMeanMode);

            }
        }

        public string SelectedASDMeanMode
        {
            get => _selectedASDMeanMode;
            set
            {
                _selectedASDMeanMode = value;
                NotifyOfPropertyChange(() => SelectedASDMeanMode);

            }
        }

        public string SelectedAggSizeDistribution
        {
            get => _selectedAggSizeDistribution;
            set
            {
                _selectedAggSizeDistribution = value;
                NotifyOfPropertyChange(() => SelectedAggSizeDistribution);
            }
        }

        public string SelectedPPSizeDistribution
        {
            get => _selectedPPSizeDistribution;
            set
            {
                _selectedPPSizeDistribution = value;
                NotifyOfPropertyChange(() => SelectedPPSizeDistribution);
            }
        }

        public string Header => "Aggregate Formation Config";
    }
}
