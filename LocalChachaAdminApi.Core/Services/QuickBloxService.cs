using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Infrastructure;
using LocalChachaAdminApi.Infrastructure.Extensions;
using LocalChachaAdminApi.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Services
{
    public class QuickBloxService : IQuickBloxService
    {
        private readonly IRestClient client;
        private readonly ILogger<QuickBloxService> logger;

        //private variables
        private readonly string _authKey;

        private readonly string _authSecret;
        private readonly string _appId;
        private readonly string _accountKey;

        private const string MerchantUsername = "localchacha-merchant-";
        private const string Password = "localchacha";

        public QuickBloxService(IConfiguration configuration, ILogger<QuickBloxService> logger)
        {
            client = new RestClient(configuration["QuickBlox:BaseUrl"]);
            this.logger = logger;

            _authKey = configuration["QuickBlox:AuthKey"];
            _authSecret = configuration["QuickBlox:AuthSecret"];
            _appId = configuration["QuickBlox:AppId"];
            _accountKey = configuration["QuickBlox:AccountKey"];

           
        }

        public async Task<string> GetQuickBloxToken(string username = "", string passowrd = "")
        {
            var session = await GetQuickBloxSession(username, passowrd);
            return session?.Token;
        }

        public async Task<QuickbloxUser> CreateUser(QuickBloxUserRequest userRequest)
        {
            logger.LogInformation($"Creating Quickblox user {userRequest.FullName}");
            var request = new RestRequest("users.json", Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftSerializer()
            };
            request.AddJsonBody(new { user = userRequest });

            var token = await GetQuickBloxToken();

            AddHeaders(request, token);

            var result = await client.ExecutePostAsync<QuickBloxUserResponse>(request);

            if (result.ResponseStatus != ResponseStatus.Completed)
            {
                logger.LogError($"Failed to create user. {result.ErrorMessage}");
                return null;
            }
            if (result.StatusCode == HttpStatusCode.Created)
            {
                logger.LogError($"Created quickblox user. {JsonConvert.SerializeObject(result.Data.User)}");
                return result.Data.User;
            }

            logger.LogError($"Failed to create user. {result.ErrorMessage}");
            return null;
        }

        public async Task<Session> GetQuickBloxSession(string username = "", string passowrd = "")
        {
            var nonce = GlobalHelper.getNonce();
            var timeStamp = GlobalHelper.getTimestamp();

            var request = new RestRequest(Method.POST)
            {
                Resource = "session.json"
            };
            request.AddParameter("application_id", _appId);
            request.AddParameter("auth_key", _authKey);
            request.AddParameter("nonce", nonce);
            request.AddParameter("timestamp", timeStamp);

            var postData = new StringBuilder();
            postData.AppendFormat($"application_id={_appId}");
            postData.AppendFormat($"&auth_key={_authKey}");
            postData.AppendFormat("&nonce={0}", nonce);
            postData.AppendFormat("&timestamp={0}", timeStamp);

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(passowrd))
            {
                postData.AppendFormat("&user[login]={0}", username);
                postData.AppendFormat("&user[password]={0}", passowrd);

                request.AddParameter("user[login]", username);
                request.AddParameter("user[password]", passowrd);
            }

            var signature = GlobalHelper.getHash(postData.ToString(), _authSecret).ByteToString();

            request.AddParameter("signature", signature);

            request.AddHeader("QuickBlox-REST-API-Version", "0.1.0");
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<Token>(request);

            if (response.ResponseStatus == ResponseStatus.Completed && response.ErrorMessage == null && response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response.Data.Session;
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteUser(Merchant merchant)
        {
            var request = new RestRequest($"users/{merchant.QuickBloxId}.json", Method.DELETE)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftSerializer()
            };

            var token = await GetQuickBloxToken($"{MerchantUsername}{merchant.Id}", Password);

            AddHeaders(request, token);

            var result = await client.ExecuteAsync(request);
        }

        public async Task<QuickBloxDialogueResponse> GetDialogues(string token)
        {
            var request = new RestRequest($"chat/Dialog.json", Method.GET)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftSerializer()
            };

            AddHeaders(request, token);

            var result = await client.ExecuteGetAsync<QuickBloxDialogueResponse>(request);

            if (result.ResponseStatus != ResponseStatus.Completed)
            {
                return null;
            }
            if (result.StatusCode == HttpStatusCode.OK)
                return result.Data;

            return null;
        }

        public async Task<CreateDialogueResponse> CreateDialogue(long userQuickBloxId, int merchantQuickBloxId, string token)
        {
            var quickBloxDialogueRequest = new CreateQuickBloxDialogueRequest
            {
                Type = 3,
                OccupantsIds = $"{merchantQuickBloxId},{userQuickBloxId}"
            };

            var request = new RestRequest("chat/Dialog.json", Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftSerializer()
            };
            request.AddJsonBody(quickBloxDialogueRequest);

            AddHeaders(request, token);

            var result = await client.ExecutePostAsync<CreateDialogueResponse>(request);

            if (result.ResponseStatus != ResponseStatus.Completed)
            {
                logger.LogError($"Failed to Create dialogue. {result.ErrorMessage}");
                return null;
            }
            if (result.StatusCode == HttpStatusCode.Created)
            {
                logger.LogInformation($"Dialogue {result.Data.Id} created successfully");
                return result.Data;
            }

            logger.LogError($"Failed to Create dialogue. {result.ErrorMessage}");
            return null;
        }

        public async Task DeleteDialogue(string dialogueId, string token)
        {
            var request = new RestRequest($"chat/Dialog/{dialogueId}.json", Method.DELETE)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftSerializer(),
                Body = new RequestBody(DataFormat.Json.ToString(), "force", 1)
            };

            AddHeaders(request, token);

            var result = await client.ExecuteAsync(request);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                logger.LogInformation($"Dialogue {dialogueId} deleted successfully");
            }
            else
            {
                logger.LogError($"Failed to delete dialogue. {result.ErrorMessage}");
            }

        }

        public async Task<string> CreateMessage(CreateMessageRequest createMessageRequest, string token)
        {
            var request = new RestRequest("chat/Message.json", Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftSerializer()
            };
            request.AddJsonBody(createMessageRequest);

            AddHeaders(request, token);

            var result = await client.ExecutePostAsync<string>(request);

            if (result.ResponseStatus != ResponseStatus.Completed)
            {
                logger.LogError($"Failed to create Message. {result.ErrorMessage}");
                return null;
            }
            if (result.StatusCode == HttpStatusCode.Created)
            {
                logger.LogInformation($"Message {createMessageRequest.Message} added successfully");
                return result.Data;
            }

            logger.LogError($"Failed to create Message. {result.ErrorMessage}");
            return null;
        }

        private void AddHeaders(RestRequest request, string token)
        {
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new NewtonsoftSerializer();
            request.AddHeader("QuickBlox-REST-API-Version", "0.1.0");

            request.AddHeader("QB-Token", token);
        }
    }
}