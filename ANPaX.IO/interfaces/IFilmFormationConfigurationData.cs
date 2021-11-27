using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IFilmFormationConfigurationData
    {
        Task<int> CreateFilmFormationConfiguration(FilmFormationConfigurationDTO filmFormationConfiguration);
        Task<FilmFormationConfigurationDTO> GetFilmFormationConfigurationById(int filmFormationConfigurationId);
        Task<List<FilmFormationConfigurationDTO>> GetFilmFormationConfigurations();
    }
}
