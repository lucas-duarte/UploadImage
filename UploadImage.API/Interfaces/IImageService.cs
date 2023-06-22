using UploadImage.API.Models;

namespace UploadImage.API.Interfaces
{
    public interface IImageService
    {
        Task<IProviderResult<bool>> SaveImageAsync(string imagePath, Guid id, string imageExtension);
        Task<IProviderResult<bool>> DeleteImageAsync(Guid id);

        Task<IProviderResult<bool>> UpdateAsync(Image obj);
        Task<IProviderResult<List<Image>>> GetLisImagens();
        Task<IProviderResult<Image>> GetAsync(Guid id);
    }
}
