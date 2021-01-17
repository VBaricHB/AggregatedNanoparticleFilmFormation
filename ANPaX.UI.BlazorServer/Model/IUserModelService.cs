using ANPaX.IO.DTO;

namespace ANPaX.UI.BlazorServer.Model
{
    public interface IUserModelService
    {
        UserDTO UserModel { get; set; }
    }
}