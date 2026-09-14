using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CoffeeNChillFunctions.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService()
        {
            string connectionString = "UseDevelopmentStorage=true";
            string containerName = "staff-docs";

            _containerClient =
                new BlobContainerClient(connectionString, containerName);

            _containerClient.CreateIfNotExists(
                PublicAccessType.None);
        }

        public async Task UploadDocumentAsync(
            Stream fileStream,
            string fileName,
            string contentType)
        {
            BlobClient blobClient =
                _containerClient.GetBlobClient(fileName);

            BlobHttpHeaders headers = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = headers
                });
        }
    }
}