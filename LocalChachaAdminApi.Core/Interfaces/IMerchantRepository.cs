using LocalChachaAdminApi.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IMerchantRepository
    {
        Task DeleteMerchants();

        Task<List<Merchant>> GetMerchants();
    }
}