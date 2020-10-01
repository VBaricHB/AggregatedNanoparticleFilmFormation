using System;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.UI.DesktopUI.Models
{
    public class FilmFormationConfig : IFilmFormationConfig
    {
        public double LargeNumber { get; set; } = 1e10;

        public double XFilmWidthAbsolute { get; set; } = 2000;
        public double YFilmWidthAbsolute { get; set; } = 2000;

        public double Delta { get; set; } = 1.01;

        public int MaxCPU { get; set; } = Environment.ProcessorCount;

        public FilmFormationConfig()
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
