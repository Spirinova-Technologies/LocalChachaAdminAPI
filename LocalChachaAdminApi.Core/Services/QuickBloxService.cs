using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Infrastructure;
using LocalChachaAdminApi.Infrastructure.Extensions;
using LocalChachaAdminApi.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Services
{
    public class QuickBloxService : IQuickBloxService
    {
        private readonly IRestClient client;

        //private variables
        private readonly string _authKey;
        private readonly string _authSecret;
        private readonly string _appId;
        private readonly string _accountKey;
        public QuickBloxService(IConfiguration configuration)
        {
            client = new RestClient(configuration["QuickBlox:BaseUrl"]); ;

            _authKey = configuration["QuickBlox:AuthKey"];
            _authSecret = configuration["QuickBlox:AuthSecret"];
            _appId = configuration["QuickBlox:AppId"];
            _accountKey = configuration["QuickBlox:AccountKey"];
        }

        public async Task<string> GetQuickBloxToken(string username ="", string passowrd = "")
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

            if(!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(passowrd))
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

            if (response.ResponseStatus == ResponseStatus.Completed && response.ErrorMessage == null)
            {
                return response.Data.Session.Token;
            }
            else
            {
                throw response.ErrorException;
            }

        }

        public async Task<QuickbloxUser> CreateUser(QuickBloxUserRequest userRequest)
        {
            var request = new RestRequest("users.json", Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftSerializer()
            };
            request.AddJsonBody(new { user = userRequest });

            await AddHeaders(request);

            var result = await client.ExecutePostAsync<QuickBloxUserResponse>(request);

            if (result.ResponseStatus != ResponseStatus.Completed)
            {
                return null;
            }
            if (result.StatusCode == HttpStatusCode.Created)
                return result.Data.User;

            return null;
        }

        public async Task DeleteUser(Merchant merchant)
        {
            var request = new RestRequest($"users/{merchant.QuickBloxId}.json", Method.DELETE)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new NewtonsoftSerializer()
            };

            await AddHeaders(request, merchant);

            try
            {
                var result = await client.ExecuteAsync(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private async Task AddHeaders(RestRequest request, Merchant merchant = null)
        {
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new NewtonsoftSerializer();
            request.AddHeader("QuickBlox-REST-API-Version", "0.1.0");

            var username = merchant != null ? $"localchacha-merchant-{merchant.Mobile}": string.Empty;
            var password = merchant != null ? "localchacha" : string.Empty;

            string token = await GetQuickBloxToken(username, password);

            request.AddHeader("QB-Token", token);
        }
    }
}