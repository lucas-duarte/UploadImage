using Microsoft.EntityFrameworkCore;
using System.Numerics;
using UploadImage.API.Helpers;
using UploadImage.API.Interfaces;
using UploadImage.API.Models;
using UploadImage.Database;

namespace UploadImage.API.Services
{
    public class ImageService : IImageService
    {
        private readonly UpdateImageDbContext _dbContext;

        public ImageService(UpdateImageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IProviderResult<bool>> SaveImageAsync(string imagePath, Guid id, string imageExtension)
        {
            try
            {
                var image = new Image
                {
                    Id = id,
                    ImagePath = imagePath,
                    ExtensionImage = imageExtension,
                    CreatedAt = DateTime.Now
                };

                var entityImage = image.Map<Database.Entities.Image>();

                await _dbContext.Images.AddAsync(entityImage);

                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    return new ProviderResult<bool>(true, null);
                }

                return new ProviderResult<bool>(false, "");
            }
            catch (Exception ex)
            {
                return new ProviderResult<bool>(false, ex.Message);
            }
        }

        public async Task<IProviderResult<bool>> DeleteImageAsync(Guid id)
        {
            string error;

            try
            {
                var entity = await _dbContext.Images.FindAsync(id);

                if (entity is not null)
                {
                    _dbContext.Images.Remove(entity);

                    var result = await _dbContext.SaveChangesAsync();

                    if (result > 0)
                    {
                        return new ProviderResult<bool>(true, null);
                    }

                    error = $"A imagem não foi deletada";

                    return new ProviderResult<bool>(false, error);
                }

                error = $"Não foi possível encontrar Imagem";

                return new ProviderResult<bool>(false, error);
            }
            catch (Exception ex)
            {
                return new ProviderResult<bool>(false, ex.Message);
            }
        }

        public async Task<IProviderResult<List<Image>>> GetLisImagens()
        {
            try
            {
                var entities = await _dbContext.Images.ToListAsync();

                return new ProviderResult<List<Image>>(entities.Map<List<Image>>(), null);
            }
            catch (Exception ex)
            {
                return new ProviderResult<List<Image>>(null, ex.Message);
            }
        }

        public async Task<IProviderResult<Image>> GetAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.Images.FirstOrDefaultAsync(p => p.Id.Equals(id));

                return new ProviderResult<Image>(entity?.Map<Image>(), null);
            }
            catch (Exception ex)
            {
                return new ProviderResult<Image>(null, ex.Message);
            }
        }

        public async Task<IProviderResult<bool>> UpdateAsync(Image obj)
        {
            try
            {
                var updatedEntity = obj?.Map<Database.Entities.Image>();

                var currentEntity = await _dbContext.Images.FirstOrDefaultAsync(d => d.Id.Equals(obj.Id));

                if (currentEntity is not null)
                {
                    _dbContext.Entry(currentEntity).CurrentValues.SetValues(updatedEntity);

                    _dbContext.Update(currentEntity);

                    var result = await _dbContext.SaveChangesAsync();

                    if (result > 0)
                    {
                        return new ProviderResult<bool>(true, null);
                    }

                    return new ProviderResult<bool>(false, "Deal has not been updated successfully");
                }

                return new ProviderResult<bool>(false, "Deal not found");
            }
            catch (Exception ex)
            {
                return new ProviderResult<bool>(false, "Deal not found");
            }
        }
    }
}
