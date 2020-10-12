using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ANPaX.IO.DTO
{
    public class AggregateConfigurationModel
    {
        public int Id { get; set; }
        public string Description { get; set; }

        [Required]
        [Range(1, 10000000, ErrorMessage = "Number of particles out of range. Please select between 1 and 10,000,000.")]
        [DisplayName("Total number of primary particles")]
        public int TotalPrimaryParticles { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Cluster size out of range. Please select between 1 and 10.")]
        [DisplayName("Primary particles per cluster")]
        public int ClusterSize { get; set; }

        [Required]
        [Range(1.0, 2.0, ErrorMessage = "Model works only for fractal dimensions between 1.0 and 2.0.")]
        [DisplayName("Fractal dimension")]
        public double Df { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Model works only for fractal prefactors between 1.0 and 2.0.")]
        [DisplayName("Fractal prefactor")]
        public double Kf { get; set; }

        [Required]
        [Range(1.0, 1.1, ErrorMessage = "Epsilon has to be greater than 1.0 and lower than 1.1.")]
        [DisplayName("Epsilon : Minimum distance factor of two primary particles")]
        public double Epsilon { get; set; }

        [Required]
        [Range(1.0, 1.1, ErrorMessage = "Delta has to be greater than 1.0 and lower than 1.1.")]
        [DisplayName("Delta : Maximum distance factor of two primary particles")]
        public double Delta { get; set; }

        [Required]
        [Range(0, 10000000, ErrorMessage = "Value must be below 10,000,000.")]
        [DisplayName("Maximum attempts to create a cluster before a restart")]
        public int MaxAttemptsPerCluster { get; set; }

        [Required]
        [Range(0, 10000000, ErrorMessage = "Value must be below 10,000,000.")]
        [DisplayName("Maximum attempts to create a aggregate before a restart")]
        public int MaxAttemptsPerAggregate { get; set; }

        [Required]
        [Range(1000000, 100000000, ErrorMessage = "Value should be above 1,000,000 and below 100,000,000.")]
        [DisplayName("A large number for internal comparison")]
        public double LargeNumber { get; set; }

        [Required]
        [StringRange(AllowableValues = new[] { "Geometric", "Arithmetic", "Sauter" }, ErrorMessage = "Invalid method. Must be 'Arithmetic', 'Geometric' or 'Sauter'.")]
        [DisplayName("Mean radius calculation method")]
        public string RadiusMeanCalculationMethod { get; set; }

        [Required]
        [StringRange(AllowableValues = new[] { "Geometric", "Arithmetic" }, ErrorMessage = "Invalid method. Must be 'Arithmetic' or 'Geometric'.")]
        [DisplayName("Mean aggregate size calculation method")]
        public string AggregateSizeMeanCalculationMethod { get; set; }

        [Required]
        [StringRange(AllowableValues = new[] { "DissDefault", "LogNormal", "Normal", "Monodisperse" }, ErrorMessage = "Invalid distribution. Must be 'DissDefault', 'LogNormal', 'Normal' or 'Monodisperse'.")]
        [DisplayName("Primary particle size distribution")]
        public string PrimaryParticleSizeDistribution { get; set; }

        [Required]
        [StringRange(AllowableValues = new[] { "DissDefault", "LogNormal", "Normal", "Monodisperse" }, ErrorMessage = "Invalid distribution. Must be 'DissDefault', 'LogNormal', 'Normal' or 'Monodisperse'.")]
        [DisplayName("Aggregate size distribution")]
        public string AggregateSizeDistribution { get; set; }

        [Required]
        [StringRange(AllowableValues = new[] { "CCA", "PCA" }, ErrorMessage = "Invalid Method. Must be 'CCA' (Cluster-Cluster Aggregation) or 'PCA' (Particle-Cluster Aggregation).")]
        [DisplayName("Aggregate formation method")]
        public string AggregateFormationFactory { get; set; }

        [Required]
        [Range(-1, 100000000, ErrorMessage = "Invalid seed. Must be -1 (disable custom seed) or below 100,000,000.")]
        public double RandomGeneratorSeed { get; set; }
    }

    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowableValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (AllowableValues?.Contains(value?.ToString()) == true)
            {
                return ValidationResult.Success;
            }

            var msg = $"Please enter one of the allowable values: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
            return new ValidationResult(msg);
        }
    }
}
