namespace ANPaX.Core
{
    public class ProgressReportModel
    {
        public int PercentageComplete { get; set; } = 0;

        public int TotalPrimaryParticles { get; set; } = 0;
        public int CumulatedAggregates { get; set; } = 0;
        public int TotalAggregates { get; set; } = 0;

        public int PrimaryParticlesLastAggregate { get; set; } = 0;
        public int CumulatedPrimaryParticles { get; set; } = 0;

        public long SimulationTime { get; set; } = 0;
    }
}
