namespace UploadImage.API.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public string ExtensionImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ImageProfile : AutoMapper.Profile
    {
        public ImageProfile()
        {
            CreateMap<Database.Entities.Image, Image>();
            CreateMap<Image, Database.Entities.Image>();
        }
    }
}
