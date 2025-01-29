using System.Text.Json;
using apiEndpoint.Model;
using apiEndpoint.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Encryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {

        private readonly AesEncryptionHelper _aesHelper;

        public HomeController()
        {
            // Key and IV must match the sender's encryption logic
            string aesKey = "IUU+w1HAa9k7bGtT9zb9asJJc42kBbq/l6o3DG2+3v0=";
            string aesIV = "I/hhunYukZED5bY1sJ/V1g==";
            _aesHelper = new AesEncryptionHelper(aesKey, aesIV);
        }

        [HttpPost("receive-encrypted")]
        public IActionResult ReceiveEncryptedData([FromBody] JsonElement payload)
        {
            try
            {
                Console.WriteLine($"Received Payload: {payload}");

                // Extract the encryptedData correctly
                if (!payload.TryGetProperty("encryptedData", out JsonElement encryptedElement))
                {
                    return BadRequest(new { message = "Error", error = "Invalid input format: 'encryptedData' is missing" });
                }

                string encryptedData = encryptedElement.GetString();
                Console.WriteLine($"Extracted Encrypted Data: {encryptedData}");

                // Decrypt the received data
                string decryptedData = _aesHelper.Decrypt(encryptedData);
                Console.WriteLine($"Decrypted Data: {decryptedData}");

                // Deserialize into model
                MyDataModel receivedData = Newtonsoft.Json.JsonConvert.DeserializeObject<MyDataModel>(decryptedData);

                // Process and create response
                var responseList = receivedData.data.Select(item => new DataReturn
                {
                    Name = item.FirstName.ToUpper() +" "+ item.LastName.ToUpper(),
                    Age = item.Age 
                }).ToList();

                var responseModel = responseList;

                // Serialize and encrypt response
                string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(responseModel);
                string encryptedResponse = _aesHelper.Encrypt(jsonResponse);

                return Ok(new { encryptedResponse });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error", error = ex.Message });
            }
        }


    }
}
