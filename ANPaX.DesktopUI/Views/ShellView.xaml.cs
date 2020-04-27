using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ANPaX.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private void ShowAggConfigMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbShowLeftMenu", ShowAggConfigMenu, HideAggConfigMenu, AggConfigViewPanel);
            ShowFilmConfigMenu.Visibility = Visibility.Visible;
            HideFilmConfigMenu.Visibility = Visibility.Hidden;
        }

        private void HideAggConfigMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideLeftMenu", ShowAggConfigMenu, HideAggConfigMenu, AggConfigViewPanel);
        }

        private void ShowFilmConfigMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbShowLeftMenu", ShowFilmConfigMenu, HideFilmConfigMenu, AggConfigViewPanel);
            ShowAggConfigMenu.Visibility = Visibility.Visible;
            HideAggConfigMenu.Visibility = Visibility.Hidden;
        }

        private void HideFilmConfigMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideLeftMenu", ShowFilmConfigMenu, HideFilmConfigMenu, AggConfigViewPanel);
        }

        private void ShowHideMenu(string storyboard, Button showButton, Button hideButton, StackPanel pnl)
        {
            var sb = Application.Current.Resources[storyboard] as Storyboard;
            sb.Begin(pnl);

            if (storyboard.Contains("Show"))
            {
                hideButton.Visibility = Visibility.Visible;
                showButton.Visibility = Visibility.Hidden;
            }
            else if (storyboard.Contains("Hide"))
            {
                hideButton.Visibility = Visibility.Hidden;
                showButton.Visibility = Visibility.Visible;
            }
        }
    }
}
