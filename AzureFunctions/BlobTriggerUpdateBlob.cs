using System;
using System.Dynamic;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions
{
    public class BlobTriggerUpdateBlob
    {
        [FunctionName("BlobTriggerUpdateBlob")]
        public void Run([BlobTrigger("chambersdon-samples/{name}", Connection = "")]Stream myBlob, string name,
            [Blob("chambersdon-samples/{name}", FileAccess.ReadWrite)] BlobContainerClient newBlob, 
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            StreamReader reader = new StreamReader(myBlob);
            string blobContent = reader.ReadToEnd();
            dynamic jsonBlob = JsonConvert.DeserializeObject(blobContent);
            jsonBlob.Revision = jsonBlob.Revision + 1;
            BinaryData blobData = new BinaryData(JsonConvert.SerializeObject(jsonBlob));

            BlobClient blobClient = newBlob.GetBlobClient(name);
            blobClient.Upload(blobData);

        }
    }
}
