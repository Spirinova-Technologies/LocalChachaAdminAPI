using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Interfaces
{
   public interface IS3BucketService
    {
        Task<string> GetS3Object(string filePath);
    }
}
