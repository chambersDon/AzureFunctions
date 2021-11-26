using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFunctions
{
    public static class HttpGetBlobByName
    {
        [FunctionName("HttpGetBlobByName")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string blobName = req.Query["blobname"];

            CloudStorageAccount blobAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudBlobClient blobClient = blobAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("chambersdon-samples");
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{blobName}");

            string responseMessage = blob.DownloadTextAsync().Result;
            return new OkObjectResult(responseMessage);
        }
    }
}
