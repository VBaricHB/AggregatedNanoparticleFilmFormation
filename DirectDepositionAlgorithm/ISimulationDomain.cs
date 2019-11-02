using System.Collections.Generic;

namespace DirectDepositionAlgorithm
{
    public interface ISimulationDomain
    {
        BoxDimension XLim { get; }
        BoxDimension YLim { get; }
        BoxDimension ZLim { get; }
        List<IAggregate> DepositedAggregates { get; }
        int DepositedPrimaryParticles { get; }
        double MaxPrimaryParticleRadius { get; set; }
    }
}
