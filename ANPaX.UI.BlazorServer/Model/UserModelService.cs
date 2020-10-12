
using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

namespace ANPaX.UI.BlazorServer.Model
{
    public class UserModelService : IUserModelService
    {
        private readonly IUserData _userData;

        public UserModel UserModel { get; set; }
        public UserModelService(IUserData userData)
        {
            _userData = userData;
            UserModel = GetDefaultModel();

        }

        private UserModel GetDefaultModel()
        {
            var output = new UserModel { User = "", EMail = "" };
            return output;
        }
    }
}
