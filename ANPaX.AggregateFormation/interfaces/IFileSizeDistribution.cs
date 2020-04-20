namespace ANPaX.AggregateFormation.interfaces
{
    public interface IFileSizeDistribution<T>
    {
        public Size<T>[] Sizes { get; set; }
    }
}
