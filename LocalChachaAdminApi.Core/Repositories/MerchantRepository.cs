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
                             Truncate table settings;
                             Truncate table orders;
                             Truncate table orderdetails;
                             Truncate table carts;
                             Truncate table cartitems;";

            using (IDbConnection connection = await OpenConnectionAsync())
            {
                await connection.ExecuteAsync(sqlQuery);
            }
        }

        public async Task<List<Merchant>> GetMerchants()
        {
            var sqlQuery = @"select * from merchants";

            using (IDbConnection connection = await OpenConnectionAsync())
            {
                var result = await connection.QueryAsync<Merchant>(sqlQuery);

                return result.ToList();
            }
        }
    }
}