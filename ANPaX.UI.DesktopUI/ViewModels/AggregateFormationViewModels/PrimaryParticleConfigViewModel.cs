using System.Collections.Generic;
using System.Globalization;

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

        private string _modalRadius;
        private string _sdevPPRadius;

        public string ModalRadius
        {
            get => _modalRadius;
            set
            {
                if (value != _modalRadius)
                {
                    _modalRadius = value;
                    NotifyOfPropertyChange(() => ModalRadius);
                    var isSuccessful = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleValue);
                    if (isSuccessful)
                    {
                        Config.ModalRadius = doubleValue;
                    }
                }
            }
        }

        public string SdevPPRadius
        {
            get => _sdevPPRadius;
            set
            {
                if (_sdevPPRadius != value)
                {
                    _sdevPPRadius = value;
                    NotifyOfPropertyChange(() => SdevPPRadius);
                    var isSuccessful = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleValue);
                    if (isSuccessful)
                    {
                        Config.StdPPRadius = doubleValue;
                    }
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
            _modalRadius = config.ModalRadius.ToString(CultureInfo.InvariantCulture);
            _sdevPPRadius = config.StdPPRadius.ToString(CultureInfo.InvariantCulture);
        }

    }
}
