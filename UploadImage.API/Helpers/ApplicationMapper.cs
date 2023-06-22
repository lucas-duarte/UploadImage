
using AutoMapper;
using System.Reflection;

namespace UploadImage.API.Helpers
{
    public static class ApplicationMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;

                cfg.AddMaps(Assembly.GetExecutingAssembly());
            });

            return config.CreateMapper();
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public static class ApplicationMapperExtensions
    {
        public static T Map<T>(this object source)
        {
            return ApplicationMapper.Mapper.Map<T>(source);
        }
    }
}
