using Dapper;
using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Repositories
{
    public class UserRepository : ConnectionRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<User>> GetAllUsers()
        {
            var sqlQuery = "select * from users";

            using (IDbConnection connection = await OpenConnectionAsync())
            {
                var result = await connection.QueryAsync<User>(sqlQuery);

                return result.ToList();
            }
        }
    }
}