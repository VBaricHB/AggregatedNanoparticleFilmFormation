using ANPaX.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class AggFormationControlViewModel : Screen
    {
        private AggregateFormationConfig _config;
        public AggFormationControlViewModel(AggregateFormationConfig config)
        {
            _config = config;
        }
    }
}
