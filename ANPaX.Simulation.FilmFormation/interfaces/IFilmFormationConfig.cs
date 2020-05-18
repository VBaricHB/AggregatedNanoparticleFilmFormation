namespace ANPaX.Simulation.FilmFormation.interfaces
{
    public interface IFilmFormationConfig
    {
        public double LargeNumber { get; }
        public double FilmWidthAbsolute { get; }
        public double Delta { get; }
        public int MaxCPU { get; }
    }
}
