using System.Collections.Generic;

using ANPaX.Simulation.AggregateFormation;
using ANPaX.UI.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI.ViewModels
{
    public class PrimaryParticleConfigViewModel : Screen, IConfigViewModel
    {
        private const string _defaultSizeDistribution = "FSP Standard";
        private string _selectedPPSizeDistribution = _defaultSizeDistribution;

        public string Header => "Primary Particle Configuration";
        public AggregateFormationConfig Config { get; set; }
        public List<string> AvailablePPSizeDistributions { get; set; } = new List<string>() { _defaultSizeDistribution, "Monodisperse", "Lognormal" };

        private double _meanPPRadius = 5.5;
        private double _sdevPPRadius = 0.230;

        public double MeanPPRadius
        {
            get => _meanPPRadius;
            set
            {
                if (value != _meanPPRadius)
                {
                    _meanPPRadius = value;
                    NotifyOfPropertyChange(() => MeanPPRadius);
                    Config.MeanPPRadius = value;
                }
            }
        }

        public double SdevPPRadius
        {
            get => _sdevPPRadius;
            set
            {
                if (_meanPPRadius != value)
                {
                    _meanPPRadius = value;
                    NotifyOfPropertyChange(() => SdevPPRadius);
                    Config.StdPPRadius = value;
                }
            }
        }

        public string SelectedPPSizeDistribution
        {
            get => _selectedPPSizeDistribution;
            set
            {
                _selectedPPSizeDistribution = value;
                NotifyOfPropertyChange(() => SelectedPPSizeDistribution);
                Config.PrimaryParticleSizeDistributionType = ConvertStringToSizeDistribution(value);
                if (value != _defaultSizeDistribution)
                {
                    Config.UseDefaultGenerationMethods = false;
                }
            }
        }

        private SizeDistributionType ConvertStringToSizeDistribution(string value)
        {
            switch (value)
            {
                case "Lognormal":
                    return SizeDistributionType.LogNormal;
                case "Monodisperse":
                    return SizeDistributionType.Monodisperse;
                default:
                case _defaultSizeDistribution:
                    return SizeDistributionType.DissDefault;
            }
        }

        public PrimaryParticleConfigViewModel(AggregateFormationConfig config)
        {
            Config = config;
        }

    }
}
