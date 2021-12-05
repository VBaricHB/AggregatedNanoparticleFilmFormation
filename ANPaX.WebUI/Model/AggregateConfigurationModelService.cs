using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;
using ANPaX.WebUI.Data;

namespace ANPaX.WebUI.Model
{
    public class AggregateConfigurationModelService : IAggregateConfigurationModelService
    {
        private readonly IANPaXAPIHandler _apiHandler;

        public AggregateConfigurationDTO AggregateConfigurationModel { get; set; }
        public AggregateConfigurationModelService(IANPaXAPIHandler apiHandler)
        {
            _apiHandler = apiHandler;
            AggregateConfigurationModel = GetDefaultConfigurationModel();

        }

        private AggregateConfigurationDTO GetDefaultConfigurationModel()
        {
            return _apiHandler.GetAggregateConfigurationById(1);
        }
    }
}
