
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

namespace ANPaX.UI.BlazorServer.Model
{
    public class UserModelService : IUserModelService
    {
        private readonly IUserData _userData;

        public UserDTO UserModel { get; set; }
        public UserModelService(IUserData userData)
        {
            _userData = userData;
            UserModel = GetDefaultModel();

        }

        private UserDTO GetDefaultModel()
        {
            var output = new UserDTO { User = "", EMail = "" };
            return output;
        }
    }
}
