using System.Windows;

namespace ANPaX.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void AggFormationButton_Click(object sender, RoutedEventArgs e)
        {
            AggFormationButton.Visibility = Visibility.Hidden;
            AggFormationHideButton.Visibility = Visibility.Visible;

            FilmAnalysisButton.Visibility = Visibility.Visible;
            FilmAnalysisHideButton.Visibility = Visibility.Hidden;

            FilmFormationButton.Visibility = Visibility.Visible;
            FilmFormationHideButton.Visibility = Visibility.Hidden;
        }

        private void FilmFormationButton_Click(object sender, RoutedEventArgs e)
        {
            AggFormationButton.Visibility = Visibility.Visible;
            AggFormationHideButton.Visibility = Visibility.Hidden;

            FilmAnalysisButton.Visibility = Visibility.Visible;
            FilmAnalysisHideButton.Visibility = Visibility.Hidden;

            FilmFormationButton.Visibility = Visibility.Hidden;
            FilmFormationHideButton.Visibility = Visibility.Visible;
        }

        private void FilmAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            AggFormationButton.Visibility = Visibility.Visible;
            AggFormationHideButton.Visibility = Visibility.Hidden;

            FilmAnalysisButton.Visibility = Visibility.Hidden;
            FilmAnalysisHideButton.Visibility = Visibility.Visible;

            FilmFormationButton.Visibility = Visibility.Visible;
            FilmFormationHideButton.Visibility = Visibility.Hidden;
        }
    }
}
