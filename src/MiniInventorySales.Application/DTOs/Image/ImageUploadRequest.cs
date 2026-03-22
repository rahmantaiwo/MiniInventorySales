namespace MiniInventorySales.Application.DTOs.Image
{
    public class ImageUploadRequest
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
    }
}
