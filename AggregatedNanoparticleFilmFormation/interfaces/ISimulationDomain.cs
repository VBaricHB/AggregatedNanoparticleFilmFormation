using System.Collections.Generic;
using Common.interfaces;

namespace DirectDepositionAlgorithm.interfaces
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
