using LocalChachaAdminApi.Interfaces;
using LocalChachaAdminApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Services
{
    public class BulkInsertService : IBulkInsertService
    {
        private HttpClient httpClient;
        private readonly IS3BucketService s3BucketService;

        public BulkInsertService(IHttpClientFactory httpClientFactory, IS3BucketService s3BucketService)
        {
            httpClient = httpClientFactory.CreateClient("localChacha");
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
                var merchantResponse = await GetHttpClientResponse<MerchantResponseModel>("api/merchants/create", merchant, HttpMethod.Post);

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

                var categoriesResponse = await GetHttpClientResponse<CategoryResponseModel>("api/categories/create", categoryRequest, HttpMethod.Post, true, token);

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

                var productResponse = await GetHttpClientResponse<ProductResponseModel>("api/products/createProduct", productRequestModel, HttpMethod.Post, true, token);

                if (productResponse.Status == 1)
                    productsResponse.Add(productResponse);
            }
        }

        private async Task<T> GetHttpClientResponse<T>(string path, object request, HttpMethod httpMethod, bool includeToken = false, string token = "")
        {
            var httpRequestMessage = new HttpRequestMessage(httpMethod, path)
            {
                Content = new StringContent(JsonConvert.SerializeObject(request),
                                    Encoding.UTF8,
                                    "application/json")
            };

            if (includeToken)
            {
                httpRequestMessage.Headers.Authorization =
                      new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await httpClient.SendAsync(httpRequestMessage);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}