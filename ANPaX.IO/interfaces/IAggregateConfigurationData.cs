using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IAggregateConfigurationData
    {
        Task<int> CreateAggregateConfiguration(AggregateConfigurationDTO aggregateConfigurationModel);
        Task<AggregateConfigurationDTO> GetAggregateConfigurationById(int aggregateConfigurationId);
        Task<List<AggregateConfigurationDTO>> GetAggregateConfigurations();
    }
}
