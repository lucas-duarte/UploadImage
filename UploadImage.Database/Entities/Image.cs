using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadImage.Database.Entities
{
    public class Image
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public string ExtensionImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
