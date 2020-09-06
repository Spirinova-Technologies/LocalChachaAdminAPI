using Jose;
using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace LocalChachaAdminApi.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateToken(int userId)
        {
            var userToken = new UserToken
            {
                UserId = userId,
                Expiration = DateTime.Now.AddDays(4),
                IssuedAt = DateTime.Now
            };

            var payload = UserToken.ConvertToJwtToken(userToken);
            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(configuration["Jwt:Key"]), JwsAlgorithm.HS512);

            return token;
        }

        public bool IsTokenValid(string authToken, out UserToken decodedToken)
        {
            try
            {
                var jwtToken = JWT.Decode<JwtToken>(authToken, Encoding.UTF8.GetBytes(configuration["Jwt:Key"]), JwsAlgorithm.HS512);
                decodedToken = UserToken.ConvertFromJwtToken(jwtToken);

                var isExpired = DateTime.Now > decodedToken.Expiration;

                return !isExpired;
            }
            catch (Exception)
            {
                decodedToken = null;
                return false;
            }
        }
    }
}