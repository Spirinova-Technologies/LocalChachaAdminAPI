using CsvHelper;
using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Infrastructure;
using LocalChachaAdminApi.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IUserRepository userRepository;
        private readonly ILogger<MerchantService> logger;

        private static string Prefix = "merchants";
        private const string Username = "localchacha-user-";
        private const string Password = "localchacha";
        private const string MerchantLogin = "localchacha-merchant-";

        public MerchantService(IS3BucketService s3BucketService, IHttpService httpService, IMerchantRepository merchantRepository,
            IQuickBloxService quickBloxService, ILogger<MerchantService> logger, IUserRepository userRepository)
        {
            this.httpService = httpService;
            this.s3BucketService = s3BucketService;
            this.merchantRepository = merchantRepository;
            this.quickBloxService = quickBloxService;
            this.logger = logger;
            this.userRepository = userRepository;
        }

        public async Task<List<MerchantRequestModel>> GetSuggestedMerchants()
        {
            logger.LogInformation("Merchant Service. Getting all merchants");
            var merchantContent = await s3BucketService.GetS3Object(Constants.MerchantsS3Path);
            var merchants = JsonConvert.DeserializeObject<List<MerchantRequestModel>>(merchantContent);

            logger.LogInformation("Merchant Service. Retrived mechants successful");

            return merchants;
        }

        public async Task<CommonResponseModel> SaveSuggestedMerchants(List<MerchantRequestModel> merchants)
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
            var merchants = await merchantRepository.GetAllMerchants();

            foreach (var merchant in merchants)
            {
               await quickBloxService.DeleteUser(merchant);
            }

            var users = await userRepository.GetAllUsers();
            //need to fetch data from somewhere else
            foreach (var user in users)
            {
                var username = $"{Username}{user.Id}";
                await DeleteDialogues(username, Password);
            }

            //delete all welcome message
            await DeleteDialogues("LocalChacha", Password);

            await merchantRepository.DeleteMerchants();
        }

        private async Task DeleteDialogues(string username, string password)
        {
            var session = await quickBloxService.GetQuickBloxSession(username, password);

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

        #region private methods

        public async Task<List<Location>> GetLocations()
        {
            logger.LogInformation("Getting locations");

            using (var reader = await s3BucketService.GetS3StreamReader(Constants.LocationsS3Path))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var locations = csv.GetRecords<Location>().ToList();

                    return locations;
                }
            }

        }

        public Location GetRandomLocation(List<Location> locations)
        {
            var random = new Random();
            int index = random.Next(locations.Count);

            return locations[index];
        }

        private async Task InsertMerchants(MerchantRequestModel merchantRequest, CommonResponseModel responseModel)
        {
            var locations = await GetLocations();
            if (merchantRequest.TotalRecords > 0)
            {
                for (int i = 1; i <= merchantRequest.TotalRecords; i++)
                {
                    var mobileNumber = i == 1 ? merchantRequest.Mobile : GetNextNumber(merchantRequest.Mobile, i);
                    var email = i == 1 ? merchantRequest.Email : GetNewEmailId(merchantRequest.Email, i);
                    var fullName = i == 1 ? merchantRequest.FullName : $"{merchantRequest.FullName} {i}";
                    var location = GetRandomLocation(locations);

                    var merchant = new
                    {
                        full_name = fullName,
                        shop_name = i == 1 ? merchantRequest.ShopName : $"{merchantRequest.ShopName} {i}",
                        email,
                        password = merchantRequest.Password,
                        mobile = mobileNumber,
                        type = merchantRequest.Type,
                        address = merchantRequest.Address,
                        lat = location.Latitude,
                        longitude = location.Longitude,
                        loginType = merchantRequest.LoginType,
                        description = merchantRequest.Description,
                        inviteCode = merchantRequest.InviteCode,
                    };

                 var merchantResponse =   await InsertMerchant(merchant, merchantRequest, responseModel);

                 CreateQuickBloxDialogue(merchantResponse, merchantRequest);
                }
            }
            else
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
                };

                var merchantResponse = await InsertMerchant(merchant, merchantRequest, responseModel);

                CreateQuickBloxDialogue(merchantResponse, merchantRequest);
            }
        }

        private async Task CreateQuickBloxDialogue(MerchantResponseModel merchantResponseModel, MerchantRequestModel merchantRequest)
        {
            if (merchantResponseModel.Merchant != null)
            {
                //for every user and merchant create default chats
                foreach (var userMobileNumber in Constants.UserMobileNumbers)
                {
                    var username = $"{Username}{userMobileNumber}";
                    var session = await quickBloxService.GetQuickBloxSession(username, Password);

                    if (session != null)
                    {
                        var dialogue = await quickBloxService.CreateDialogue(session.UserId, merchantResponseModel.Merchant.QuickBloxId, session.Token);

                        if (dialogue != null)
                        {
                            logger.LogInformation($"Dialoue {dialogue.Id} created successfully for user {username} and merchant {merchantResponseModel.Merchant.FullName}");
                            logger.LogInformation($"Inserting total {merchantRequest.TotalMessages} for user {username} and merchant {merchantResponseModel.Merchant.FullName}");
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

        private async Task<MerchantResponseModel> InsertMerchant(object merchant, MerchantRequestModel merchantRequest, CommonResponseModel responseModel)
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

            return merchantResponse;
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

        public Task<List<Merchant>> GetMerchants(SearchFilter filter)
        {
            return merchantRepository.GetMerchants(filter);
        }

        #endregion private methods
    }
}