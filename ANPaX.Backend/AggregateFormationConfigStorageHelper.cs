using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

namespace ANPaX.Backend
{
    public class AggregateFormationConfigStorageHelper : IDataStorageHelper<AggregateConfigurationDTO>
    {
        private readonly IAggregateConfigurationData _aggregateConfigurationData;

        public AggregateFormationConfigStorageHelper(IAggregateConfigurationData aggregateConfigurationData)
        {
            _aggregateConfigurationData = aggregateConfigurationData;
        }

        public async Task<int> SaveIfNotExist(AggregateConfigurationDTO dto)
        {
            var aggConfigs = await _aggregateConfigurationData.GetAggregateConfigurations();
            int id;
            var match = aggConfigs.FirstOrDefault(a => a.Equals(dto));
            if (match != null)
            {
                id = match.Id;
            }
            else
            {
                id = await _aggregateConfigurationData.CreateAggregateConfiguration(dto);
            }

            return id;
        }

    }
}
