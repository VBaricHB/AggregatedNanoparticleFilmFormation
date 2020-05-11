namespace ANPaX.FilmFormation
{
    public class BoxDimension
    {
        public double Lower { get; set; }
        public double Upper { get; set; }
        public double Width => Upper - Lower;

        public BoxDimension(double lower, double upper)
        {
            Lower = lower;
            Upper = upper;
        }
    }
}
