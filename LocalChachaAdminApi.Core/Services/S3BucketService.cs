using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using LocalChachaAdminApi.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Services
{
    public class S3BucketService : IS3BucketService
    {
        private readonly IConfiguration configuration;

        public S3BucketService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> GetS3Object(string filePath)
        {
            string bucketName = configuration["bucketName"]; 
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = filePath
            };

            var client = GetAmazonS3Client();
            GetObjectResponse response = await client.GetObjectAsync(request);
            StreamReader reader = new StreamReader(response.ResponseStream);
            return reader.ReadToEnd();
        }

        private AmazonS3Client GetAmazonS3Client()
        {
            string awsAccessKey = configuration["awsAccessKey"];
            string awsSecretKey = configuration["awsSecretKey"]; 

            return new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.APSouth1);
        }
    }
}