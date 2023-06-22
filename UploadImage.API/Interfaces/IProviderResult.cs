namespace UploadImage.API.Interfaces
{

    public interface IProviderResult<T>
    {
        public T? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }

}
