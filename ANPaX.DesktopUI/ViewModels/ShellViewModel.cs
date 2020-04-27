using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

using ANPaX.DesktopUI.Models;

using Caliburn.Micro;

using Moq;

using NLog;

namespace ANPaX.DesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private ILogger _logger = new Mock<ILogger>().Object;
        private ITabViewModel _selectedTabViewModel;

        public ITabViewModel SelectedTabViewModel
        {
            get => _selectedTabViewModel;
            set
            {
                _selectedTabViewModel = value;
                NotifyOfPropertyChange(() => SelectedTabViewModel);
            }
        }
        public List<ITabViewModel> TabViewModels { get; set; }

        public AggregateFormationConfig Config { get; set; }
        public static AggregateFormationConfigViewModel AggregateFormationConfigViewModel;
        public static FilmFormationConfigViewModel FilmFormationConfigViewModel;
        public static MainControlViewModel MainControlViewModel;

        public string SimulationName { get; set; } = DateTime.Now.ToString("yyyyMMdd");

        public int NumberOfCPU { get; set; } = Environment.ProcessorCount;

        public ShellViewModel()
        {
            Config = new AggregateFormationConfig();
            AggregateFormationConfigViewModel = new AggregateFormationConfigViewModel(Config);
            FilmFormationConfigViewModel = new FilmFormationConfigViewModel();
            MainControlViewModel = new MainControlViewModel(Config);
            TabViewModels = new List<ITabViewModel>() { MainControlViewModel };
            SelectedTabViewModel = MainControlViewModel;

        }

        public async Task ShowAggConfigMenu(object sender, RoutedEventArgs e)
        {

            SwitchToAggConfigView();
            await Task.Delay(1000);
        }

        public void SwitchToAggConfigView()
        {
            ActivateItem(AggregateFormationConfigViewModel);
        }

        public async Task ShowFilmConfigMenu(object sender, RoutedEventArgs e)
        {

            SwitchToFilmConfigView();
            await Task.Delay(1000);
        }

        public async Task HideAggConfigMenu(object sender, RoutedEventArgs e)
        {
            ActivateItem(null);
            await Task.Delay(1000);
        }

        public void SwitchToFilmConfigView()
        {
            ActivateItem(FilmFormationConfigViewModel);
        }


        public async Task HideFilmConfigMenu(object sender, RoutedEventArgs e)
        {
            ActivateItem(null);
            await Task.Delay(1000);
        }
    }
}
