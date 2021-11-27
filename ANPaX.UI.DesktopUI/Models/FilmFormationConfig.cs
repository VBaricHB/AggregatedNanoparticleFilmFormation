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

        public ISimulationBoxFactory SimulationBoxFactory { get; set; }

        public ISingleParticleDepositionHandler SingleParticleDepositionHandler { get; set; }

        public IAggregateDepositionHandler AggregateDepositionHandler { get; set; }

        public IWallCollisionHandler WallCollisionHandler { get; set; }

        public INeighborslistFactory NeighborslistFactory { get; set; }
    }

    public enum WallCollisionType
    {
        Periodic,
        Open
    }
}
