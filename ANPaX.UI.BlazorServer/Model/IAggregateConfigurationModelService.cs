using ANPaX.IO.DTO;

namespace ANPaX.UI.BlazorServer.Model
{
    public interface IAggregateConfigurationModelService
    {
        AggregateConfigurationDTO AggregateConfigurationModel { get; set; }
    }
}
