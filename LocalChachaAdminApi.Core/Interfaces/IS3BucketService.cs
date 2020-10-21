using System.IO;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IS3BucketService
    {
        Task<string> GetS3Object(string filePath);
        Task<StreamReader> GetS3StreamReader(string filePath);
    }
}