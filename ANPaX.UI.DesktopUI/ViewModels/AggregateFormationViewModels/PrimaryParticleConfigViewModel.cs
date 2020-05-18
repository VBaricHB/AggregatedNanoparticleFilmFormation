using System.Collections.Generic;

using ANPaX.UI.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI.ViewModels
{
    public class PrimaryParticleConfigViewModel : Screen, IConfigViewModel
    {
        private static string _defaultSizeDistribution = "FSP Standard";
        private string _selectedPPSizeDistribution = _defaultSizeDistribution;

        public string Header => "Primary Particle Configuration";
        public AggregateFormationConfig Config { get; set; }
        public List<string> AvailablePPSizeDistributions { get; set; } = new List<string>() { _defaultSizeDistribution, "Monodisperse" };

        public double MeanPPRadius { get; set; } = 5;
        public double SdevPPRadius { get; set; } = 0.230;

        public string SelectedPPSizeDistribution
        {
            get => _selectedPPSizeDistribution;
            set
            {
                _selectedPPSizeDistribution = value;
                NotifyOfPropertyChange(() => SelectedPPSizeDistribution);
            }
        }

        public PrimaryParticleConfigViewModel(AggregateFormationConfig config)
        {
            Config = config;
        }

    }
}
