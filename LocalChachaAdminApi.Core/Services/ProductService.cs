using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Core.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<SuggestedItemResult> GetSuggestedItems(SearchFilter filter)
        {
            return await productRepository.GetSuggestedItems(filter);
        }

        public async Task BulkInsertSuggestedItems(List<SuggestedItem> suggestedItems)
        {
            foreach (var suggestedItem in suggestedItems)
            {
                await productRepository.InsertSuggestedProduct(suggestedItem);
            }
        }
    }
}