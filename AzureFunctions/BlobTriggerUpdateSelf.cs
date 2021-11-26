using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureFunctions
{
    public class BlobTriggerUpdateSelf
    {
        [FunctionName("BlobTriggerUpdateSelf")]
        public void Run([BlobTrigger("chambersdon-samples/{name}", Connection = "")]BlobClient myBlob, string name,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n");

            //Skip if we have processed this blob
            if (!myBlob.GetProperties().Value.Metadata.ContainsKey("BlobTriggerUpdateBlob"))
            {
                var jsonInBlob = myBlob.DownloadContent().Value.Content.ToString();
                dynamic jsonBlob = JsonConvert.DeserializeObject(jsonInBlob);
                jsonBlob.Revision = jsonBlob.Revision + 1;
                BinaryData blobData = new BinaryData(JsonConvert.SerializeObject(jsonBlob));
                myBlob.Upload(blobData, true);

                //Add metedata to indicate this was processed.
                IDictionary<string, string> metadata = new Dictionary<string, string>();
                metadata.Add("BlobTriggerUpdateBlob", "processed");
                myBlob.SetMetadata(metadata);
            }

        }
    }
}
