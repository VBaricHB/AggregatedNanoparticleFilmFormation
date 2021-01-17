
using System;

using ANPaX.Backend.Models;
using ANPaX.IO.DTO;
using ANPaX.Simulation.AggregateFormation;
using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.Backend.Mapper
{
    public static class AggregateConfigurationMapper
    {
        public static IAggregateFormationConfig MapDTOtoAggregateFormationConfiguration(AggregateConfigurationDTO dto)
        {
            var config = new AggregateFormationConfig
            {
                TotalPrimaryParticles = dto.TotalPrimaryParticles,
                ClusterSize = dto.ClusterSize,
                Df = dto.Df,
                Kf = dto.Kf,
                Epsilon = dto.Epsilon,
                Delta = dto.Delta,
                MaxAttemptsPerCluster = dto.MaxAttemptsPerCluster,
                MaxAttemptsPerAggregate = dto.MaxAttemptsPerAggregate,
                LargeNumber = dto.LargeNumber,
                RandomGeneratorSeed = Convert.ToInt32(dto.RandomGeneratorSeed),
                RadiusMeanCalculationMethod = GetRadiusMeanCalculationMethod(dto.RadiusMeanCalculationMethod),
                AggregateSizeMeanCalculationMethod = GetAggregateSizeMeanCalculationMethod(dto.AggregateSizeMeanCalculationMethod),
                AggregateFormationType = GetAggregateFormationType(dto.AggregateFormationType),
                AggregateSizeDistributionType = GetSizeDistributionType(dto.AggregateSizeDistributionType),
                PrimaryParticleSizeDistributionType = GetSizeDistributionType(dto.PrimaryParticleSizeDistributionType)
            };
            return config;
        }

        private static SizeDistributionType GetSizeDistributionType(string sizeDistributionType)
        {
            var success = Enum.TryParse(typeof(SizeDistributionType), sizeDistributionType, out var result);
            if (!success)
            {
                throw new ArgumentException($"Invalid SizeDistributionType {sizeDistributionType}");
            }
            return (SizeDistributionType)result;
        }

        private static AggregateFormationType GetAggregateFormationType(string aggregateFormationType)
        {
            var success = Enum.TryParse(typeof(AggregateFormationType), aggregateFormationType, out var result);
            if (!success)
            {
                throw new ArgumentException($"Invalid AggregateFormationType {aggregateFormationType}");
            }
            return (AggregateFormationType)result;
        }

        private static MeanMethod GetAggregateSizeMeanCalculationMethod(string aggregateSizeMeanCalculationMethod)
        {
            var success = Enum.TryParse(typeof(MeanMethod), aggregateSizeMeanCalculationMethod, out var result);
            if (!success)
            {
                throw new ArgumentException($"Invalid MeanMethod {aggregateSizeMeanCalculationMethod}");
            }
            return (MeanMethod)result;
        }

        private static MeanMethod GetRadiusMeanCalculationMethod(string radiusMeanCalculationMethod)
        {
            var success = Enum.TryParse(typeof(MeanMethod), radiusMeanCalculationMethod, out var result);
            if (!success)
            {
                throw new ArgumentException($"Invalid MeanMethod {radiusMeanCalculationMethod}");
            }
            return (MeanMethod)result;
        }
    }
}
