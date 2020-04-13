namespace ANPaX.AggregateFormation.interfaces
{
    public interface IParticleFactory<T>
    {
        T Build(int size);
    }
}
