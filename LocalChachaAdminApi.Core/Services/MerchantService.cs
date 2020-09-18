﻿using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Infrastructure;
using LocalChachaAdminApi.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly IQuickBloxService quickBloxService;
        private readonly ILogger<MerchantService> logger;

        private static string Prefix = "merchants";
        private const string Username = "localchacha-user-";
        private const string Password = "localchacha";
        private const string MerchantLogin = "localchacha-merchant-";

        public MerchantService(IS3BucketService s3BucketService, IHttpService httpService, IMerchantRepository merchantRepository,
            IQuickBloxService quickBloxService, ILogger<MerchantService> logger)
        {
            this.httpService = httpService;
            this.s3BucketService = s3BucketService;
            this.merchantRepository = merchantRepository;
            this.quickBloxService = quickBloxService;
            this.logger = logger;
        }

        public async Task<List<MerchantRequestModel>> GetMerchants()
        {
            logger.LogInformation("Merchant Service. Getting all merchants");
            var merchantContent = await s3BucketService.GetS3Object(Constants.MerchantsS3Path);
            var merchants = JsonConvert.DeserializeObject<List<MerchantRequestModel>>(merchantContent);

            logger.LogInformation("Merchant Service. Retrived mechants successful");

            return merchants;
        }

        public async Task<CommonResponseModel> SaveMerchants(List<MerchantRequestModel> merchants)
        {
            var responseModel = new CommonResponseModel();
            foreach (var merchant in merchants)
            {
                await InsertMerchants(merchant, responseModel);
            }

            return responseModel;
        }

        public async Task DeleteMerchants()
        {
            var merchants = await merchantRepository.GetMerchants();

            foreach (var merchant in merchants)
            {
                quickBloxService.DeleteUser(merchant);
            }

            //need to fetch data from somewhere else
            foreach (var mobileNumber in Constants.UserMobileNumbers)
            {
                var username = $"{Username}{mobileNumber}";
                var session = await quickBloxService.GetQuickBloxSession(username, Password);

                if (session != null)
                {
                    var quickBloxDialogueResponse = await quickBloxService.GetDialogues(session.Token);

                    //delete dialogues of the quickblox merchants
                    if (quickBloxDialogueResponse != null)
                    {
                        foreach (var dialogue in quickBloxDialogueResponse.Items)
                        {
                            quickBloxService.DeleteDialogue(dialogue.Id, session.Token);
                        }
                    }
                }
            }

            await merchantRepository.DeleteMerchants();
        }

        #region private methods

        private async Task InsertMerchants(MerchantRequestModel merchantRequest, CommonResponseModel responseModel)
        {
            if (merchantRequest.TotalRecords > 0)
            {
                for (int i = 1; i <= merchantRequest.TotalRecords; i++)
                {
                    //before inserting into the database we have to create quickblox user
                    var mobileNumber = i == 1 ? merchantRequest.Mobile : GetNextNumber(merchantRequest.Mobile, i);
                    var email = i == 1 ? merchantRequest.Email : GetNewEmailId(merchantRequest.Email, i);
                    var fullName = i == 1 ? merchantRequest.FullName : $"{merchantRequest.FullName} {i}";

                    var quickBloxUser = await CreateQuickBloxUser(mobileNumber, email, fullName);

                    var merchant = new
                    {
                        full_name = fullName,
                        shop_name = i == 1 ? merchantRequest.ShopName : $"{merchantRequest.ShopName} {i}",
                        email,
                        password = merchantRequest.Password,
                        mobile = mobileNumber,
                        type = merchantRequest.Type,
                        address = merchantRequest.Address,
                        lat = merchantRequest.Latitude,
                        longitude = merchantRequest.Longitude,
                        loginType = merchantRequest.LoginType,
                        description = merchantRequest.Description,
                        inviteCode = merchantRequest.InviteCode,
                        quickbloxId = quickBloxUser?.Id ?? 0
                    };

                    await InsertMerchant(merchant, merchantRequest, responseModel);

                    CreateQuickBloxDialogue(quickBloxUser, merchantRequest);
                }
            }
            else
            {
                var quickBloxUser = await CreateQuickBloxUser(merchantRequest.Mobile, merchantRequest.Email, merchantRequest.FullName);

                CreateQuickBloxDialogue(quickBloxUser, merchantRequest);

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
                    quickbloxId = quickBloxUser?.Id ?? 0
                };

                await InsertMerchant(merchant, merchantRequest, responseModel);
            }
        }

        private async Task CreateQuickBloxDialogue(QuickbloxUser quickBloxUser, MerchantRequestModel merchantRequest)
        {
            if (quickBloxUser != null)
            {
                //for every user and merchant create default chats
                foreach (var userMobileNumber in Constants.UserMobileNumbers)
                {
                    var username = $"{Username}{userMobileNumber}";
                    var session = await quickBloxService.GetQuickBloxSession(username, Password);

                    if (session != null)
                    {
                        var dialogue = await quickBloxService.CreateDialogue(session.UserId, quickBloxUser.Id, session.Token);

                        if (dialogue != null)
                        {
                            logger.LogInformation($"Dialoue {dialogue.Id} created successfully for user {username} and merchant {quickBloxUser.Login}");
                            logger.LogInformation($"Inserting total {merchantRequest.TotalMessages} for user {username} and merchant {quickBloxUser.Login}");
                            //add chat for the dialogue. take a chat count as input
                            for (var i = 1; i <= merchantRequest.TotalMessages; i++)
                            {
                                var messageRequest = new CreateMessageRequest
                                {
                                    ChatDialogId = dialogue.Id,
                                    Message = $"Test Message {i}"
                                };

                                quickBloxService.CreateMessage(messageRequest, session.Token);
                            }
                        }
                    }
                }
            }
        }

        private async Task<QuickbloxUser> CreateQuickBloxUser(string mobileNumber, string email, string fullName)
        {
            var quickBloxUserRequest = new QuickBloxUserRequest()
            {
                Email = email,
                Phone = mobileNumber,
                Password = "localchacha",
                Login = $"{MerchantLogin}{mobileNumber}",
                FullName = fullName
            };

            var quickBloxUser = await quickBloxService.CreateUser(quickBloxUserRequest);
            return quickBloxUser;
        }

        private async Task InsertMerchant(object merchant, MerchantRequestModel merchantRequest, CommonResponseModel responseModel)
        {
            var merchantResponse = await httpService.GetHttpClientResponse<MerchantResponseModel>("api/merchants/create", merchant, HttpMethod.Post);
            var merchantName = ObjectExtension.GetPropertyValue<string>(merchant, "full_name");
            if (merchantResponse.Status == 1)
            {
                logger.LogInformation($"Created merchant {merchantName} successfully.");
                await InsertCategories(merchantRequest, merchantResponse.Merchant.Id, merchantResponse.Merchant.Token);
            }
            else
            {
                logger.LogInformation($"Failed to insert merchant {merchantName}.");
                responseModel.Errors.Add(merchantName);
            }
        }

        private async Task InsertCategories(MerchantRequestModel merchantRequestModel, int merchantId, string token)
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
                {
                    logger.LogInformation($"Created category  {categoryRequest.Name} successfully.");
                    categoriesResponses.Add(categoriesResponse);
                }
                else
                {
                    logger.LogInformation($"Failed to insert category {categoryRequest.Name}.");
                }
            }

            foreach (var categoryResponse in categoriesResponses)
            {
                //insert products
                await InsertProducts(merchantRequestModel, merchantId, token, categoryResponse.Id.ToString(), categoryResponse.Name);
            }
        }

        private async Task InsertProducts(MerchantRequestModel merchantRequestModel, int merchantId, string token, string categoryId, string categoryName)
        {
            //insert category
            var productContents = await s3BucketService.GetS3Object($"{Prefix}/{merchantRequestModel.ProductFileName}");
            var productsRequest = !string.IsNullOrEmpty(productContents) ? JsonConvert.DeserializeObject<List<Product>>(productContents)
                : new List<Product>();

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

                await httpService.GetHttpClientResponse<ProductResponseModel>("api/products/createProduct", productRequestModel, HttpMethod.Post, true, token);
            }
        }

        private string GetNextNumber(string number, int incrementBy)
        {
            return !string.IsNullOrEmpty(number) ? (long.Parse(number) + incrementBy).ToString() : number;
        }

        private string GetNewEmailId(string email, int number)
        {
            var array = email.Split('@');
            return string.Join($"{number}@", array);
        }

        #endregion private methods
    }
}