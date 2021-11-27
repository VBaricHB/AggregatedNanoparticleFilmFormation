
using System;

using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

namespace ANPaX.UI.BlazorServer.Model
{
    public class AggregateConfigurationModelService : IAggregateConfigurationModelService
    {
        private readonly IAggregateConfigurationData _configData;

        public AggregateConfigurationDTO AggregateConfigurationModel { get; set; }
        public AggregateConfigurationModelService(IAggregateConfigurationData configData)
        {
            _configData = configData;
            AggregateConfigurationModel = GetDefaultConfigurationModel();

        }

        private AggregateConfigurationDTO GetDefaultConfigurationModel()
        {
            var output = _configData.GetAggregateConfigurationById(1).Result;
            output.Description = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return output;
        }
    }
}
