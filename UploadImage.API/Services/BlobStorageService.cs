using Azure.Storage.Blobs;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UploadImage.API.Models;
using UploadImage.API.Interfaces;

namespace UploadImage.API.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly IImageService _imageService;

        public BlobStorageService(string connectionString, string containerName, IImageService imageService)
        {
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _imageService = imageService;
        }

        public async Task<Image> CreateImageAsync(IFormFile file, Guid id)
        {
            using var fileStream = file.OpenReadStream();

            var extensionImage = Path.GetExtension(file.FileName);

            var blobClient = _containerClient.GetBlobClient(id.ToString() + extensionImage);

            await blobClient.UploadAsync(fileStream, true);

            var imagePath = blobClient.Uri.ToString(); // Obtém a URL completa da imagem no blob

            Image img = new()
            {
                Id = id,
                ExtensionImage = extensionImage,
                ImagePath = imagePath,
                CreatedAt = DateTime.Now
            };

            return img;
        }

        public async Task<bool> DeleteFileAsync(string blobName)
        {

            var blobClient = _containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();

            return response.Value;
        }

        public IEnumerable<string> GetBlobList()
        {
            var blobs = _containerClient.GetBlobs();

            var blobNames = new List<string>();
            foreach (var blobItem in blobs)
            {
                blobNames.Add(blobItem.Name);
            }

            return blobNames;
        }

    }
}
