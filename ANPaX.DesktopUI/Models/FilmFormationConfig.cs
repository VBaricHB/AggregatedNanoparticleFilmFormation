using ANPaX.FilmFormation.interfaces;

namespace ANPaX.DesktopUI.Models
{
    public class FilmFormationConfig : IFilmFormationConfig
    {
        public double LargeNumber { get; set; } = 1e10;

        public double FilmWidthAbsolute { get; set; } = 200;

        public double Delta { get; set; } = 1.01;
    }
}
