namespace ANPaX.AggregateFormation.interfaces
{
    public interface ISizeDistribution<T>
    {
        T GetRandomSize();
        T Mean { get; }
    }
}
