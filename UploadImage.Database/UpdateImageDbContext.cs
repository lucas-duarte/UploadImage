using Microsoft.EntityFrameworkCore;
using UploadImage.Database.Entities;

namespace UploadImage.Database
{
    public class UpdateImageDbContext : DbContext
    {
        public UpdateImageDbContext(DbContextOptions<UpdateImageDbContext> options) : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }

    }
}