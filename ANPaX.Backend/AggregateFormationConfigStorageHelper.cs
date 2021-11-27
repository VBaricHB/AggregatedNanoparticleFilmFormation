using System;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Data;
using ANPaX.IO.DTO;

using Microsoft.EntityFrameworkCore;

namespace ANPaX.Backend
{
    public class AggregateFormationConfigStorageHelper : IDataStorageHelper<AggregateConfigurationDTO>
    {
        private readonly DataContext _context;

        public AggregateFormationConfigStorageHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<AggregateConfigurationDTO> SaveIfNotExist(AggregateConfigurationDTO dto)
        {
            var configs = await _context.AggConfigs
                .Where(c => c.Description == dto.Description)
                .Where(c => c.TotalPrimaryParticles == dto.TotalPrimaryParticles)
                .Where(c => c.ClusterSize == dto.ClusterSize)
                .Where(c => Math.Abs(c.Df - dto.Df) < 1e-6)
                .Where(c => Math.Abs(c.Kf - dto.Kf) < 1e-6)
                .Where(c => Math.Abs(c.Epsilon - dto.Epsilon) < 1e-6)
                .Where(c => Math.Abs(c.Delta - dto.Delta) < 1e-6)
                .Where(c => Math.Abs(c.LargeNumber - dto.LargeNumber) < 1e-6)
                .Where(c => c.MaxAttemptsPerAggregate == dto.MaxAttemptsPerAggregate)
                .Where(c => c.MaxAttemptsPerCluster == dto.MaxAttemptsPerCluster)
                .Where(c => c.RadiusMeanCalculationMethod == dto.RadiusMeanCalculationMethod)
                .Where(c => c.AggregateSizeMeanCalculationMethod == dto.AggregateSizeMeanCalculationMethod)
                .Where(c => c.PrimaryParticleSizeDistributionType == dto.PrimaryParticleSizeDistributionType)
                .Where(c => c.AggregateSizeDistributionType == dto.AggregateSizeDistributionType)
                .Where(c => c.AggregateFormationType == dto.AggregateFormationType)
                .Where(c => c.RandomGeneratorSeed == dto.RandomGeneratorSeed)
                .ToListAsync();

            if (configs.Any())
            {
                return configs.First();
            }
            else
            {
                _context.AggConfigs.Add(dto);
                await _context.SaveChangesAsync();
                return dto;
            }
        }
    }
}
