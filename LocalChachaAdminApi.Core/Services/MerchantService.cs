using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Services
{
    public class MerchantService : IMerchantService
    {
        private readonly IHttpService httpService;
        private readonly IS3BucketService s3BucketService;
        private readonly IMerchantRepository merchantRepository;

        private static string Prefix = "merchants";

        public MerchantService(IS3BucketService s3BucketService, IHttpService httpService, IMerchantRepository merchantRepository)
        {
            this.httpService = httpService;
            this.s3BucketService = s3BucketService;
            this.merchantRepository = merchantRepository;
        }

        public async Task<List<MerchantRequestModel>> GetMerchants()
        {
            var merchantContent = await s3BucketService.GetS3Object(Constants.MerchantsS3Path);
            var merchants = JsonConvert.DeserializeObject<List<MerchantRequestModel>>(merchantContent);

            //now get all the categoris data as well;
            //foreach (var merchant in merchants)
            //{
            //    merchant.Categories = await s3BucketService.GetS3Object($"{Prefix}/{merchant.CategoryFileName}");
            //    merchant.Products = await s3BucketService.GetS3Object($"{Prefix}/{merchant.ProductFileName}");
            //    merchant.Settings = await s3BucketService.GetS3Object($"{Prefix}/{merchant.MerchantSettingsFileName}");
            //}

            return merchants;
        }

        public async Task<CommonResponseModel> SaveMerchants(List<MerchantRequestModel> merchants)
        {
            var responseModel = new CommonResponseModel();
            foreach (var merchant in merchants)
            {
                var merchantResponse = await InsertMerchants(merchant);

                if (merchantResponse.Status == 1)
                {
                    var categoriesResponses = await InsertCategories(merchant, merchantResponse.Merchant.Id, merchantResponse.Merchant.Token);
                    foreach (var categoryResponse in categoriesResponses)
                    {
                        //insert products
                        await InsertProducts(merchant, merchantResponse.Merchant.Id, merchantResponse.Merchant.Token, categoryResponse.Id.ToString(), categoryResponse.Name);
                    }
                }
                else
                {
                    responseModel.Errors.Add(merchant.FullName);
                }
            }

            return responseModel;
        }

        public async Task DeleteMerchants()
        {
           await merchantRepository.DeleteMerchants();
        }

        #region private methods

        private async Task<MerchantResponseModel> InsertMerchants(MerchantRequestModel merchantRequest)
        {
            var merchant = new
            {
                full_name = merchantRequest.FullName,
                shop_name = merchantRequest.ShopName,
                email = merchantRequest.Email,
                password = merchantRequest.Password,
                mobile = merchantRequest.Mobile,
                type = merchantRequest.Type,
                address = merchantRequest.Address,
                lat = merchantRequest.Latitude,
                longitude = merchantRequest.Longitude,
                loginType = merchantRequest.LoginType,
                description = merchantRequest.Description,
                inviteCode = merchantRequest.InviteCode,
                quickbloxId = merchantRequest.QuickBloxId,
            };

            return await httpService.GetHttpClientResponse<MerchantResponseModel>("api/merchants/create", merchant, HttpMethod.Post);

        }
        private async Task<List<CategoryResponseModel>> InsertCategories(MerchantRequestModel merchantRequestModel, int merchantId, string token)
        {
            //insert category
            var categoriesContent = await s3BucketService.GetS3Object($"{Prefix}/{merchantRequestModel.CategoryFileName}");
            var categoriesRequest = !string.IsNullOrEmpty(categoriesContent) ? JsonConvert.DeserializeObject<List<CategoryRequestModel>>(categoriesContent)
                : new List<CategoryRequestModel>();

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

        private async Task InsertProducts(MerchantRequestModel merchantRequestModel, int merchantId, string token, string categoryId, string categoryName)
        {
            //insert category
            var productContents = await s3BucketService.GetS3Object($"{Prefix}/{merchantRequestModel.ProductFileName}");
            var productsRequest = !string.IsNullOrEmpty(productContents) ? JsonConvert.DeserializeObject<List<Product>>(productContents)
                : new List<Product>();

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

        #endregion

    }
}