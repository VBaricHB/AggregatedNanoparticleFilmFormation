using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IUserData
    {
        Task<int> CreateUser(UserDTO user);
        Task<List<UserDTO>> GetUsers();
    }
}
