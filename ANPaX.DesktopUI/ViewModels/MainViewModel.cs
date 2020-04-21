using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ANPaX.AggregateFormation;
using ANPaX.DesktopUI.Models;

using Caliburn.Micro;

using Moq;

using NLog;

namespace ANPaX.DesktopUI.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private static string _defaultSizeDistribution = "FSP Standard";
        private static string _defaultAggFormationFactory = "Cluster Cluster Aggregation";
        private string _selectedAggSizeDistribution = _defaultSizeDistribution;
        private string _selectedPPSizeDistribution = _defaultSizeDistribution;
        private string _selectedAggFormationFactory = _defaultAggFormationFactory;
        private string _selectedPSDMeanMode = "Geometric Mean";
        private string _selectedASDMeanMode = "Geometric Mean";
        private string _simulatonPath = "<Select path>";
        private ILogger _logger = new Mock<ILogger>().Object;
        private bool _doSaveAggregates = true;


        public AggregateFormationConfig Config { get; set; }
        public static ITabViewModel AggConfigView;
        public static ITabViewModel FilmConfigView;
        public double MeanPPRadius { get; set; } = 5.0;
        public double SdevPPRadius { get; set; } = 0.23;

        public string SimulationName { get; set; } = DateTime.Now.ToString("yyyyMMdd");

        public int NumberOfCPU { get; set; } = Environment.ProcessorCount;

        public string SimulationPath
        {
            get => _simulatonPath;
            set
            {
                _simulatonPath = value;
                NotifyOfPropertyChange(() => SimulationPath);
            }
        }

        public bool DoSaveAggregates
        {
            get => _doSaveAggregates;
            set
            {
                _doSaveAggregates = value;
                NotifyOfPropertyChange(() => DoSaveAggregates);
            }
        }

        public string AggregateFileName { get; set; } = "Aggregates.xml";

        public MainViewModel()
        {
            Config = new AggregateFormationConfig();
            AggConfigView = new AggregateFormationConfigViewModel(Config);
            FilmConfigView = new FilmFormationConfigViewModel();
        }




        public ObservableCollection<ITabViewModel> TabViewModels { get; set; } = new ObservableCollection<ITabViewModel>() { AggConfigView, FilmConfigView };

        public ITabViewModel SelectedTabViewModel { get; set; } = AggConfigView;

        public void SetSimulationPath()
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            SimulationPath = dialog.SelectedPath;
        }

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


        public async void GenerateAggregates()
        {
            var progress = new Mock<IProgress<ProgressReportModel>>().Object;
            var view = (AggregateFormationConfigViewModel)AggConfigView;
            var service = new AggregateFormationService(view.Config, _logger);
            var result = await service.GenerateAggregates_Parallel_Async(NumberOfCPU, progress);
        }

    }
}
