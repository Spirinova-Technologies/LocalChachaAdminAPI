using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Core.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IProductService
    {
        Task<SuggestedItemResult> GetSuggestedItems(SearchFilter searchFilter);
        Task BulkInsertSuggestedItems(List<SuggestedItem> suggestedItems);
    }
}