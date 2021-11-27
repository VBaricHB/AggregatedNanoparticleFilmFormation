using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ANPaX.IO.DTO
{
    [Table("PrimaryParticleData")]
    public class PrimaryParticleDTO
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public int ParticleSimulationId { get; set; }
        public int AggregateId { get; set; }
        public int ClusterId { get; set; }
        public int AggregateConfigurationId { get; set; }
        public double XCoord { get; set; }
        public double YCoord { get; set; }
        public double ZCoord { get; set; }
        public double Radius { get; set; }
    }
}
