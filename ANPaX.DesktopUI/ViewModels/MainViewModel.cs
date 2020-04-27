using System.Threading.Tasks;
using System.Windows;

using ANPaX.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class MainViewModel : Conductor<object>
    {

        public AggregateFormationConfig AggregateFormationConfig { get; set; }
        public AggFormationControlViewModel AggFormationControlViewModel { get; set; }
        public FilmAnalysisControlViewModel FilmAnalysisControlViewModel { get; set; }
        public FilmFormationControlViewModel FilmFormationControlViewModel { get; set; }
        public LoggingViewModel LoggingViewModel { get; set; }
        public StatusViewModel StatusViewModel { get; set; }

        public MainViewModel()
        {
            AggregateFormationConfig = new AggregateFormationConfig();
            AggFormationControlViewModel = new AggFormationControlViewModel(AggregateFormationConfig);
            FilmAnalysisControlViewModel = new FilmAnalysisControlViewModel();
            FilmFormationControlViewModel = new FilmFormationControlViewModel();
            LoggingViewModel = new LoggingViewModel();
            StatusViewModel = new StatusViewModel();
            ActivateItem(AggFormationControlViewModel);
        }

        public async Task AggFormationButton(object sender, RoutedEventArgs e)
        {

            SwitchToAggFormationControlView();
            await Task.Delay(1000);
        }

        public async Task AggFormationHideButton(object sender, RoutedEventArgs e)
        {

            SwitchToAggFormationControlView();
            await Task.Delay(1000);
        }


        public async Task FilmAnalysisButton(object sender, RoutedEventArgs e)
        {

            SwitchToFilmAnalysisControlView();
            await Task.Delay(1000);
        }

        public async Task FilmFormationButton(object sender, RoutedEventArgs e)
        {

            SwitchToFilmFormationControlView();
            await Task.Delay(1000);
        }

        public void SwitchToAggFormationControlView()
        {
            ActivateItem(AggFormationControlViewModel);
        }

        public void SwitchToFilmAnalysisControlView()
        {
            ActivateItem(FilmAnalysisControlViewModel);
        }

        public void SwitchToFilmFormationControlView()
        {
            ActivateItem(FilmFormationControlViewModel);
        }


    }
}
