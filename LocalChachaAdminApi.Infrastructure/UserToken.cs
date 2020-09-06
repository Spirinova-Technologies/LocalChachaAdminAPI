using System;

namespace LocalChachaAdminApi.Infrastructure
{
    public class UserToken
    {
        public int UserId { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime IssuedAt { get; set; }

        public static JwtToken ConvertToJwtToken(UserToken token) => new JwtToken
        {
            exp = new DateTimeOffset(token.Expiration).ToUnixTimeSeconds(),
            iat = new DateTimeOffset(token.IssuedAt).ToUnixTimeSeconds(),
            userId = token.UserId,
        };

        public static UserToken ConvertFromJwtToken(JwtToken token) => new UserToken
        {
            Expiration = DateTimeOffset.FromUnixTimeSeconds(token.exp).UtcDateTime,
            IssuedAt = DateTimeOffset.FromUnixTimeSeconds(token.iat).UtcDateTime,
            UserId = token.userId
        };
    }

    public sealed class JwtToken
    {
        public long exp { get; set; }

        public long iat { get; set; }

        public int userId { get; set; }
    }
}