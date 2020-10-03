using LocalChachaAdminApi.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IMerchantService
    {
        Task<List<MerchantRequestModel>> GetSuggestedMerchants();
        Task<List<Merchant>> GetMerchants(SearchFilter filter);
        Task<CommonResponseModel> SaveSuggestedMerchants(List<MerchantRequestModel> merchants);
        Task DeleteMerchants();
    }
}