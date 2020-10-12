using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IUserData
    {
        Task<int> CreateUser(UserModel user);
        Task<List<UserModel>> GetUsers();
    }
}
