using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using PlaygroundCompiler.Infrastrucutre.Services.Interfaces;
using PlaygroundService.Infrastrucutre.Configurations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlaygroundCompiler.Infrastrucutre.Services
{
    public class BlobService : IBlobService
    {
        private const string blobContainerName = "codeplaygroundimages";
        private readonly IOptions<RootConfiguration> _options;

        public BlobService(IOptions<RootConfiguration> options)
        {
            _options = options;
        }

        public async Task<string> GetBlobImageUrl(FileStream fileStream)
        {
            BlobContainerClient blobContainerClient = new(_options.Value.BlobConnectionString, blobContainerName);
            string newName = Guid.NewGuid().ToString() + ".png";
            BlobClient blobClient = blobContainerClient.GetBlobClient(newName);
            if (!blobClient.Exists())
            {
                await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "image/png" });
            }

            return blobClient.Uri.AbsoluteUri;
        }
    }
}
