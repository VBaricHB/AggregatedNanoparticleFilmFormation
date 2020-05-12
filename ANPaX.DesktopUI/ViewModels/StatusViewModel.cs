using System;

using ANPaX.AggregateFormation;
using ANPaX.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class StatusViewModel : Screen
    {

        private SimulationStatus _simulationStatus;
        private int _currentProgress;

        public int CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
                NotifyOfPropertyChange(() => CurrentProgress);
            }
        }

        private string _aggregatesDoneString;

        public string AggregatesDoneString
        {
            get => _aggregatesDoneString;
            set
            {
                _aggregatesDoneString = value;
                NotifyOfPropertyChange(() => AggregatesDoneString);
            }
        }

        private string _primaryParticlesDoneString;

        public string PrimaryParticlesDoneString
        {
            get => _primaryParticlesDoneString;
            set
            {
                _primaryParticlesDoneString = value;
                NotifyOfPropertyChange(() => PrimaryParticlesDoneString);
            }
        }

        private string _simulationTimeString;

        public string SimulationTimeString
        {
            get => _simulationTimeString;
            set
            {
                _simulationTimeString = value;
                NotifyOfPropertyChange(() => SimulationTimeString);
            }
        }

        private string _remainingTimeString;

        public string RemainingTimeString
        {
            get => _remainingTimeString;
            set
            {
                _remainingTimeString = value;
                NotifyOfPropertyChange(() => RemainingTimeString);
            }
        }



        public string SimulationStatusString { get; set; }

        public SimulationStatus SimulationStatus
        {
            get => _simulationStatus;
            set
            {
                _simulationStatus = value;
                SetSimulationStatus();
                NotifyOfPropertyChange(() => SimulationStatus);
                NotifyOfPropertyChange(() => SimulationStatusString);
            }
        }

        public LoggingViewModel LoggingViewModel { get; set; }

        public StatusViewModel(LoggingViewModel loggingViewModel)
        {
            SimulationStatus = SimulationStatus.Idle;
            LoggingViewModel = loggingViewModel;
            CurrentProgress = 0;
        }

        public void SetSimulationStatus()
        {
            switch (SimulationStatus)
            {
                case SimulationStatus.Idle:
                    SimulationStatusString = "Simulation idle";
                    break;
                case SimulationStatus.Running:
                    SimulationStatusString = "Simulation running";
                    break;
                case SimulationStatus.Finished:
                    SimulationStatusString = "Simulation finished";
                    break;
                case SimulationStatus.Canceling:
                    SimulationStatusString = "canceling";
                    break;
                case SimulationStatus.Canceled:
                    SimulationStatusString = "Simulation canceled";
                    break;
            };
        }

        public void ReportProgress(object sender, ProgressReportModel e)
        {
            CurrentProgress = e.PercentageComplete;
            AggregatesDoneString = $"{e.CumulatedAggregates} / {e.TotalAggregates}";
            PrimaryParticlesDoneString = $"{e.CumulatedPrimaryParticles} / {e.TotalPrimaryParticles}";
            SimulationTimeString = TimeSpan.FromMilliseconds(e.SimulationTime).ToString(@"hh\:mm\:ss");
            var timePerAggregate = e.SimulationTime / e.CumulatedAggregates;
            var remainingTime = (e.TotalAggregates - e.CumulatedAggregates) * timePerAggregate;
            RemainingTimeString = TimeSpan.FromMilliseconds(remainingTime).ToString(@"hh\:mm\:ss");
        }

        public void ReportFilmProgress(object sender, ProgressReportModel e)
        {
            CurrentProgress = e.PercentageComplete;
            AggregatesDoneString = $"{e.CumulatedAggregates} / {e.TotalAggregates}";
            PrimaryParticlesDoneString = $"{e.CumulatedPrimaryParticles} / {e.TotalPrimaryParticles}";
            SimulationTimeString = TimeSpan.FromMilliseconds(e.SimulationTime).ToString(@"hh\:mm\:ss");
            var timePerAggregate = e.SimulationTime / e.CumulatedAggregates;
            var remainingTime = (e.TotalAggregates - e.CumulatedAggregates) * timePerAggregate;
            RemainingTimeString = TimeSpan.FromMilliseconds(remainingTime).ToString(@"hh\:mm\:ss");
        }
    }
}
