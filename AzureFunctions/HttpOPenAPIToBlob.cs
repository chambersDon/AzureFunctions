using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AzureFunctions
{
    public class HttpOPenAPIToBlob
    {
        private readonly ILogger<HttpOPenAPIToBlob> _logger;

        public HttpOPenAPIToBlob(ILogger<HttpOPenAPIToBlob> log)
        {
            _logger = log;
        }

        [FunctionName("HttpOp" +
            "enAPIToBlob")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Blob("chambersdon-samples", FileAccess.ReadWrite)] BlobContainerClient newBlob,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name1 = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic jsonBody = JsonConvert.DeserializeObject(requestBody);
            //TODO actions on jsonBody
            var jsonToSave = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(jsonBody));
            BinaryData blobData = new BinaryData(jsonToSave);

            newBlob.CreateIfNotExists();
            string nameBlobName = Guid.NewGuid().ToString();
            BlobClient blobClient = newBlob.GetBlobClient(nameBlobName);
            await blobClient.UploadAsync(blobData);

            dynamic retVal = new ExpandoObject();
            retVal.NewBlobID = nameBlobName;
            string responseBody = JsonConvert.SerializeObject(retVal);

            return new OkObjectResult(responseBody);
        }
    }
}

