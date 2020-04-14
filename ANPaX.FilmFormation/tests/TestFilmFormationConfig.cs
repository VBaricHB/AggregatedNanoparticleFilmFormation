using ANPaX.FilmFormation.interfaces;

namespace ANPaX.FilmFormation.tests
{
    internal class TestFilmFormationConfig : IFilmFormationConfig
    {
        public double LargeNumber => 1e10;

        public double Delta => 1.01;

        public double FilmWidthAbsolute => 2000;
    }
}
