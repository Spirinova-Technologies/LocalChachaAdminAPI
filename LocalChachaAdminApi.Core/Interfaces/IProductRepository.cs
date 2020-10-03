using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Core.Models.DTO;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<SuggestedItemResult> GetSuggestedItems(SearchFilter filter);

        Task InsertSuggestedProduct(SuggestedItem suggestedItem);
    }
}