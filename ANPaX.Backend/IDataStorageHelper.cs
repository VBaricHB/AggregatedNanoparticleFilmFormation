using System.Threading.Tasks;

namespace ANPaX.Backend
{
    public interface IDataStorageHelper<T>
    {
        Task<T> SaveIfNotExist(T dto);
    }
}
