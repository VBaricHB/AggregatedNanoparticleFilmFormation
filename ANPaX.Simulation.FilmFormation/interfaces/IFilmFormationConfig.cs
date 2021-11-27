using ANPaX.Core.interfaces;

namespace ANPaX.Simulation.FilmFormation.interfaces
{
    public interface IFilmFormationConfig
    {
        public double LargeNumber { get; set; }
        public double XFilmWidthAbsolute { get; set; }
        public double YFilmWidthAbsolute { get; set; }
        public double Delta { get; set; }
        public int MaxCPU { get; set; }
        public ISimulationBoxFactory SimulationBoxFactory { get; set; }
        public ISingleParticleDepositionHandler SingleParticleDepositionHandler { get; set; }
        public IAggregateDepositionHandler AggregateDepositionHandler { get; set; }
        public IWallCollisionHandler WallCollisionHandler { get; set; }
        public INeighborslistFactory NeighborslistFactory { get; set; }
    }
}
