using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Services
{
    public class BulkInsertService : IBulkInsertService
    {
        private readonly IHttpService httpService;
        private readonly IS3BucketService s3BucketService;

        public BulkInsertService(IS3BucketService s3BucketService, IHttpService httpService)
        {
            this.httpService = httpService;
            this.s3BucketService = s3BucketService;
        }

        public async Task InsertBulkData()
        {
            //first insert multiple Merchants from json
            List<MerchantResponseModel> merchantResponses = await InsertMerchants();

            //Insert catorgories for the Merchants
            foreach (var merchantResponse in merchantResponses)
            {
                var categoriesResponses = await InsertCategories(merchantResponse.Merchant.Id, merchantResponse.Merchant.Token);

                foreach (var categoryResponse in categoriesResponses)
                {
                    //insert products
                    await InsertProducts(merchantResponse.Merchant.Id, merchantResponse.Merchant.Token, categoryResponse.Id.ToString(), categoryResponse.Name);
                }
            }
        }

        private async Task<List<MerchantResponseModel>> InsertMerchants()
        {
            var merchantContent = await s3BucketService.GetS3Object("bulkinsertdata/Merchant.json");
            var merchants = JsonConvert.DeserializeObject<List<MerchantRequestModel>>(merchantContent);

            var merchantResponses = new List<MerchantResponseModel>();
            foreach (var merchant in merchants)
            {
                var merchantResponse = await httpService.GetHttpClientResponse<MerchantResponseModel>("api/merchants/create", merchant, HttpMethod.Post);

                if (merchantResponse.Merchant != null)
                    merchantResponses.Add(merchantResponse);
            };
            return merchantResponses;
        }

        private async Task<List<CategoryResponseModel>> InsertCategories(int merchantId, string token)
        {
            //insert category
            var categoriesContent = await s3BucketService.GetS3Object("bulkinsertdata/Categories.json");
            var categoriesRequest = JsonConvert.DeserializeObject<List<CategoryRequestModel>>(categoriesContent);

            var categoriesResponses = new List<CategoryResponseModel>();
            foreach (var categoryRequest in categoriesRequest)
            {
                categoryRequest.MerchantId = merchantId;

                var categoriesResponse = await httpService.GetHttpClientResponse<CategoryResponseModel>("api/categories/create", categoryRequest, HttpMethod.Post, true, token);

                if (categoriesResponse.Status == 1)
                    categoriesResponses.Add(categoriesResponse);
            }

            return categoriesResponses;
        }

        private async Task InsertProducts(int merchantId, string token, string categoryId, string categoryName)
        {
            //insert category
            var productContents = await s3BucketService.GetS3Object("bulkinsertdata/Products.json");
            var productsRequest = JsonConvert.DeserializeObject<List<Product>>(productContents);

            var productsResponse = new List<ProductResponseModel>();
            foreach (var product in productsRequest.Where(x => x.CategoryName.Trim() == categoryName.Trim()).ToList())
            {
                var productRequestModel = new ProductRequestModel
                {
                    CategoryId = categoryId,
                    MerchantId = merchantId,
                    Description = product.Description,
                    IsActive = product.IsActive,
                    IsPublished = product.IsPublished,
                    MaxQuantity = product.MaxQuantity,
                    Name = product.Name,
                    Price = product.Price
                };

                var productResponse = await httpService.GetHttpClientResponse<ProductResponseModel>("api/products/createProduct", productRequestModel, HttpMethod.Post, true, token);

                if (productResponse.Status == 1)
                    productsResponse.Add(productResponse);
            }
        }
    }
}