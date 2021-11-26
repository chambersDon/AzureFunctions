using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using System.Text;
using System.Dynamic;

namespace AzureFunctions
{
    public static class HttpToBlob
    {
        [FunctionName("HttpToBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Blob("chambersdon-samples", FileAccess.ReadWrite)] BlobContainerClient newBlob,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

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
