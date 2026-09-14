using System.Net;
using CoffeeNChillFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CoffeeNChillFunctions.Functions
{
    public class UploadStaffDocument
    {
        private readonly BlobStorageService _blobStorageService;

        public UploadStaffDocument()
        {
            _blobStorageService = new BlobStorageService();
        }

        [Function("UploadStaffDocument")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "documents/upload")]
            HttpRequestData req)
        {
            try
            {
                // Get the file name from the URL
                var query = System.Web.HttpUtility.ParseQueryString(
                    req.Url.Query);

                string? fileName = query["fileName"];

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    var badRequest =
                        req.CreateResponse(HttpStatusCode.BadRequest);

                    await badRequest.WriteStringAsync(
                        "The fileName query parameter is required.");

                    return badRequest;
                }

                // Get the file extension
                string extension =
                    Path.GetExtension(fileName).ToLower();

                string[] allowedExtensions =
                {
                    ".pdf",
                    ".doc",
                    ".docx",
                    ".txt"
                };

                if (!allowedExtensions.Contains(extension))
                {
                    var badRequest =
                        req.CreateResponse(HttpStatusCode.BadRequest);

                    await badRequest.WriteStringAsync(
                        "Only PDF, DOC, DOCX and TXT files are allowed.");

                    return badRequest;
                }

                string contentType =
                    req.Headers.TryGetValues(
                        "Content-Type",
                        out var values)
                    ? values.FirstOrDefault()
                        ?? "application/octet-stream"
                    : "application/octet-stream";

                if (req.Body == null ||
                    (req.Body.CanSeek && req.Body.Length == 0))
                {
                    var badRequest =
                        req.CreateResponse(HttpStatusCode.BadRequest);

                    await badRequest.WriteStringAsync(
                        "A document must be supplied.");

                    return badRequest;
                }

                await _blobStorageService.UploadDocumentAsync(
                    req.Body,
                    fileName,
                    contentType);

                var response =
                    req.CreateResponse(HttpStatusCode.Created);

                await response.WriteStringAsync(
                    $"Document '{fileName}' uploaded successfully.");

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