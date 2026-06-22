using FileStorage.Application.Interfaces;
using FileStorage.Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace FileStorage.Infrastructure.Services
{
    public class MinioService : IMinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;

        public MinioService(IConfiguration configuration)
        {
            _bucketName = configuration["MinIO:BucketName"] ?? "filestorage";

            _minioClient = new MinioClient()
                .WithEndpoint(configuration["MinIO:Endpoint"]!)
                .WithCredentials(
                    configuration["MinIO:AccessKey"]!,
                    configuration["MinIO:SecretKey"]!)
                .WithSSL(false)
                .Build();
        }

        public async Task<string> UploadFileAsync(IFormFile file, int userId)
        {
            var objectName = $"users/{userId}/{Guid.NewGuid()}_{file.FileName}";

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(file.OpenReadStream())
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType));

            return objectName;
        }

        public async Task<Stream> GetFileAsync(string objectName, int userId)
        {
            var memoryStream = new MemoryStream();
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream)));

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<bool> DeleteFileAsync(string objectName, int userId)
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName));

            return true;
        }

        public async Task<List<object>> ListFilesAsync(int userId)
        {
            var list = new List<object>();
            var prefix = $"users/{userId}/";

            await foreach (var item in _minioClient.ListObjectsEnumAsync(new ListObjectsArgs()
                .WithBucket(_bucketName)
                .WithPrefix(prefix)))
            {
                list.Add(new
                {
                    ObjectName = item.Key,
                    Size = item.Size,
                    LastModified = item.LastModified
                });
            }

            return list;
        }
    }
}