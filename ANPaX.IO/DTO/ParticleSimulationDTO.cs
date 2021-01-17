using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ANPaX.IO.DTO
{
    [Table("ParticleSimulationData")]
    public class ParticleSimulationDTO
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string SimulationType { get; set; }
        public int AggregateFormationConfigId { get; set; }
        public int FilmFormationConfigId { get; set; }
        public string Status { get; set; }
        public double Percentage { get; set; }
    }
}
