using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;

namespace LocalChachaAdminApi.Core.Services
{
    public class UserService : IUserService
    {
        public LoginResponseModel Authenticate(LoginModel userModel)
        {
            if (userModel.Username == "admin" && userModel.Password == "admin@123")
            {
                return new LoginResponseModel
                {
                    Email = "admin@test.com",
                    Id = 1
                };
            }

            return null;
        }
    }
}