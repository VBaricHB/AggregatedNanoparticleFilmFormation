using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IAggregateConfigurationData
    {
        Task<int> CreateAggregateConfiguration(AggregateConfigurationModel aggregateConfigurationModel);
        Task<AggregateConfigurationModel> GetAggregateConfigurationById(int aggregateConfigurationId);
        Task<List<AggregateConfigurationModel>> GetAggregateConfigurations();
    }
}
