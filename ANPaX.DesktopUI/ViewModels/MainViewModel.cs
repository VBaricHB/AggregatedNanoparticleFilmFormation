using System.Threading.Tasks;
using System.Windows;

using ANPaX.DesktopUI.Models;
using ANPaX.DesktopUI.Views;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public AggregateFormationConfig AggregateFormationConfig { get; set; }
        public FilmFormationConfig FilmFormationConfig { get; set; }
        public AggFormationControlViewModel AggFormationControlViewModel { get; set; }
        public FilmAnalysisControlViewModel FilmAnalysisControlViewModel { get; set; }
        public FilmFormationControlViewModel FilmFormationControlViewModel { get; set; }
        public LoggingViewModel LoggingViewModel { get; set; }
        public SimulationProperties SimulationProperties { get; set; }
        public StatusViewModel StatusViewModel { get; set; }


        public MainViewModel()
        {

            AggregateFormationConfig = new AggregateFormationConfig();
            FilmFormationConfig = new FilmFormationConfig();
            SimulationProperties = new SimulationProperties();

            LoggingViewModel = new LoggingViewModel();
            StatusViewModel = new StatusViewModel(LoggingViewModel);

            AggFormationControlViewModel = new AggFormationControlViewModel(
                AggregateFormationConfig,
                SimulationProperties,
                StatusViewModel,
                LoggingViewModel);

            FilmFormationControlViewModel = new FilmFormationControlViewModel(
                FilmFormationConfig,
                SimulationProperties,
                AggFormationControlViewModel,
                StatusViewModel,
                LoggingViewModel);

            FilmAnalysisControlViewModel = new FilmAnalysisControlViewModel();

            ActivateItem(AggFormationControlViewModel);
        }


        public async Task AggFormationButtonClick(MainView view)
        {

            SwitchToHideAggFormationButton(view);
            SwitchToShowFilmAnalysisButton(view);
            SwitchToShowFilmFormationButton(view);

            SwitchToAggFormationControlView();
            await Task.Delay(1000);
        }

        public async Task FilmAnalysisButtonClick(MainView view)
        {
            SwitchToShowAggFormationButton(view);
            SwitchToHideFilmAnalysisButton(view);
            SwitchToShowFilmFormationButton(view);

            SwitchToFilmAnalysisControlView();
            await Task.Delay(1000);

        }

        public async Task FilmFormationButtonClick(MainView view)
        {
            SwitchToShowAggFormationButton(view);
            SwitchToShowFilmAnalysisButton(view);
            SwitchToHideFilmFormationButton(view);

            SwitchToFilmFormationControlView();
            await Task.Delay(1000);
        }

        private void SwitchToAggFormationControlView()
        {
            ActivateItem(AggFormationControlViewModel);
        }

        private void SwitchToFilmAnalysisControlView()
        {
            ActivateItem(FilmAnalysisControlViewModel);
        }

        private void SwitchToFilmFormationControlView()
        {
            ActivateItem(FilmFormationControlViewModel);
        }

        private void SwitchToShowAggFormationButton(MainView view)
        {
            view.AggFormationButton.Visibility = Visibility.Visible;
            view.AggFormationHideButton.Visibility = Visibility.Hidden;
        }

        private void SwitchToHideAggFormationButton(MainView view)
        {
            view.AggFormationButton.Visibility = Visibility.Hidden;
            view.AggFormationHideButton.Visibility = Visibility.Visible;
        }

        private void SwitchToShowFilmFormationButton(MainView view)
        {
            view.FilmFormationButton.Visibility = Visibility.Visible;
            view.FilmFormationHideButton.Visibility = Visibility.Hidden;
        }

        private void SwitchToHideFilmFormationButton(MainView view)
        {
            view.FilmFormationButton.Visibility = Visibility.Hidden;
            view.FilmFormationHideButton.Visibility = Visibility.Visible;
        }

        private void SwitchToShowFilmAnalysisButton(MainView view)
        {
            view.FilmAnalysisButton.Visibility = Visibility.Visible;
            view.FilmAnalysisHideButton.Visibility = Visibility.Hidden;
        }

        private void SwitchToHideFilmAnalysisButton(MainView view)
        {
            view.FilmAnalysisButton.Visibility = Visibility.Hidden;
            view.FilmAnalysisHideButton.Visibility = Visibility.Visible;
        }


    }
}
