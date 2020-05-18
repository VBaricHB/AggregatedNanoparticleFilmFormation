using System;
using System.Collections.Generic;
using System.Linq;

using ANPaX.Core.interfaces;
using ANPaX.Core.ParticleFilm;

namespace ANPaX.Core
{
    public class TetragonalAggregatedParticleFilm : IParticleFilm<Aggregate>
    {
        private readonly INeighborslistFactory _neighborslistFactory;

        public ISimulationBox SimulationBox { get; }
        public double CurrentHeight
        {
            get => SimulationBox.ZDim.Upper;
            set => SimulationBox.ZDim.Upper = value;
        }
        public int NumberOfPrimaryParticles => Particles.Sum(a => a.NumberOfPrimaryParticles);
        public IList<Aggregate> Particles { get; set; }
        public IEnumerable<PrimaryParticle> PrimaryParticles => Particles.GetPrimaryParticles() ?? new List<PrimaryParticle>();
        public INeighborslist Neighborslist2D { get; private set; } = null;

        public TetragonalAggregatedParticleFilm(ISimulationBox simulationBox, INeighborslistFactory neighborslistFactory)
        {
            _neighborslistFactory = neighborslistFactory;
            SimulationBox = simulationBox;
            Particles = new List<Aggregate>();
            CurrentHeight = 0;
        }

        public void AddDepositedParticlesToFilm(Aggregate aggregate)
        {
            Particles.Add(aggregate);
            AddAggregateToNeighborslist(aggregate);
            UpdateFilmHeight(aggregate);
        }

        private void UpdateFilmHeight(Aggregate aggregate)
        {
            CurrentHeight = Math.Max(CurrentHeight, aggregate.GetPrimaryParticles().Select(p => p.Position.Z).Max());
        }

        private void AddAggregateToNeighborslist(Aggregate aggregate)
        {
            if (Neighborslist2D is null)
            {
                Neighborslist2D = _neighborslistFactory.Build2DNeighborslist(aggregate.GetPrimaryParticles());
            }
            else
            {
                Neighborslist2D.AddParticlesToNeighborsList(aggregate);
                Neighborslist2D.AddVirtualParticlePeriodicBoundaries(
                    aggregate,
                    PrimaryParticles.GetMaxRadius(),
                    SimulationBox.XDim,
                    SimulationBox.YDim);
            }
        }



    }
}

