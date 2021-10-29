using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.DesktopUI.Models;
using ANPaX.IO;
using ANPaX.Simulation.AggregateFormation;
using ANPaX.UI.DesktopUI.Models;
using ANPaX.UI.DesktopUI.Views;

using Caliburn.Micro;

using Moq;

using NLog;

namespace ANPaX.UI.DesktopUI.ViewModels
{
    public class AggFormationControlViewModel : Conductor<object>
    {
        #region private variables

        private string _selectedAggFileFormat = "json";
        private readonly ILogger _logger = new Mock<ILogger>().Object;
        private CancellationTokenSource _cts;

        private FileExport _export = new FileExport();
        #endregion

        public List<Aggregate> GeneratedAggregates { get; set; }
        public AdvancedConfigViewModel AdvancedConfigViewModel { get; set; }
        public AggregateConfigViewModel AggregateConfigViewModel { get; set; }
        public ClusterConfigViewModel ClusterConfigViewModel { get; set; }
        public PrimaryParticleConfigViewModel PrimaryParticleConfigViewModel { get; set; }
        public LoggingViewModel LoggingViewModel { get; set; }
        public StatusViewModel StatusViewModel { get; set; }

        public AggregateFormationConfig Config { get; set; }
        public SimulationProperties SimProp { get; set; }
        public AggregateFormationConfigViewModel AggregateFormationConfigViewModel { get; set; }

        public AggregateInformationViewModel AggregateInformationViewModel { get; set; }

        public string AggFileName { get; set; } = "Aggregates.json";

        public FileFormat FileFormat { get; set; } = FileFormat.Json;
        public List<string> AvailableAggFileFormats { get; set; } = new List<string>() { "xml", "json", "none" };

        public bool DoAutoSaveFile { get; set; } = true;

        public int NumberOfPrimaryParticles
        {
            get => Config.TotalPrimaryParticles;
            set
            {
                Config.TotalPrimaryParticles = value;
                NotifyOfPropertyChange(() => NumberOfPrimaryParticles);
            }
        }
        public string SelectedAggFileFormat
        {
            get => _selectedAggFileFormat;
            set
            {
                _selectedAggFileFormat = value;
                AdjustAggFileName(value);
                SetFileFormat(value);
                NotifyOfPropertyChange(() => SelectedAggFileFormat);
                NotifyOfPropertyChange(() => AggFileName);
            }
        }

        public AggFormationControlViewModel(
            AggregateFormationConfig config,
            SimulationProperties simulationProperties,
            StatusViewModel statusViewModel,
            LoggingViewModel loggingViewModel)
        {
            Config = config;
            SimProp = simulationProperties;
            AggregateFormationConfigViewModel = new AggregateFormationConfigViewModel(Config);
            AdvancedConfigViewModel = new AdvancedConfigViewModel(Config);
            AggregateConfigViewModel = new AggregateConfigViewModel(Config);
            ClusterConfigViewModel = new ClusterConfigViewModel(Config);
            PrimaryParticleConfigViewModel = new PrimaryParticleConfigViewModel(Config);
            StatusViewModel = statusViewModel;
            LoggingViewModel = loggingViewModel;
            AggregateInformationViewModel = new AggregateInformationViewModel();
        }

        private void AdjustAggFileName(string fileEnding)
        {
            if (Path.GetExtension(AggFileName) == $".{fileEnding}")
            {
                return;
            }
            if (fileEnding == "none")
            {
                return;
            }

            AggFileName = AggFileName.Split('.')[0] + $".{fileEnding}";
        }

        public void SetSimulationPath()
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            SimProp.SimulationPath = dialog.SelectedPath;
        }

        public async Task GenerateAggregates(AggFormationControlView view)
        {
            StatusViewModel.SimulationStatus = SimulationStatus.Running;
            if (!(view is null))
            {
                view.CancelGenerationButton.Visibility = System.Windows.Visibility.Visible;
                view.GenerateAggregatesButton.Visibility = System.Windows.Visibility.Hidden;
            }

            LogInfo("Starting aggregate formation simulation");

            var progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += StatusViewModel.ReportProgress;

            var service = new AggregateFormationService(
                new AggregateSizeDistributionFactory(),
                new PrimaryParticleSizeDistributionFactory(),
                new AggregateFormationFactory(),
                new AccordNeighborslistFactory(),
                Config,
                _logger);
            _cts = new CancellationTokenSource();

            GeneratedAggregates = await service.GenerateAggregates_Parallel_Async(SimProp.NumberOfCPU, progress, _cts.Token);
            AggregateInformationViewModel.UpdateAggregates(GeneratedAggregates);

            if (StatusViewModel.SimulationStatus == SimulationStatus.Canceling)
            {
                StatusViewModel.SimulationStatus = SimulationStatus.Idle;
            }
            else
            {
                StatusViewModel.SimulationStatus = SimulationStatus.Idle;
                LogInfo("Aggregate generation finished.");
                if (DoAutoSaveFile)
                {
                    ExportAggregatesToFile();
                }
            }
            if (!(view is null))
            {
                view.CancelGenerationButton.Visibility = System.Windows.Visibility.Hidden;
                view.GenerateAggregatesButton.Visibility = System.Windows.Visibility.Visible;
            }

        }

        public void CancelGeneration(AggFormationControlView view)
        {
            _cts.Cancel();
            LogInfo("Simulation canceled by user");
            StatusViewModel.SimulationStatus = SimulationStatus.Canceling;
            view.CancelGenerationButton.Visibility = System.Windows.Visibility.Hidden;
            view.GenerateAggregatesButton.Visibility = System.Windows.Visibility.Visible;

        }

        public void ExportAggregatesToFile()
        {

            var isSuccess = RunExportPreCheck();
            if (!isSuccess)
            {
                return;
            }

            var fileNameWithPath = Path.Join(SimProp.SimulationPath, AggFileName);
            LogInfo($"Exporting aggregates:{Environment.NewLine}{fileNameWithPath}");

            _export.Export(
                GeneratedAggregates,
                Config,
                fileNameWithPath, FileFormat, false);

            LogInfo("done.");
        }

        private void SetFileFormat(string fileFormatString)
        {
            switch (fileFormatString)
            {
                case "json":
                    FileFormat = FileFormat.Json;
                    break;
                case "xml":
                    FileFormat = FileFormat.Xml;
                    break;
                case "none":
                    FileFormat = FileFormat.None;
                    break;
                default:
                    return;
            }
        }

        private bool RunExportPreCheck()
        {
            if (!GeneratedAggregates.Any())
            {
                LogWarning("Did not save file, no loaded aggregates");
                return false;
            }

            if (SimProp.SimulationPath == "<Select path>")
            {
                LogWarning("Did not save file, no simulation path chosen");
                return false;
            }

            if (FileFormat == FileFormat.None)
            {
                LogWarning("Did not save file, no file format chosen");
                return false;
            }

            if (!DoFormatEndingMatch())
            {
                LogWarning("File format and ending do not match");
            }
            return true;
        }

        private bool DoFormatEndingMatch()
        {
            var fileEnding = AggFileName.Split('.')[1];
            if ((fileEnding == "xml" && FileFormat == FileFormat.Xml) ||
                (fileEnding == "json" && FileFormat == FileFormat.Json))
            {
                return true;
            }

            return false;
        }

        private void LogInfo(string message) => LoggingViewModel.LogInfo(message);
        private void LogWarning(string message) => LoggingViewModel.LogWarning(message);

        #region ViewVisibility
        public void ShowAdvancedConfigView(AggFormationControlView view)
        {
            SwitchToHideAdvancedConfigButton(view);
            SwitchToShowAggregateConfigButton(view);
            SwitchToShowClusterConfigButton(view);
            SwitchToShowPrimaryParticleConfigButton(view);

            ActivateItem(AdvancedConfigViewModel);
        }

        public void HideAdvancedConfigView(AggFormationControlView view)
        {
            SwitchToShowAdvancedConfigButton(view);

            DeactivateItem(AdvancedConfigViewModel, false);
        }

        public void ShowAggregateConfigView(AggFormationControlView view)
        {
            SwitchToShowAdvancedConfigButton(view);
            SwitchToHideAggregateConfigButton(view);
            SwitchToShowClusterConfigButton(view);
            SwitchToShowPrimaryParticleConfigButton(view);

            ActivateItem(AggregateConfigViewModel);
        }

        public void HideAggregateConfigView(AggFormationControlView view)
        {
            SwitchToShowAggregateConfigButton(view);

            DeactivateItem(AggregateConfigViewModel, false);
        }

        public void ShowClusterConfigView(AggFormationControlView view)
        {
            SwitchToShowAdvancedConfigButton(view);
            SwitchToShowAggregateConfigButton(view);
            SwitchToHideClusterConfigButton(view);
            SwitchToShowPrimaryParticleConfigButton(view);

            ActivateItem(ClusterConfigViewModel);
        }

        public void HideClusterConfigView(AggFormationControlView view)
        {
            SwitchToShowClusterConfigButton(view);
            DeactivateItem(ClusterConfigViewModel, false);
        }

        public void ShowPrimaryParticleConfigView(AggFormationControlView view)
        {
            SwitchToShowAdvancedConfigButton(view);
            SwitchToShowAggregateConfigButton(view);
            SwitchToShowClusterConfigButton(view);
            SwitchToHidePrimaryParticleConfigButton(view);

            ActivateItem(PrimaryParticleConfigViewModel);
        }

        public void HidePrimaryParticleConfigView(AggFormationControlView view)
        {
            SwitchToShowPrimaryParticleConfigButton(view);
            DeactivateItem(PrimaryParticleConfigViewModel, false);
        }
        #endregion

        #region ButtonVisibility
        private void SwitchToShowAdvancedConfigButton(AggFormationControlView view)
        {
            view.HideAdvancedConfigButton.Visibility = System.Windows.Visibility.Hidden;
            view.ShowAdvancedConfigButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void SwitchToHideAdvancedConfigButton(AggFormationControlView view)
        {
            view.HideAdvancedConfigButton.Visibility = System.Windows.Visibility.Visible;
            view.ShowAdvancedConfigButton.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SwitchToShowAggregateConfigButton(AggFormationControlView view)
        {
            view.HideAggregateConfigButton.Visibility = System.Windows.Visibility.Hidden;
            view.ShowAggregateConfigButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void SwitchToHideAggregateConfigButton(AggFormationControlView view)
        {
            view.HideAggregateConfigButton.Visibility = System.Windows.Visibility.Visible;
            view.ShowAggregateConfigButton.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SwitchToShowClusterConfigButton(AggFormationControlView view)
        {
            view.HideClusterConfigButton.Visibility = System.Windows.Visibility.Hidden;
            view.ShowClusterConfigButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void SwitchToHideClusterConfigButton(AggFormationControlView view)
        {
            view.HideClusterConfigButton.Visibility = System.Windows.Visibility.Visible;
            view.ShowClusterConfigButton.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SwitchToShowPrimaryParticleConfigButton(AggFormationControlView view)
        {
            view.HidePrimaryParticleConfigButton.Visibility = System.Windows.Visibility.Hidden;
            view.ShowPrimaryParticleConfigButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void SwitchToHidePrimaryParticleConfigButton(AggFormationControlView view)
        {
            view.HidePrimaryParticleConfigButton.Visibility = System.Windows.Visibility.Visible;
            view.ShowPrimaryParticleConfigButton.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion
    }
}
