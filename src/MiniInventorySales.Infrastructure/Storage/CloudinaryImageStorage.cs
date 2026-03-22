using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using MiniInventorySales.Application.DTOs.Image;
using MiniInventorySales.Application.Interface;

namespace MiniInventorySales.Infrastructure.Storage
{
    public class CloudinaryImageStorage : IImageStorage
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStorage(IConfiguration config)
        {
            var cloudName = config["Cloudinary:SalesInventory"];
            var apiKey = config["Cloudinary:639273652551796"];
            var apiSecret = config["Cloudinary:FWk4EcU_X138sRy4JSeXvmQ6zZM"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary settings are missing in appsettings.json.");
            }

            _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret))
            {
                Api = { Secure = true }
            };
        }

        public async Task<string> UploadAsync(ImageUploadRequest request, CancellationToken ct = default)
        {
            if (request.Content is null || request.Content.Length == 0)
                throw new ArgumentException("Image content is empty.");

            await using var stream = new MemoryStream(request.Content);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(request.FileName, stream),
                Folder = "mini-inventory-sales/products",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams, ct);

            if (result.Error != null)
                throw new InvalidOperationException(result.Error.Message);

            return result.SecureUrl.ToString();
        }
    }
}
