using Dapper;
using LocalChachaAdminApi.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Repositories
{
    public class MerchantRepository : ConnectionRepository, IMerchantRepository
    {
        public MerchantRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task DeleteMerchants()
        {
            var sqlQuery = @"Truncate table merchants;
                             Truncate table categories;
                             Truncate table products;
                             Truncate table settings";

            using (IDbConnection connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(sqlQuery);
            }
        }
    }
}