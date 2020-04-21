using System;
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

        public static ITabViewModel AggConfigView = new AggregateFormationConfigViewModel();
        public static ITabViewModel FilmConfigView = new FilmFormationConfigViewModel();
        private ILogger _logger = new Mock<ILogger>().Object;

        public string SimulationName { get; set; }
        private string _simulatonPath = "<Select path>";

        public string SimulationPath
        {
            get { return _simulatonPath; }
            set
            {
                _simulatonPath = value;
                NotifyOfPropertyChange(() => SimulationPath);
            }
        }


        public ObservableCollection<ITabViewModel> TabViewModels { get; set; } = new ObservableCollection<ITabViewModel>() { AggConfigView, FilmConfigView };

        public ITabViewModel SelectedTabViewModel { get; set; } = AggConfigView;

        public void SetSimulationPath()
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            SimulationPath = dialog.SelectedPath;
        }

        public async void GenerateAggregates()
        {
            var progress = new Mock<IProgress<ProgressReportModel>>().Object;
            var view = (AggregateFormationConfigViewModel)AggConfigView;
            var service = new AggregateFormationService(view.Config, _logger);
            var result = await service.GenerateAggregates_Parallel_Async(4, progress);
        }

    }
}
