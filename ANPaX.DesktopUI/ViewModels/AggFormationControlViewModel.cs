using System;
using System.Collections.Generic;
using System.IO;

using ANPaX.AggregateFormation;
using ANPaX.DesktopUI.Models;
using ANPaX.DesktopUI.Views;

using Caliburn.Micro;

using Moq;

using NLog;

namespace ANPaX.DesktopUI.ViewModels
{
    public class AggFormationControlViewModel : Conductor<object>
    {
        #region private variables

        private string _selectedAggFileFormat = "xml";
        private readonly ILogger _logger = new Mock<ILogger>().Object;
        #endregion

        AdvancedConfigViewModel AdvancedConfigViewModel { get; set; }
        AggregateConfigViewModel AggregateConfigViewModel { get; set; }
        ClusterConfigViewModel ClusterConfigViewModel { get; set; }
        PrimaryParticleConfigViewModel PrimaryParticleConfigViewModel { get; set; }

        public AggregateFormationConfig Config { get; set; }
        public SimulationProperties SimProp { get; set; }
        public AggregateFormationConfigViewModel AggregateFormationConfigViewModel { get; set; }

        public string AggFileName { get; set; } = "Aggregates.xml";
        public List<string> AvailableAggFileFormats { get; set; } = new List<string>() { "xml", "json", "none" };

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
                NotifyOfPropertyChange(() => SelectedAggFileFormat);
                NotifyOfPropertyChange(() => AggFileName);

            }
        }



        public AggFormationControlViewModel(AggregateFormationConfig config, SimulationProperties simulationProperties)
        {
            Config = config;
            SimProp = simulationProperties;
            AggregateFormationConfigViewModel = new AggregateFormationConfigViewModel(Config);
            AdvancedConfigViewModel = new AdvancedConfigViewModel(Config);
            AggregateConfigViewModel = new AggregateConfigViewModel(Config);
            ClusterConfigViewModel = new ClusterConfigViewModel(Config);
            PrimaryParticleConfigViewModel = new PrimaryParticleConfigViewModel(Config);
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

        public async void GenerateAggregates()
        {
            var progress = new Mock<IProgress<ProgressReportModel>>().Object;
            var service = new AggregateFormationService(Config, _logger);
            var result = await service.GenerateAggregates_Parallel_Async(SimProp.NumberOfCPU, progress);
        }

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
