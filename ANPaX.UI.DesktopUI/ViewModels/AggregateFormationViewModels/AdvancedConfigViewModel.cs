using System;
using System.Collections.Generic;

using ANPaX.Simulation.AggregateFormation;
using ANPaX.UI.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI.ViewModels
{
    public class AdvancedConfigViewModel : Screen, IConfigViewModel
    {
        private string _selectedPSDMeanMode = "Geometric Mean";
        private string _selectedASDMeanMode = "Geometric Mean";

        public string Header => "Advanced Configuration";

        public AggregateFormationConfig Config { get; set; }

        public AdvancedConfigViewModel(AggregateFormationConfig config)
        {
            Config = config;
        }

        public List<string> AvailablePSDMeanModes { get; set; } = new List<string> { "Arithmetic Mean", "Geometric Mean", "Sauter radius" };
        public List<string> AvailableASDMeanModes { get; set; } = new List<string> { "Arithmetic Mean", "Geometric Mean" };
        public string SelectedPSDMeanMode
        {
            get => _selectedPSDMeanMode;
            set
            {
                _selectedPSDMeanMode = value;
                NotifyOfPropertyChange(() => SelectedPSDMeanMode);
                Config.RadiusMeanCalculationMethod = ConvertStringToMeanMethod(value);

            }
        }

        public string SelectedASDMeanMode
        {
            get => _selectedASDMeanMode;
            set
            {
                _selectedASDMeanMode = value;
                NotifyOfPropertyChange(() => SelectedASDMeanMode);
                Config.AggregateSizeMeanCalculationMethod = ConvertStringToMeanMethod(value);

            }
        }

        public MeanMethod ConvertStringToMeanMethod(string methodString)
        {
            switch (methodString)
            {
                case "Arithmetic Mean":
                    return MeanMethod.Arithmetic;
                case "Geometric Mean":
                    return MeanMethod.Geometric;
                case "Sauter radius":
                    return MeanMethod.Sauter;

            }
            throw new ArgumentException($"Unknown string {methodString}");
        }
    }
}
