using System.Collections.Generic;

using ANPaX.Collection;
using ANPaX.Extensions;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class AggregateInformationViewModel : Screen
    {
        public List<Aggregate> Aggregates { get; set; } = new List<Aggregate>();
        public int CurrentAggregateCount { get; set; } = 0;
        public int CurrentPrimaryParticleCount { get; set; } = 0;

        public void UpdateAggregates(List<Aggregate> aggregates)
        {
            Aggregates = new List<Aggregate>();
            Aggregates.AddRange(aggregates);

            CurrentAggregateCount = Aggregates.Count;
            CurrentPrimaryParticleCount = Aggregates.GetNumberOfPrimaryParticles();

            NotifyOfPropertyChange(() => Aggregates);
            NotifyOfPropertyChange(() => CurrentAggregateCount);
            NotifyOfPropertyChange(() => CurrentPrimaryParticleCount);
        }



    }
}
