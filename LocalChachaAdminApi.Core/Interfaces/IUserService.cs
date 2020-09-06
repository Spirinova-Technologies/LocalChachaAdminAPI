using LocalChachaAdminApi.Core.Models;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IUserService
    {
        LoginResponseModel Authenticate(LoginModel userModel);
    }
}