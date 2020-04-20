namespace ANPaX.AggregateFormation
{
    public class ProgressReportModel
    {
        public int PercentageComplete { get; set; } = 0;

        public int TotalPrimaryParticles { get; set; } = 0;

        public int TotalAggregates { get; set; } = 0;

        public int PrimaryParticlesLastAggregate { get; set; } = 0;

        public long ProcessingTimeLastAggregate { get; set; } = 0;
    }
}
