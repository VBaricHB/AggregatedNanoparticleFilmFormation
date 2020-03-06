namespace AggregatedNanoparticleFilmFormation
{
    public class BoxDimension
    {
        public double Lower { get; }
        public double Upper { get; set; }

        public BoxDimension(double lower, double upper)
        {
            Lower = lower;
            Upper = upper;
        }
    }
}