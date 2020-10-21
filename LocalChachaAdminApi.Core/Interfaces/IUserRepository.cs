using LocalChachaAdminApi.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
    }
}