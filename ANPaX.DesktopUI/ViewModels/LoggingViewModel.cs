using System;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class LoggingViewModel : Screen
    {
        private string _simulationLog = "";
        public string SimulationLog
        {
            get => _simulationLog;
            set
            {
                _simulationLog = value;
                NotifyOfPropertyChange(() => SimulationLog);
            }
        }

        public LoggingViewModel()
        {
            Clear();
        }


        public void LogInfo(string message)
        {
            SimulationLog += $"{DateTime.Now.ToLongTimeString()}: {message}{Environment.NewLine}";
        }

        public void LogWarning(string message)
        {
            LogInfo($" -WARNING- {message}");
        }

        public void Clear()
        {
            SimulationLog = "";
        }
    }
}
