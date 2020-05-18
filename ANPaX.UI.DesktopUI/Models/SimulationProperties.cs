using System;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI.Models
{
    public class SimulationProperties : PropertyChangedBase
    {

        private string _simulatonPath = "<Select path>";
        public string SimulationName { get; set; }
        public string SimulationPath
        {
            get => _simulatonPath;
            set
            {
                _simulatonPath = value;
                NotifyOfPropertyChange(() => SimulationPath);
            }
        }
        public int NumberOfCPU { get; set; }

        public SimulationProperties()
        {
            NumberOfCPU = Environment.ProcessorCount;
            SimulationName = DateTime.Now.ToString("yyyyMMdd");
        }
    }
}
