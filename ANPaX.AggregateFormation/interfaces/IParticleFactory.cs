namespace ANPaX.AggregateFormation.interfaces
{
    public interface IParticleFactory<T>
    {
        /// <summary>
        /// Build a cluster/aggregate
        /// </summary>
        /// <param name="size">Target size (number of primary particles)</param>
        /// <returns></returns>
        public T Build(int size);
    }
}
