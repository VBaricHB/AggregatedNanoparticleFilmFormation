using System;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation.tests
{
    internal class TestFilmFormationConfig : IFilmFormationConfig
    {
        public double LargeNumber { get; set; } = 1e10;

        public double Delta { get; set; } = 1.01;

        public double XFilmWidthAbsolute { get; set; } = 2000;
        public double YFilmWidthAbsolute { get; set; } = 2000;

        public int MaxCPU { get; set; } = Environment.ProcessorCount;

        public TestFilmFormationConfig()
        {
            SimulationBoxFactory = new AbsoluteTetragonalSimulationBoxFactory();
            SingleParticleDepositionHandler = new BallisticSingleParticleDepositionHandler();
            AggregateDepositionHandler = new BallisticAggregateDepositionHandler(SingleParticleDepositionHandler);
            WallCollisionHandler = new PeriodicBoundaryCollisionHandler();
            NeighborslistFactory = new AccordNeighborslistFactory();
        }

        public ISimulationBoxFactory SimulationBoxFactory { get; set; }

        public ISingleParticleDepositionHandler SingleParticleDepositionHandler { get; set; }

        public IAggregateDepositionHandler AggregateDepositionHandler { get; set; }

        public IWallCollisionHandler WallCollisionHandler { get; set; }

        public INeighborslistFactory NeighborslistFactory { get; set; }
    }
}
