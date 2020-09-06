using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Services
{
    public class MerchantService : IMerchantService
    {
        private readonly IHttpService httpService;
        private readonly IS3BucketService s3BucketService;

        public MerchantService(IS3BucketService s3BucketService, IHttpService httpService)
        {
            this.httpService = httpService;
            this.s3BucketService = s3BucketService;
        }

        public async Task<List<MerchantRequestModel>> GetMerchants()
        {
            var merchantContent = await s3BucketService.GetS3Object(Constants.MerchantsS3Path);
            var merchants = JsonConvert.DeserializeObject<List<MerchantRequestModel>>(merchantContent);

            return merchants;
        }
    }
}