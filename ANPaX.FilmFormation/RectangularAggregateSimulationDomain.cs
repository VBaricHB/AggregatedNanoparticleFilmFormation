using System.Collections.Generic;
using System.Linq;
using ANPaX.Collection;
using ANPaX.FilmFormation.interfaces;

namespace ANPaX.FilmFormation
{
    internal class RectangularAggregateSimulationDomain : ISimulationDomain<Aggregate>
    {
        public ISimulationBox SimulationBox { get; }
        public List<Aggregate> DepositedAggregates { get; }
        public double CurrentHeight { get; set; }
        public double MaxPrimaryParticleRadius { get; set; }
        public int NumberOfPrimaryParticles => DepositedAggregates.Sum(a => a.NumberOfPrimaryParticles);
        public IEnumerable<PrimaryParticle> PrimaryParticles => DepositedAggregates.SelectMany(a => a.Cluster.SelectMany(c => c.PrimaryParticles));

        public RectangularAggregateSimulationDomain(ISimulationBox simulationBox, double maxPrimaryParticleRadius)
        {
            SimulationBox = simulationBox;
            DepositedAggregates = new List<Aggregate>();
            CurrentHeight = 0;
            MaxPrimaryParticleRadius = maxPrimaryParticleRadius;
        }

        public void AddDepositedParticlesToDomain(Aggregate aggregate)
        {
            DepositedAggregates.Add(aggregate);
        }

    }
}
