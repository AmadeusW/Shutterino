using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Windows.Storage;

namespace AmadeusW.Shutterino.Azure
{
    public class FileUploader
    {
        public static async Task<string> UploadFile(string connectionString, string fileName, StorageFile file)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference("shutterino");
            await share.CreateIfNotExistsAsync();
            var root = share.GetRootDirectoryReference();
            var reference = root.GetFileReference(fileName);
            await reference.UploadFromFileAsync(file);

            return reference.Uri.AbsoluteUri;
        }

    }
}

