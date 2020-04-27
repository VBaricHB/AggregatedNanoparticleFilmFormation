using System;
using System.Collections.Generic;
using System.IO;

using ANPaX.AggregateFormation;
using ANPaX.DesktopUI.Models;

using Caliburn.Micro;

using Moq;

using NLog;

namespace ANPaX.DesktopUI.ViewModels
{
    public class MainControlViewModel : Screen, ITabViewModel
    {
        #region private variables
        private AggregateFormationConfig _aggConfig;
        private string _selectedAggFileFormat = "xml";
        private string _simulatonPath = "<Select path>";
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructur takes a config from the MainView
        /// </summary>
        /// <param name="aggConfig"></param>
        public MainControlViewModel(AggregateFormationConfig aggConfig)
        {
            _aggConfig = aggConfig;
        }
        #endregion

        public string SimulationName { get; set; } = DateTime.Now.ToString("YYYYMMDD");
        public string Header => SimulationName;
        public List<string> AvailableAggFileFormats { get; set; } = new List<string>() { "xml", "json", "none" };

        public string AggFileName { get; set; } = "Aggregates.xml";

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

        public int NumberOfPrimaryParticles
        {
            get => _aggConfig.TotalPrimaryParticles;
            set
            {
                _aggConfig.TotalPrimaryParticles = value;
                NotifyOfPropertyChange(() => NumberOfPrimaryParticles);
            }
        }

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

        public void SetSimulationPath()
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            SimulationPath = dialog.SelectedPath;
        }

        public async void GenerateAggregates()
        {
            var progress = new Mock<IProgress<ProgressReportModel>>().Object;
            var service = new AggregateFormationService(_aggConfig, _logger);
            var result = await service.GenerateAggregates_Parallel_Async(NumberOfCPU, progress);
        }






    }
}
