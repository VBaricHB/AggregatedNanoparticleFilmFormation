using ANPaX.Core.interfaces;

namespace ANPaX.Simulation.FilmFormation.interfaces
{
    public interface IFilmFormationConfig
    {
        public double LargeNumber { get; }
        public double XFilmWidthAbsolute { get; }
        public double YFilmWidthAbsolute { get; }
        public double Delta { get; }
        public int MaxCPU { get; }
        public ISimulationBoxFactory SimulationBoxFactory { get; }
        public ISingleParticleDepositionHandler SingleParticleDepositionHandler { get; }
        public IAggregateDepositionHandler AggregateDepositionHandler { get; }
        public IWallCollisionHandler WallCollisionHandler { get; }
        public INeighborslistFactory NeighborslistFactory { get; }


    }
}
