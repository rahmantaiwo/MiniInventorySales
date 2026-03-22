using MiniInventorySales.Application.DTOs.Image;

namespace MiniInventorySales.Application.Interface
{
    public interface IImageStorage
    {
        /// <summary>
        /// Uploads a single image and returns the URL (Cloudinary secure URL).
        /// </summary>
        Task<string> UploadAsync(ImageUploadRequest request, CancellationToken ct = default);
    }
}
