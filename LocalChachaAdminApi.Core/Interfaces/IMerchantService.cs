using LocalChachaAdminApi.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IMerchantService
    {
        Task<List<MerchantRequestModel>> GetMerchants();
        Task<CommonResponseModel> SaveMerchants(List<MerchantRequestModel> merchants);
    }
}