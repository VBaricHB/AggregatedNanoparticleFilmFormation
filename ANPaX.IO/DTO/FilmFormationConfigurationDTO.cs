using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ANPaX.IO.DTO
{
    [Table("FilmFormationConfiguration")]
    public class FilmFormationConfigurationDTO
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Description { get; set; }
        public double XWidth { get; set; }
        public double YWidth { get; set; }
        public double ZWidth { get; set; }
        public int MaxCPU { get; set; }
        public double LargeNumber { get; set; }
        public double Delta { get; set; }
        public string DepositionMethod { get; set; }
        public string WallCollisionMethod { get; set; }
        public string NeighborslistMethod { get; set; }
    }
}
