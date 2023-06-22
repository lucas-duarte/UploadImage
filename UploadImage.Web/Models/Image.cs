namespace UploadImage.Web.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public string ExtensionImage { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
