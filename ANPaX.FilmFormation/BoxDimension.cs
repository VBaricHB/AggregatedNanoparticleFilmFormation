namespace ANPaX.FilmFormation
{
    public class BoxDimension
    {
        public double Lower { get; }
        public double Upper { get; }
        public double Width => Upper - Lower;

        public BoxDimension(double lower, double upper)
        {
            Lower = lower;
            Upper = upper;
        }
    }
}
