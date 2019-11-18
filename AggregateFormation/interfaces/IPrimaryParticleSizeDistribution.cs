namespace AggregateFormation.interfaces
{
    public interface IPrimaryParticleSizeDistribution
    {
        double GetRadiusByProbability(double probability);
        double MeanRadius { get; }
    }
}
