using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IFilmFormationConfigurationData
    {
        Task<int> CreateAggregateConfiguration(FilmFormationConfigurationModel filmFormationConfiguration);
        Task<FilmFormationConfigurationModel> GetFilmFormationConfigurationById(int filmFormationConfigurationId);
        Task<List<FilmFormationConfigurationModel>> GetFilmFormationConfigurations();
    }
}
