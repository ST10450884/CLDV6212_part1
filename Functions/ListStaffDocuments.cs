using System.Net;
using CoffeeNChillFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CoffeeNChillFunctions.Functions
{
    public class ListStaffDocuments
    {
        private readonly BlobStorageService _blobStorageService;

        public ListStaffDocuments()
        {
            _blobStorageService = new BlobStorageService();
        }

        [Function("ListStaffDocuments")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "documents")]
            HttpRequestData req)
        {
            try
            {
                var documents =
                    await _blobStorageService.ListDocumentsAsync();

                var response =
                    req.CreateResponse(HttpStatusCode.OK);

                await response.WriteAsJsonAsync(documents);

                return response;
            }
            catch (Exception ex)
            {
                var response =
                    req.CreateResponse(
                        HttpStatusCode.InternalServerError);

                await response.WriteStringAsync(
                    $"An error occurred: {ex.Message}");

                return response;
            }
        }
    }
}