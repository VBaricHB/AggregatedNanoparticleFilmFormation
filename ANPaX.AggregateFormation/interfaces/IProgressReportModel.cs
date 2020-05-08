namespace ANPaX.AggregateFormation.interfaces
{
    public interface IProgressReportModel
    {
        int PercentageComplete { get; set; }
        int PrimaryParticlesLastAggregate { get; set; }
        long ProcessingTimeLastAggregate { get; set; }
        int TotalAggregates { get; set; }
        int TotalPrimaryParticles { get; set; }
    }
}
