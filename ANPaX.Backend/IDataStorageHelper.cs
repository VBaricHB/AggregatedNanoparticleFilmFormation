using System.Threading.Tasks;

namespace ANPaX.Backend
{
    public interface IDataStorageHelper<T>
    {
        Task<int> SaveIfNotExist(T dto);
    }
}
