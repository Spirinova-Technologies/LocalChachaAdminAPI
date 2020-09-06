using LocalChachaAdminApi.Infrastructure;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(int userId);
        bool IsTokenValid(string authToken, out UserToken decodedToken);
    }
}