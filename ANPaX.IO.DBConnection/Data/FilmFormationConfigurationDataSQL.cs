using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Db;
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

using Dapper;

namespace ANPaX.IO.DBConnection.Data
{
    public class FilmFormationConfigurationDataSQL : IFilmFormationConfigurationData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionData _connectionString;

        public FilmFormationConfigurationDataSQL(IDataAccess dataAccess, ConnectionData connectionString)
        {
            _dataAccess = dataAccess;
            _connectionString = connectionString;
        }

        public async Task<FilmFormationConfigurationModel> GetFilmFormationConfigurationById(int filmFormationConfigurationId)
        {
            var recs = await _dataAccess.LoadData<FilmFormationConfigurationModel, dynamic>("dbo.spFilmConfig_GetById",
                                                                                  new { Id = filmFormationConfigurationId },
                                                                                  _connectionString);

            return recs.FirstOrDefault();

        }

        public async Task<int> CreateAggregateConfiguration(FilmFormationConfigurationModel filmFormationConfiguration)
        {
            var p = new DynamicParameters();

            p.Add("Description", filmFormationConfiguration.Description);
            p.Add("XWidth", filmFormationConfiguration.XWidth);
            p.Add("YWidth", filmFormationConfiguration.YWidth);
            p.Add("ZWidth", filmFormationConfiguration.ZWidth);
            p.Add("MaxCPU", filmFormationConfiguration.MaxCPU);
            p.Add("LargeNumber", filmFormationConfiguration.LargeNumber);
            p.Add("Delta", filmFormationConfiguration.Delta);
            p.Add("DepositionMethod", filmFormationConfiguration.DepositionMethod);
            p.Add("WallCollisionMethod", filmFormationConfiguration.WallCollisionMethod);
            p.Add("NeighborslistMethod", filmFormationConfiguration.NeighborslistMethod);
            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.SaveData("dbo.spFilmConfig_Insert", p, _connectionString);

            return p.Get<int>("Id");
        }

        public Task<List<FilmFormationConfigurationModel>> GetFilmFormationConfigurations()
        {
            return _dataAccess.LoadData<FilmFormationConfigurationModel, dynamic>("dbo.spFilmConfig_All",
                                                                          new { },
                                                                          _connectionString);
        }
    }
}
