using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Text;
using Encryption.Interface;
using Encryption.Model;
using Newtonsoft.Json;

namespace Encryption.Repository
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AesEncryptionHelper _aesHelper;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Key and IV must match with external API's decryption logic
            string aesKey = "IUU+w1HAa9k7bGtT9zb9asJJc42kBbq/l6o3DG2+3v0=";  
            string aesIV = "I/hhunYukZED5bY1sJ/V1g==";    
            _aesHelper = new AesEncryptionHelper(aesKey, aesIV);
        }

        public async Task<string> SendEncryptedDataAsync(object data, string externalApiUrl)
        {
            // Convert data to JSON
            string jsonData = JsonConvert.SerializeObject(data);

            // Encrypt the JSON data
            string encryptedData = _aesHelper.Encrypt(jsonData);

            // Create the HTTP request content
            //var httpContent = new StringContent(encryptedData, Encoding.UTF8, "application/json");
            // Send the encrypted data as a JSON object
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { encryptedData }), Encoding.UTF8, "application/json");


            // Send POST request to the external API
            HttpResponseMessage response = await _httpClient.PostAsync(externalApiUrl, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error sending data to external API");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();

            // Parse the JSON to extract the encrypted response
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
            string encryptedResponse = responseObject?.encryptedResponse;  // ✅ Extract actual encrypted data

            if (string.IsNullOrEmpty(encryptedResponse))
            {
                throw new Exception("Received empty or invalid encrypted response.");
            }

            // Decrypt the response
            string decryptedResponse = _aesHelper.Decrypt(encryptedResponse);

            return decryptedResponse;
        }
    }
}
