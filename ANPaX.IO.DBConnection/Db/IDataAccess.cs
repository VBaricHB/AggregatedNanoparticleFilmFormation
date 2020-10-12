using System.Collections.Generic;
using System.Threading.Tasks;

namespace ANPaX.IO.DBConnection.Db
{
    public interface IDataAccess
    {
        Task<List<T>> LoadData<T, U>(string procedure, U parameters, ConnectionData connectionData);
        Task<int> SaveData<U>(string procedure, U parameters, ConnectionData connectionData);
    }
}
