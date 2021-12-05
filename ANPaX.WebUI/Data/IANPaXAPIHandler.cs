using ANPaX.IO.DTO;

namespace ANPaX.WebUI.Data
{
    public interface IANPaXAPIHandler
    {
        AggregateConfigurationDTO GetAggregateConfigurationById(int id);
    }
}
