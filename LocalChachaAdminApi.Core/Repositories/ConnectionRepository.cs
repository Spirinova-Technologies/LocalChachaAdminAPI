using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Repositories
{
    public class ConnectionRepository
    {
        private readonly IConfiguration configuration;

        public ConnectionRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IDbConnection> OpenConnectionAsync()
        {
            MySqlConnection dbConnection = new MySqlConnection(configuration["ConnectionStrings:DefaultConnection"]);

            await dbConnection.OpenAsync();

            return dbConnection;
        }
    }
}