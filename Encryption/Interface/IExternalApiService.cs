namespace Encryption.Interface
{
    public interface IExternalApiService
    {
        Task<string> SendEncryptedDataAsync(object data, string externalApiUrl);
    }
}
