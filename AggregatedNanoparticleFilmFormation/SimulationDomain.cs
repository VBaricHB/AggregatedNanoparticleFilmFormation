using System.Collections.Generic;
using System.Linq;
using Common.interfaces;
using AggregatedNanoparticleFilmFormation.interfaces;

namespace AggregatedNanoparticleFilmFormation
{
    public class SimulationDomain : ISimulationDomain
    {

        public BoxDimension XLim { get; }
        public BoxDimension YLim { get; }
        public BoxDimension ZLim { get; }

        public List<IAggregate> DepositedAggregates { get; }
        public double CurrentHeight { get; set; }
        public double MaxPrimaryParticleRadius { get; set; }
        public int DepositedPrimaryParticles => DepositedAggregates.Sum(a => a.NumberOfPrimaryParticles);

        public SimulationDomain(double width, double maxPrimaryParticleRadius)
        {
            XLim = new BoxDimension(-width / 2, width / 2);
            YLim = new BoxDimension(-width / 2, width / 2);
            ZLim = new BoxDimension(0, 0);
            DepositedAggregates = new List<IAggregate>();
            CurrentHeight = 0;
            MaxPrimaryParticleRadius = maxPrimaryParticleRadius;
        }
       
    }
}
