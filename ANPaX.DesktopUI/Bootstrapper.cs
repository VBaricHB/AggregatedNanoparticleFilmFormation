using System.Windows;

using ANPaX.DesktopUI.ViewModels;

using Caliburn.Micro;

namespace ANPaX.DesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
