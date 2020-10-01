using System;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation.tests
{
    internal class TestFilmFormationConfig : IFilmFormationConfig
    {
        public double LargeNumber => 1e10;

        public double Delta => 1.01;

        public double XFilmWidthAbsolute => 2000;
        public double YFilmWidthAbsolute => 2000;

        public int MaxCPU => Environment.ProcessorCount;

        public TestFilmFormationConfig()
        {
            SimulationBoxFactory = new AbsoluteTetragonalSimulationBoxFactory();
            SingleParticleDepositionHandler = new BallisticSingleParticleDepositionHandler();
            AggregateDepositionHandler = new BallisticAggregateDepositionHandler(SingleParticleDepositionHandler);
            WallCollisionHandler = new PeriodicBoundaryCollisionHandler();
            NeighborslistFactory = new AccordNeighborslistFactory();
        }

        public ISimulationBoxFactory SimulationBoxFactory { get; }

        public ISingleParticleDepositionHandler SingleParticleDepositionHandler { get; }

        public IAggregateDepositionHandler AggregateDepositionHandler { get; }

        public IWallCollisionHandler WallCollisionHandler { get; }

        public INeighborslistFactory NeighborslistFactory { get; }
    }
}
