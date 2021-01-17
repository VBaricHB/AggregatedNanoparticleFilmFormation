using System.Collections.Generic;

namespace ANPaX.Backend.Models
{
    public class SimulationMonitor
    {
        private List<ISimulationRunner> _runningSimulations;
        public SimulationMonitor()
        {
            _runningSimulations = new List<ISimulationRunner>();
        }

        public void AddRunner(ISimulationRunner runner)
        {
            _runningSimulations.Add(runner);
        }
    }
}
